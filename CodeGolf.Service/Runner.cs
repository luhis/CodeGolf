using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;
using CodeGolf.Domain;
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

        private static readonly MetadataReference[] MetadataReferences =
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(AssemblyTargetedPatchBandAttribute).Assembly.Location)
        };

        public Runner(ISyntaxTreeTransformer syntaxTreeTransformer)
        {
            this.syntaxTreeTransformer = syntaxTreeTransformer;
        }

        Option<Func<IChallenge, Task<Option<object, string>>>, ErrorSet> IRunner.Compile(
            string function, IReadOnlyList<Type> paramTypes, Type returnType, CancellationToken cancellationToken)
        {
            var assembly = this.Compile(function, cancellationToken);

            return assembly.FlatMap(success =>
            {
                var type = success.GetType(ClassName);
                var fun = type.GetMethod(FunctionName,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                var validationFailures = ValidateCompiledFunction(fun, returnType, paramTypes);
                if (validationFailures.Errors.Any())
                {
                    return Option.None<Func<IChallenge, Task<Option<object, string>>>, ErrorSet>(validationFailures);
                }

                async Task<Option<object, string>> Func(IChallenge challenge)
                {
                    var obj = Activator.CreateInstance(type);
                    try
                    {
                        var source = new CancellationTokenSource();
                        source.CancelAfter(TimeSpan.FromMilliseconds(ExecutionTimeoutMilliseconds));
                        var task = Task<object>.Factory.StartNew(() => fun.Invoke(obj,
                            BindingFlags.Default | BindingFlags.InvokeMethod,
                            null, challenge.Args.Append(source.Token).ToArray(), CultureInfo.InvariantCulture), source.Token);

                        return Option.Some<object, string>(await task);
                    }
                    catch (Exception e)
                    {
                        return Option.None<object, string>(e.InnerException.Message);
                    }
                }

                return Option.Some<Func<IChallenge, Task<Option<object, string>>>, ErrorSet>(Func);
            });
        }

        private static ErrorSet ValidateCompiledFunction(MethodInfo fun, Type expectedReturn,
            IReadOnlyCollection<Type> paramTypes)
        {
            if (fun == null)
            {
                return new ErrorSet($"Function '{FunctionName}' missing");
            }

            var compiledParams = fun.GetParameters().Take(fun.GetParameters().Length - 1);

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

        string IRunner.Wrap(string function, CancellationToken cancellationToken) =>
            WrapInClass(function, cancellationToken).GetRoot().NormalizeWhitespace().ToFullString();

        string IRunner.DebugCode(string function, CancellationToken cancellationToken)
        {
            var syntaxTree = WrapInClass(function, cancellationToken);

            var transformed = this.syntaxTreeTransformer.Transform(syntaxTree);
            return transformed.GetRoot().NormalizeWhitespace().ToFullString();
        }

        void IRunner.WakeUpCompiler(CancellationToken cancellationToken)
        {
            TryCompile(WrapInClass(string.Empty, cancellationToken), cancellationToken);
        }

        private static SyntaxTree WrapInClass(string function, CancellationToken cancellationToken)
        {
            var transformed = string.Join("\n", function.Split('\n').Select(s => "    " + s));
            return CSharpSyntaxTree.ParseText("using System;\n"
                                              + "using System.Collections.Generic;\n"
                                              + "using System.Linq;\n\n"
                                              + $"public class {ClassName}\n"
                                              + "{\n"
                                              + transformed
                                              + "\n}", cancellationToken: cancellationToken);
        }

        private static T UseTempFile<T>(Func<string> gen, Func<string, T> process)
        {
            var fileName = gen();

            var res = process(fileName);

            File.Delete(fileName);

            return res;
        }

        private static bool IsStoppable(Diagnostic a) => a.Severity > DiagnosticSeverity.Warning;

        private Option<Assembly, ErrorSet> Compile(string function, CancellationToken cancellationToken)
        {
            var syntaxTree = WrapInClass(function, cancellationToken);

            // compile the basic source first, then the modified source to keep the error messages readable
            return TryCompile(syntaxTree, cancellationToken).FlatMap(_ =>
            {
                var transformed = this.syntaxTreeTransformer.Transform(syntaxTree);

                return TryCompile(transformed, cancellationToken);
            });
        }

        private static Option<Assembly, ErrorSet> TryCompile(SyntaxTree syntaxTree, CancellationToken cancellationToken)
        {
            return UseTempFile(Path.GetRandomFileName, assemblyName =>
            {
                var compilation = CSharpCompilation.Create(
                    assemblyName,
                    syntaxTrees: new[] {syntaxTree},
                    references: MetadataReferences,
                    options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, warningLevel: 4,
                        reportSuppressedDiagnostics: true, allowUnsafe: false));

                using (var ms = new MemoryStream())
                {
                    var result = compilation.Emit(ms, cancellationToken: cancellationToken);

                    if (result.Diagnostics.Any(IsStoppable))
                    {
                        var failures = result.Diagnostics.Where(IsStoppable).Select(a => a.ToString());

                        return Option.None<Assembly, ErrorSet>(new ErrorSet(failures.ToList()));
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
