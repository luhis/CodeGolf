using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CodeGolf.Dtos;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Optional;

namespace CodeGolf
{
    public class Runner : IRunner
    {
        private const string ClassName = "CodeGolf";
        private const string FunctionName = "Main";

        Option<Func<object[], Task<Option<T, ErrorSet>>>, ErrorSet> IRunner.Compile<T>(
            string function, IReadOnlyList<Type> paramTypes)
        {
            var assembly = Compile(function);

            return assembly.FlatMap(success =>
            {
                var type = success.GetType(ClassName);
                var fun = type.GetMethod(FunctionName);
                var validationFailures = ValidateCompiledFunction(fun, typeof(T), paramTypes);
                if (validationFailures.Errors.Any())
                {
                    return Option.None<Func<object[], Task<Option<T, ErrorSet>>>, ErrorSet>(validationFailures);
                }

                Task<Option<T, ErrorSet>> Func(object[] args)
                {
                    var obj = Activator.CreateInstance(type);
                    try
                    {
                        var t = Task.Run(() => fun.Invoke(obj, BindingFlags.Default | BindingFlags.InvokeMethod,
                            null, args, CultureInfo.InvariantCulture));
                        var r = t.Wait(TimeSpan.FromMilliseconds(100));
                        if (!r)
                        {
                            throw new Exception("A task was canceled.");
                        }
                        return Task.FromResult(Option.Some<T, ErrorSet>((T)t.Result));
                    }
                    catch (Exception e)
                    {
                        return Task.FromResult(Option.None<T, ErrorSet>(new ErrorSet (e.Message)));
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
                return new ErrorSet($"Function '{FunctionName}' missing");
            }

            if (fun.GetParameters().Length != paramTypes.Count)
            {
                return new ErrorSet($"Incorrect parameter count expected {paramTypes.Count}");
            }

            if (expectedReturn != fun.ReturnType)
            {
                return new ErrorSet($"Return type incorrect expected {expectedReturn}");
            }

            var missMatches = fun.GetParameters().Select(a => a.ParameterType)
                .Zip(paramTypes, ValueTuple.Create).Where(a => a.Item1 != a.Item2);
            if (missMatches.Any())
            {
                return new ErrorSet("Parameter type mismatch");
            }

            return new ErrorSet();
        }

        private static string WrapInClass(string function)
        {
            return @"using System;"
                   + $"public class {ClassName}"
                   + "{"
                   + function
                   + "}";
        }

        private static T UseTempFile<T>(Func<string> gen, Func<string, T> process)
        {
            var fileName = gen();

            var res = process(fileName);

            File.Delete(fileName);

            return res;
        }

        private static Option<Assembly, ErrorSet> Compile(string function)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(WrapInClass(function));

            return UseTempFile(Path.GetRandomFileName, assemblyName =>
            {
                var references = new MetadataReference[]
                {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location)
                };

                var compilation = CSharpCompilation.Create(
                    assemblyName,
                    syntaxTrees: new[] {syntaxTree},
                    references: references,
                    options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

                using (var ms = new MemoryStream())
                {
                    var result = compilation.Emit(ms);

                    if (!result.Success)
                    {
                        var failures = result.Diagnostics.Where(diagnostic =>
                            diagnostic.IsWarningAsError ||
                            diagnostic.Severity == DiagnosticSeverity.Error).Select(a => a.ToString());

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
