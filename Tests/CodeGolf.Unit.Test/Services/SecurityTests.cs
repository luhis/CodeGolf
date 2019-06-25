using System.Threading;
using CodeGolf.Domain;
using CodeGolf.ExecutionServer;
using CodeGolf.Service;
using CodeGolf.Unit.Test.Tooling;
using FluentAssertions;
using Xunit;

namespace CodeGolf.Unit.Test.Services
{
    using System.Collections.Generic;

    public class SecurityTests
    {
        private readonly ICodeGolfService codeGolfService = new CodeGolfService(
            new Runner(new SyntaxTreeTransformer(new CancellationTokenInjector()), new ExecutionService()),
            new Scorer());

        private readonly IReadOnlyList<ParamDescription> noParams = new ParamDescription[] { };

        [Fact]
        public void NotAllowFileAccess()
        {
            var r = this.codeGolfService.Score(
                "public string Main(){System.IO.File.ReadAllBytes(\"a.txt\");return \"a\";}",
                new ChallengeSet<string>(
                    "a",
                    "b",
                    this.noParams,
                    new[] { new Challenge<string>(new object[0], "Hello World") }),
                CancellationToken.None).Result;
            r.ExtractErrors().Should().BeEquivalentTo(
                "(7,26): error CS0234: The type or namespace name 'File' does not exist in the namespace 'System.IO' (are you missing an assembly reference?)");
        }

        [Fact]
        public void NotAllowReflection()
        {
            var code = "public string Main(){\n" + "Assembly assembly = Assembly.LoadFrom (\"testdll.dll\");\n"
                                                 + "return \"a\";\n}";
            var r = this.codeGolfService.Score(
                code,
                new ChallengeSet<string>(
                    "a",
                    "b",
                    this.noParams,
                    new[] { new Challenge<string>(new object[0], "Hello World") }),
                CancellationToken.None).Result;
            r.ExtractErrors().Should().BeEquivalentTo(
                "(8,5): error CS0246: The type or namespace name 'Assembly' could not be found (are you missing a using directive or an assembly reference?)",
                "(8,25): error CS0103: The name 'Assembly' does not exist in the current context");
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
                    "a",
                    "b",
                    this.noParams,
                    new[] { new Challenge<string>(new object[0], "Hello World") }),
                CancellationToken.None).Result;
            r.ExtractErrors().Should().BeEquivalentTo(
                "(6,2): error CS1513: } expected",
                "(7,5): error CS1529: A using clause must precede all other elements defined in the namespace except extern alias declarations",
                "(8,1): error CS1022: Type or namespace definition, or end-of-file expected",
                "(7,50): error CS0246: The type or namespace name 'Assembly' could not be found (are you missing a using directive or an assembly reference?)",
                "(7,70): error CS0103: The name 'Assembly' does not exist in the current context");
        }

        [Fact]
        public void HandleDoubleClasses()
        {
            var code = "}" + "using System.Reflection;" + "public class Naughty {" + "public string Main(){"
                       + "return \"a\";}";
            var r = this.codeGolfService.Score(
                code,
                new ChallengeSet<string>(
                    "a",
                    "b",
                    this.noParams,
                    new[] { new Challenge<string>(new object[0], "Hello World") }),
                CancellationToken.None).Result;
            r.ExtractErrors().Should().BeEquivalentTo(
                "(7,6): error CS1529: A using clause must precede all other elements defined in the namespace except extern alias declarations");
        }
    }
}
