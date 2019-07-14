﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;
using CodeGolf.Domain;
using CodeGolf.ServiceInterfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Optional;
using ResultOrError = Optional.Option<object, string>;

namespace CodeGolf.Service
{
    using CodeGolf.Service.Dtos;

    public class Runner : IRunner
    {
        private const string ClassName = "CodeGolf";

        private const string FunctionName = "Main";

        private readonly ISyntaxTreeTransformer syntaxTreeTransformer;

        private readonly IExecutionService svc;

        private readonly IErrorMessageTransformer errorMessageTransformer;

        private static readonly MetadataReference[] MetadataReferences =
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(AssemblyTargetedPatchBandAttribute).Assembly.Location)
            };

        public Runner(
            ISyntaxTreeTransformer syntaxTreeTransformer,
            IExecutionService svc,
            IErrorMessageTransformer errorMessageTransformer)
        {
            this.syntaxTreeTransformer = syntaxTreeTransformer;
            this.svc = svc;
            this.errorMessageTransformer = errorMessageTransformer;
        }

        Option<CompileResult, IReadOnlyList<CompileErrorMessage>> IRunner.Compile(
            string function,
            IReadOnlyList<Type> paramTypes,
            Type returnType,
            CancellationToken cancellationToken)
        {
            var compileResult = this.Compile(function, cancellationToken);

            return compileResult.FlatMap(
                success =>
                    {
                        var ass = Assembly.Load(success.Item1);
                        var type = ass.GetType(ClassName);
                        var fun = type.GetMethod(
                            FunctionName,
                            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        var validationFailures = ValidateCompiledFunction(fun, returnType, paramTypes);
                        if (validationFailures.Errors.Any())
                        {
                            return Option.None<CompileResult, IReadOnlyList<CompileErrorMessage>>(validationFailures.Errors.Select(a => new CompileErrorMessage(a)).ToArray());
                        }

                        async Task<IReadOnlyList<ResultOrError>> Func(object[][] args)
                        {
                            try
                            {
                                return await this.InvokeAsync(success.Item1, success.Item2, args, paramTypes.ToArray(), returnType);
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

                        return Option.Some<CompileResult, IReadOnlyList<CompileErrorMessage>>(new CompileResult(Func));
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

        private async Task<IReadOnlyList<Option<object, string>>> InvokeAsync(
            byte[] dll,
            byte[] pdb,
            object[][] args,
            Type[] paramTypes,
            Type returnType)
        {
            if (returnType == typeof(int[]))
            {
                return (await this.svc.Execute<int[]>(dll, pdb, ClassName, FunctionName, args, paramTypes)).Select(ToOpt)
                    .ToArray();
            }

            if (returnType == typeof(string[]))
            {
                return (await this.svc.Execute<string[]>(dll, pdb, ClassName, FunctionName, args, paramTypes))
                    .Select(ToOpt).ToArray();
            }

            if (returnType.IsArray)
            {
                return (await this.svc.Execute<object[]>(dll, pdb, ClassName, FunctionName, args, paramTypes))
                    .Select(ToOpt).ToArray();
            }
            else
            {
                return (await this.svc.Execute<object>(dll, pdb, ClassName, FunctionName, args, paramTypes))
                    .Select(ToOpt).ToArray();
            }
        }

        private static Option<object, string> ToOpt<T>(ValueTuple<T, string> t) =>
            t.Item2 == null ? Option.Some<object, string>(t.Item1) : Option.None<object, string>(t.Item2);

        private static ErrorSet ValidateCompiledFunction(
            MethodInfo fun,
            Type expectedReturn,
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

            var missMatches = compiledParams.Select(a => a.ParameterType).Zip(paramTypes, ValueTuple.Create)
                .Where(a => a.Item1 != a.Item2);
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
            this.TryCompile(WrapInClass(string.Empty, cancellationToken), (_, __) => true, cancellationToken);
        }

        private static SyntaxTree WrapInClass(string function, CancellationToken cancellationToken)
        {
            var transformed = string.Join("\n", function.Split('\n').Select(s => "    " + s));
            return CSharpSyntaxTree.ParseText(
                "using System;\n" + "using System.Collections.Generic;\n" + "using System.Linq;\n\n"
                + $"public class {ClassName}\n" + "{\n" + transformed + "\n}",
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

        private Option<(byte[], byte[]), IReadOnlyList<CompileErrorMessage>> Compile(string function, CancellationToken cancellationToken)
        {
            var syntaxTree = WrapInClass(function, cancellationToken);

            // compile the basic source first, then the modified source to keep the error messages readable
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
                                    return (dll.ToArray(), pdb.ToArray());
                            }, cancellationToken);
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
                                                        ls.StartLinePosition.Line,
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
