using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeGolf
{
    public class Runner
    {
        private const string ClassName = "CodeGolf";
        private const string FunctionName = "Main";

        public T Execute<T>(string code, object[] args)
        {
            var assembly = Compile(code);

            var type = assembly.GetType($"{ClassName}");
            var fun = type.GetMethod(FunctionName);
            ValidateCompiledFunction(fun, typeof(T), GetParamTypes(args));
            var obj = Activator.CreateInstance(type);
            return (T) fun.Invoke(obj,
                BindingFlags.Default | BindingFlags.InvokeMethod,
                null,
                args,
                CultureInfo.InvariantCulture);
        }

        private static IEnumerable<Type> GetParamTypes(IEnumerable<object> ps) => ps.Select(a => a.GetType());

        private static void ValidateCompiledFunction(MethodInfo fun, Type expectedReturn, IEnumerable<Type> paramTypes)
        {
            if (fun == null)
            {
                throw new Exception($"Function '{FunctionName}' missing");
            }

            if (fun.GetParameters().Length != paramTypes.Count())
            {
                throw new Exception($"Incorrect parameter count expected {paramTypes.Count()}");
            }

            if (expectedReturn != fun.ReturnType)
            {
                throw new Exception($"Return type incorrect expected {expectedReturn}");
            }

            var missMatches = fun.GetParameters().Select(a => a.ParameterType).Zip(paramTypes, (typeA, typeB) => (typeA, typeB)).Where(a => a.typeA != a.typeB);
            if (missMatches.Any())
            {
                throw new Exception("Parameter type mismatch");
            }
        }

        private static string WrapInClass(string function)
        {
            return @"using System;" 
                 + $"public class {ClassName}"
                 + "{" 
                   + function
                 + "}";
        }

        private static Assembly Compile(string function)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(WrapInClass(function));

            var assemblyName = Path.GetRandomFileName();
            var references = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location)
            };

            var compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);

                if (!result.Success)
                {
                    var failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    throw new Exception(string.Join(",\n", failures));
                }
                else
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    return Assembly.Load(ms.ToArray());
                }
            }
        }
    }
}
