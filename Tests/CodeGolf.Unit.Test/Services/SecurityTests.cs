namespace CodeGolf.Unit.Test.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using CodeGolf.Domain;
    using CodeGolf.ExecutionServer;
    using CodeGolf.Persistence.Static;
    using CodeGolf.Service;
    using CodeGolf.Service.Dtos;
    using FluentAssertions;
    using Xunit;

    public class SecurityTests
    {
        private readonly ICodeGolfService codeGolfService = new CodeGolfService(
            new Runner(new SyntaxTreeTransformer(), new ExecutionService(), new ErrorMessageTransformer()),
            new Scorer(),
            new ChallengeRepository());

        private readonly IReadOnlyList<ParamDescription> noParams = new ParamDescription[] { };

        [Fact]
        public async Task NotAllowFileAccess()
        {
            var r = await this.codeGolfService.Score(
                "public string Main(){System.IO.File.ReadAllBytes(\"a.txt\");return \"a\";}",
                new ChallengeSet<string>(
                    Guid.NewGuid(),
                    "a",
                    "b",
                    this.noParams,
                    new[] { new Challenge<string>(new object[0], "Hello World") }),
                CancellationToken.None);
            r.AsT2.Should().BeEquivalentTo(new CompileErrorMessage(
                1, 21, 35, "The type or namespace name 'File' does not exist in the namespace 'System.IO' (are you missing an assembly reference?)"));
        }

        [Fact]
        public async Task NotAllowReflection()
        {
            var code = "public string Main(){\n" + "Assembly assembly = Assembly.LoadFrom (\"testdll.dll\");\n"
                                                 + "return \"a\";\n}";
            var r = await this.codeGolfService.Score(
                code,
                new ChallengeSet<string>(
                    Guid.NewGuid(),
                    "a",
                    "b",
                    this.noParams,
                    new[] { new Challenge<string>(new object[0], "Hello World") }),
                CancellationToken.None);
            r.AsT2.Should().BeEquivalentTo(
                new CompileErrorMessage(
                    2, 0, 8, "The type or namespace name 'Assembly' could not be found (are you missing a using directive or an assembly reference?)"),
                new CompileErrorMessage(2, 20, 28, "The name 'Assembly' does not exist in the current context"));
        }

        [Fact]
        public async Task NotAllowAdditionalUsings()
        {
            var code = "using System.Reflection;" + "public string Main(){"
                                                  + "Assembly assembly = Assembly.LoadFrom (\"testdll.dll\");"
                                                  + "return \"a\";}";
            var r = await this.codeGolfService.Score(
                code,
                new ChallengeSet<string>(
                    Guid.NewGuid(),
                    "a",
                    "b",
                    this.noParams,
                    new[] { new Challenge<string>(new object[0], "Hello World") }),
                CancellationToken.None);
            r.AsT2.Should().BeEquivalentTo(
                new CompileErrorMessage(6, -3, -3, "} expected"),
                new CompileErrorMessage(1, 0, 24, "A using clause must precede all other elements defined in the namespace except extern alias declarations"),
                new CompileErrorMessage(2, -4, -3, "Type or namespace definition, or end-of-file expected"),
                new CompileErrorMessage(1, 45, 53, "The type or namespace name 'Assembly' could not be found (are you missing a using directive or an assembly reference?)"),
                new CompileErrorMessage(1, 65, 73, "The name 'Assembly' does not exist in the current context"));
        }

        [Fact]
        public async Task HandleDoubleClasses()
        {
            var code = "}" + "using System.Reflection;" + "public class Naughty {" + "public string Main(){"
                       + "return \"a\";}";
            var r = await this.codeGolfService.Score(
                code,
                new ChallengeSet<string>(
                    Guid.NewGuid(),
                    "a",
                    "b",
                    this.noParams,
                    new[] { new Challenge<string>(new object[0], "Hello World") }),
                CancellationToken.None);
            r.AsT2.Should().BeEquivalentTo(new CompileErrorMessage(
                1, 1, 25, "A using clause must precede all other elements defined in the namespace except extern alias declarations"));
        }
    }
}
