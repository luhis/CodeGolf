namespace CodeGolf.Service
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime;
    using System.Runtime.Loader;
    using System.Threading;
    using System.Threading.Tasks;
    using CodeGolf.Domain;
    using CodeGolf.Service.Dtos;
    using CodeGolf.ServiceInterfaces;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Optional;
    using ResultOrError = Optional.Option<object, string>;

    public class Runner : IRunner
    {
        private const string ClassName = "CodeGolf";

        private const string FunctionName = "Main";

        private static readonly MetadataReference[] MetadataReferences =
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(AssemblyTargetedPatchBandAttribute).Assembly.Location)
            };

        private readonly ISyntaxTreeTransformer syntaxTreeTransformer;

        private readonly IExecutionService svc;

        private readonly IErrorMessageTransformer errorMessageTransformer;

        private readonly FunctionValidator functionValidator;

        public Runner(
            ISyntaxTreeTransformer syntaxTreeTransformer,
            IExecutionService svc,
            IErrorMessageTransformer errorMessageTransformer)
        {
            this.syntaxTreeTransformer = syntaxTreeTransformer;
            this.svc = svc;
            this.errorMessageTransformer = errorMessageTransformer;
            this.functionValidator = new FunctionValidator(FunctionName);
        }

        Option<CompileRunner, IReadOnlyList<CompileErrorMessage>> IRunner.Compile(
            string function,
            IReadOnlyList<Type> paramTypes,
            Type returnType,
            CancellationToken cancellationToken)
        {
            var compileResult = this.Compile(function, cancellationToken);

            return compileResult.FlatMap(
                success =>
                    {
                        using (var dll = new MemoryStream(success.Dll))
                        {
                            var ass = AssemblyLoadContext.Default.LoadFromStream(dll);
                            var type = ass.GetType(ClassName);
                            var fun = type.GetMethod(
                                FunctionName,
                                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                            var validationFailures = this.functionValidator.ValidateCompiledFunction(fun, returnType, paramTypes);
                            if (validationFailures.Errors.Any())
                            {
                                return Option.None<CompileRunner, IReadOnlyList<CompileErrorMessage>>(
                                    validationFailures.Errors.Select(a => new CompileErrorMessage(a)).ToArray());
                            }

                            async Task<IReadOnlyList<ResultOrError>> Func(object[][] args)
                            {
                                try
                                {
                                    return await this.InvokeAsync(success, args, paramTypes.ToArray(), returnType);
                                }
                                catch (Exception)
                                {
                                    return new[]
                                               {
                                                   Option.None<object, string>(
                                                       "It looks like someone crashed the microservice. Give it a sec.")
                                               };
                                }
                            }

                            return Option.Some<CompileRunner, IReadOnlyList<CompileErrorMessage>>(
                                new CompileRunner(Func));
                        }
                    });
        }

        Option<IReadOnlyList<CompileErrorMessage>> IRunner.TryCompile(
            string function,
            IReadOnlyList<Type> paramTypes,
            Type returnType,
            CancellationToken cancellationToken)
        {
            var syntaxTree = WrapInClass(function, cancellationToken);

            var compileResult = this.TryCompile(syntaxTree, (_, __) => true, cancellationToken);

            return compileResult.Match(_ => Option.None<IReadOnlyList<CompileErrorMessage>>(), Option.Some);
        }

        string IRunner.Wrap(string function, CancellationToken cancellationToken) =>
            WrapInClass(function, cancellationToken).GetRoot().ToFullString();

        string IRunner.DebugCode(string function, CancellationToken cancellationToken)
        {
            var syntaxTree = WrapInClass(function, cancellationToken);

            var transformed = this.syntaxTreeTransformer.Transform(syntaxTree);
            return transformed.GetRoot().ToFullString();
        }

        void IRunner.WakeUpCompiler(CancellationToken cancellationToken)
        {
            this.TryCompile(WrapInClass(string.Empty, cancellationToken), (_, __) => true, cancellationToken);
        }

        private static Option<object, string> ToOpt<T>(ValueTuple<T, string> t) =>
            t.Item2 == null ? Option.Some<object, string>(t.Item1) : Option.None<object, string>(t.Item2);

        private static SyntaxTree WrapInClass(string function, CancellationToken cancellationToken)
        {
            var transformed = string.Join("\n", function.Split('\n').Select(s => "    " + s));
            return CSharpSyntaxTree.ParseText(
                "using System;\n" + "using System.Collections.Generic;\n" + "using System.Linq;\n\n"
                + $"public class {ClassName}\n" + "{\n" +
                "#line 1\n" +
                transformed + "\n}",
                cancellationToken: cancellationToken,
                options: CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest));
        }

        private static T UseTempFile<T>(Func<string> gen, Func<string, T> process)
        {
            var fileName = gen();

            var res = process(fileName);

            File.Delete(fileName);

            return res;
        }

        private static bool IsStoppable(Diagnostic a) => a.Severity > DiagnosticSeverity.Warning;

        private async Task<IReadOnlyList<Option<object, string>>> InvokeAsync(
            CompileResult compileResult,
            object[][] args,
            Type[] paramTypes,
            Type returnType)
        {
            if (returnType == typeof(int[]))
            {
                return (await this.svc.Execute<int[]>(compileResult, ClassName, FunctionName, args, paramTypes)).Select(ToOpt)
                    .ToArray();
            }

            if (returnType == typeof(string[]))
            {
                return (await this.svc.Execute<string[]>(compileResult, ClassName, FunctionName, args, paramTypes))
                    .Select(ToOpt).ToArray();
            }

            if (returnType.IsArray)
            {
                return (await this.svc.Execute<object[]>(compileResult, ClassName, FunctionName, args, paramTypes))
                    .Select(ToOpt).ToArray();
            }
            else
            {
                return (await this.svc.Execute<object>(compileResult, ClassName, FunctionName, args, paramTypes))
                    .Select(ToOpt).ToArray();
            }
        }

        private Option<CompileResult, IReadOnlyList<CompileErrorMessage>> Compile(
            string function,
            CancellationToken cancellationToken)
        {
            var syntaxTree = WrapInClass(function, cancellationToken);

            // compile the basic source first, then the modified source to keep the error messages readable
            // todo this is hiding issues in the line number modification
            return this.TryCompile(syntaxTree, (_, __) => true, cancellationToken).FlatMap(
                _ =>
                    {
                        var transformed = this.syntaxTreeTransformer.Transform(syntaxTree);

                        return this.TryCompile(
                            transformed,
                            (dll, pdb) =>
                                {
                                    dll.Seek(0, SeekOrigin.Begin);
                                    pdb.Seek(0, SeekOrigin.Begin);
                                    return new CompileResult(dll.ToArray(), pdb.ToArray());
                                },
                            cancellationToken);
                    });
        }

        private Option<T, IReadOnlyList<CompileErrorMessage>> TryCompile<T>(SyntaxTree syntaxTree, Func<MemoryStream, MemoryStream, T> onSuccess, CancellationToken cancellationToken)
        {
            return UseTempFile(
                Path.GetRandomFileName,
                assemblyName =>
                    {
                        var compilation = CSharpCompilation.Create(
                            assemblyName,
                            syntaxTrees: new[] { syntaxTree },
                            references: MetadataReferences,
                            options: new CSharpCompilationOptions(
                                OutputKind.DynamicallyLinkedLibrary,
                                warningLevel: 4,
                                reportSuppressedDiagnostics: true,
                                allowUnsafe: false));

                        using (var dll = new MemoryStream())
                        {
                            using (var pdb = new MemoryStream())
                            {
                                var result = compilation.Emit(dll, pdb, cancellationToken: cancellationToken);

                                if (result.Diagnostics.Any(IsStoppable))
                                {
                                    var failures = result.Diagnostics.Where(IsStoppable).Select(
                                        a =>
                                            {
                                                var ls = a.Location.GetMappedLineSpan();
                                                return this.errorMessageTransformer.Transform(
                                                    new CompileErrorMessage(
                                                        ls.StartLinePosition.Line + 1,
                                                        ls.Span.Start.Character,
                                                        ls.Span.End.Character,
                                                        a.GetMessage()));
                                            });

                                    return Option.None<T, IReadOnlyList<CompileErrorMessage>>(failures.ToList());
                                }
                                else
                                {
                                    return Option.Some<T, IReadOnlyList<CompileErrorMessage>>(onSuccess(dll, pdb));
                                }
                            }
                        }
                    });
        }
    }
}
