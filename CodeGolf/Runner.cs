using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeGolf
{
    public class Runner
    {
        private const string CodeNamespace = "RoslynCompileSample";
        private const string ClassName = "Writer";
        private const string FunctionName = "Write";

        public T Execute<T>(string code, object[] args)
        {
            var assembly = Compile(code);

            var type = assembly.GetType($"{CodeNamespace}.{ClassName}");
            ValidateCompiledFunction(type, args.Length);
            var obj = Activator.CreateInstance(type);
            return (T) type.InvokeMember(FunctionName,
                BindingFlags.Default | BindingFlags.InvokeMethod,
                null,
                obj,
                args);
        }

        private static void ValidateCompiledFunction(Type type, int parameterCount)
        {
            var fun = type.GetMethod(FunctionName);
            if (fun == null)
            {
                throw new Exception($"Function '{FunctionName}' missing");
            }

            if (fun.GetParameters().Length != parameterCount)
            {
                throw new Exception($"Incorrect parameter count on function '{FunctionName}' expected {parameterCount}");
            }
        }

        private static string WrapInNamespace(string function)
        {
            return @"using System;" +

            $"namespace {CodeNamespace}"
            + @"{"
                 + $"public class {ClassName}"
                 + "{" 
                   + function +
                @"}
            }";
        }

        public static Assembly Compile(string function)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(WrapInNamespace(function));

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
