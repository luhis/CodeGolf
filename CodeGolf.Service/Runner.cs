using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;
using CodeGolf.Service.Dtos;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Optional;

namespace CodeGolf.Service
{
    public class Runner : IRunner
    {
        private const string ClassName = "CodeGolf";
        private const string FunctionName = "Main";
        private const int ExecutionTimeoutMilliseconds = 1000;
        private readonly ISyntaxTreeTransformer syntaxTreeTransformer;
        private static readonly MetadataReference[] MetadataReferences = {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(AssemblyTargetedPatchBandAttribute).Assembly.Location)
        };

        public Runner(ISyntaxTreeTransformer syntaxTreeTransformer)
        {
            this.syntaxTreeTransformer = syntaxTreeTransformer;
        }

        Option<Func<object[], Task<Option<T, ErrorSet>>>, ErrorSet> IRunner.Compile<T>(
            string function, IReadOnlyList<Type> paramTypes)
        {
            var assembly = this.Compile(function);

            return assembly.FlatMap(success =>
            {
                var type = success.GetType(ClassName);
                var fun = type.GetMethod(FunctionName);
                var validationFailures = ValidateCompiledFunction(fun, typeof(T), paramTypes);
                if (validationFailures.Errors.Any())
                {
                    return Option.None<Func<object[], Task<Option<T, ErrorSet>>>, ErrorSet>(validationFailures);
                }

                async Task<Option<T, ErrorSet>> Func(object[] args)
                {
                    var obj = Activator.CreateInstance(type);
                    try
                    {
                        var source = new CancellationTokenSource();
                        source.CancelAfter(TimeSpan.FromMilliseconds(ExecutionTimeoutMilliseconds));
                        var task = Task<object>.Factory.StartNew(() => fun.Invoke(obj,
                            BindingFlags.Default | BindingFlags.InvokeMethod,
                            null, args.Append(source.Token).ToArray(), CultureInfo.InvariantCulture), source.Token);

                        return Option.Some<T, ErrorSet>((T)await task);
                    }
                    catch (Exception e)
                    {
                        return Option.None<T, ErrorSet>(new ErrorSet(e.InnerException.Message));
                    }
                }

                return Option.Some<Func<object[], Task<Option<T, ErrorSet>>>, ErrorSet>(Func);
            });
        }

        private static ErrorSet ValidateCompiledFunction(MethodInfo fun, Type expectedReturn,
            IReadOnlyCollection<Type> paramTypes)
        {
            if (fun == null)
            {
                return new ErrorSet($"Public function '{FunctionName}' missing");
            }

            var compiledParams = fun.GetParameters().Take(fun.GetParameters().Length -1);

            if (compiledParams.Count() != paramTypes.Count)
            {
                return new ErrorSet($"Incorrect parameter count expected {paramTypes.Count}");
            }

            if (expectedReturn != fun.ReturnType)
            {
                return new ErrorSet($"Return type incorrect expected {expectedReturn}");
            }

            var missMatches = compiledParams.Select(a => a.ParameterType)
                .Zip(paramTypes, ValueTuple.Create).Where(a => a.Item1 != a.Item2);
            if (missMatches.Any())
            {
                return new ErrorSet("Parameter type mismatch");
            }

            return new ErrorSet();
        }

        string IRunner.Wrap(string function) => WrapInClass(function);

        string IRunner.DebugCode(string function)
        {
            var classString = WrapInClass(function);
            var syntaxTree = CSharpSyntaxTree.ParseText(classString);

            var transformed = this.syntaxTreeTransformer.Transform(syntaxTree);
            return transformed.GetRoot().NormalizeWhitespace().ToFullString();
        }

        private static string WrapInClass(string function)
        {
            var transformed = string.Join("\n", function.Split('\n').Select(s => "    " + s));
            return "using System;\n"
                   + "using System.Collections.Generic;\n"
                   + "using System.Linq;\n"
                   + $"public class {ClassName}\n"
                   + "{\n"
                   + transformed
                   + "\n}";
        }

        private static T UseTempFile<T>(Func<string> gen, Func<string, T> process)
        {
            var fileName = gen();

            var res = process(fileName);

            File.Delete(fileName);

            return res;
        }

        private static bool IsStoppable(Diagnostic a) => a.Severity > DiagnosticSeverity.Warning;

        private Option<Assembly, ErrorSet> Compile(string function)
        {
            var finalCode = WrapInClass(function);
            var syntaxTree = CSharpSyntaxTree.ParseText(finalCode);

            var transformed = this.syntaxTreeTransformer.Transform(syntaxTree);

            return UseTempFile(Path.GetRandomFileName, assemblyName =>
            {
                var compilation = CSharpCompilation.Create(
                    assemblyName,
                    syntaxTrees: new[] { transformed },
                    references: MetadataReferences,
                    options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, warningLevel: 4, reportSuppressedDiagnostics: true, allowUnsafe: false));

                using (var ms = new MemoryStream())
                {
                    var result = compilation.Emit(ms);

                    if (result.Diagnostics.Any(IsStoppable))
                    {
                        var failures = result.Diagnostics.Where(IsStoppable).Select(a => a.ToString());

                        return Option.None<Assembly, ErrorSet>( new ErrorSet(failures.ToList()));
                    }
                    else
                    {
                        ms.Seek(0, SeekOrigin.Begin);
                        return Option.Some<Assembly, ErrorSet>(Assembly.Load(ms.ToArray()));
                    }
                }
            });
        }
    }
}
