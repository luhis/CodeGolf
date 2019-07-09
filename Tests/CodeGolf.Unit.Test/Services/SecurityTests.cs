using System.Threading;
using CodeGolf.Domain;
using CodeGolf.ExecutionServer;
using CodeGolf.Service;
using FluentAssertions;
using Xunit;

namespace CodeGolf.Unit.Test.Services
{
    using System;
    using System.Collections.Generic;

    using CodeGolf.Persistence.Static;

    public class SecurityTests
    {
        private readonly ICodeGolfService codeGolfService = new CodeGolfService(
            new Runner(new SyntaxTreeTransformer(new CancellationTokenInjector()), new ExecutionService(), new ErrorMessageTransformer()),
            new Scorer(), new ChallengeRepository());

        private readonly IReadOnlyList<ParamDescription> noParams = new ParamDescription[] { };

        [Fact]
        public void NotAllowFileAccess()
        {
            var r = this.codeGolfService.Score(
                "public string Main(){System.IO.File.ReadAllBytes(\"a.txt\");return \"a\";}",
                new ChallengeSet<string>(
                    Guid.NewGuid(),
                    "a",
                    "b",
                    this.noParams,
                    new[] { new Challenge<string>(new object[0], "Hello World") }),
                CancellationToken.None).Result;
            r.AsT2.Should().BeEquivalentTo(new ErrorSet(
                "(1,21): error CS0234: The type or namespace name 'File' does not exist in the namespace 'System.IO' (are you missing an assembly reference?)"));
        }

        [Fact]
        public void NotAllowReflection()
        {
            var code = "public string Main(){\n" + "Assembly assembly = Assembly.LoadFrom (\"testdll.dll\");\n"
                                                 + "return \"a\";\n}";
            var r = this.codeGolfService.Score(
                code,
                new ChallengeSet<string>(
                    Guid.NewGuid(),
                    "a",
                    "b",
                    this.noParams,
                    new[] { new Challenge<string>(new object[0], "Hello World") }),
                CancellationToken.None).Result;
            r.AsT2.Should().BeEquivalentTo(new ErrorSet(
                "(2,0): error CS0246: The type or namespace name 'Assembly' could not be found (are you missing a using directive or an assembly reference?)",
                "(2,20): error CS0103: The name 'Assembly' does not exist in the current context"));
        }

        [Fact]
        public void NotAllowAdditionalUsings()
        {
            var code = "using System.Reflection;" + "public string Main(){"
                                                  + "Assembly assembly = Assembly.LoadFrom (\"testdll.dll\");"
                                                  + "return \"a\";}";
            var r = this.codeGolfService.Score(
                code,
                new ChallengeSet<string>(
                    Guid.NewGuid(),
                    "a",
                    "b",
                    this.noParams,
                    new[] { new Challenge<string>(new object[0], "Hello World") }),
                CancellationToken.None).Result;
            r.AsT2.Should().BeEquivalentTo(new ErrorSet(
                "(0,-3): error CS1513: } expected",
                "(1,0): error CS1529: A using clause must precede all other elements defined in the namespace except extern alias declarations",
                "(2,-4): error CS1022: Type or namespace definition, or end-of-file expected",
                "(1,45): error CS0246: The type or namespace name 'Assembly' could not be found (are you missing a using directive or an assembly reference?)",
                "(1,65): error CS0103: The name 'Assembly' does not exist in the current context"));
        }

        [Fact]
        public void HandleDoubleClasses()
        {
            var code = "}" + "using System.Reflection;" + "public class Naughty {" + "public string Main(){"
                       + "return \"a\";}";
            var r = this.codeGolfService.Score(
                code,
                new ChallengeSet<string>(
                    Guid.NewGuid(),
                    "a",
                    "b",
                    this.noParams,
                    new[] { new Challenge<string>(new object[0], "Hello World") }),
                CancellationToken.None).Result;
            r.AsT2.Should().BeEquivalentTo(new ErrorSet(
                "(1,1): error CS1529: A using clause must precede all other elements defined in the namespace except extern alias declarations"));
        }
    }
}
