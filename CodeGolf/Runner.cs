﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Optional;

namespace CodeGolf
{
    public class Runner
    {
        private const string ClassName = "CodeGolf";
        private const string FunctionName = "Main";

        public Option<Func<object[], Option<T, IReadOnlyList<string>>>, IReadOnlyList<string>> Compile<T>(string function)
        {
            var assembly = Compile(function);

            return assembly.Map(success =>
            {
                var type = success.GetType($"{ClassName}");
                var fun = type.GetMethod(FunctionName);

                Option<T, IReadOnlyList<string>> Func(object[] args)
                {
                    var validationFailures = ValidateCompiledFunction(fun, typeof(T), GetParamTypes(args).ToList());
                    if (validationFailures.Any())
                    {
                        return Option.None<T, IReadOnlyList<string>>(validationFailures);
                    }

                    var obj = Activator.CreateInstance(type);
                    return Option.Some<T, IReadOnlyList<string>>((T) fun.Invoke(obj, BindingFlags.Default | BindingFlags.InvokeMethod, null, args, CultureInfo.InvariantCulture));
                }

                return (Func<object[], Option<T, IReadOnlyList<string>>>) Func;

            });
        }

        private static IEnumerable<Type> GetParamTypes(IEnumerable<object> ps) => ps.Select(a => a.GetType());

        private static IReadOnlyList<string> ValidateCompiledFunction(MethodInfo fun, Type expectedReturn,
            IReadOnlyCollection<Type> paramTypes)
        {
            if (fun == null)
            {
                return new[] {$"Function '{FunctionName}' missing"};
            }

            if (fun.GetParameters().Length != paramTypes.Count)
            {
                return new[] {$"Incorrect parameter count expected {paramTypes.Count}"};
            }

            if (expectedReturn != fun.ReturnType)
            {
                return new[] {$"Return type incorrect expected {expectedReturn}"};
            }

            var missMatches = fun.GetParameters().Select(a => a.ParameterType)
                .Zip(paramTypes, (typeA, typeB) => (typeA, typeB)).Where(a => a.typeA != a.typeB);
            if (missMatches.Any())
            {
                return new[] {"Parameter type mismatch"};
            }

            return new string[] { };
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

        private static Option<Assembly, IReadOnlyList<string>> Compile(string function)
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

                        return Option.None<Assembly, IReadOnlyList<string>>(failures.ToList());
                    }
                    else
                    {
                        ms.Seek(0, SeekOrigin.Begin);
                        return Option.Some<Assembly, IReadOnlyList<string>>(Assembly.Load(ms.ToArray()));
                    }
                }
            });
        }
    }
}
