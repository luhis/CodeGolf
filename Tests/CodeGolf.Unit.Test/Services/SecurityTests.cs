using System;
using CodeGolf.Domain;
using CodeGolf.Service;
using CodeGolf.Unit.Test.Tooling;
using FluentAssertions;
using Xunit;

namespace CodeGolf.Unit.Test.Services
{
    public class SecurityTests
    {
        private readonly ICodeGolfService codeGolfService = new CodeGolfService(new Runner(new SyntaxTreeTransformer(new CancellationTokenInjector())), new Scorer());

        [Fact]
        public void NotAllowFileAccess()
        {
            var r = this.codeGolfService.Score(
                "public string Main(){System.IO.File.ReadAllBytes(\"a.txt\");return \"a\";}",
                new ChallengeSet<string>("a", "b", new Type[] { },
                    new[] {new Challenge<string>(new object[0], "Hello World")})).Result;
            r.ExtractErrors().Should()
                .BeEquivalentTo(
                    "(8,5): error CS0234: The type or namespace name 'File' does not exist in the namespace 'System.IO' (are you missing an assembly reference?)");
        }

        [Fact]
        public void NotAllowReflection()
        {
            var code = @"public string Main(){" +
                       "Assembly assembly = Assembly.LoadFrom (\"testdll.dll\");" + 
                       "return \"a\";}";
            var r = this.codeGolfService.Score(
                code,
                new ChallengeSet<string>("a", "b", new Type[] { },
                    new[] { new Challenge<string>(new object[0], "Hello World") })).Result;
            r.ExtractErrors().Should()
                .BeEquivalentTo(
                    "(8,5): error CS0246: The type or namespace name 'Assembly' could not be found (are you missing a using directive or an assembly reference?)", 
                    "(8,25): error CS0103: The name 'Assembly' does not exist in the current context");
        }

        [Fact]
        public void NotAllowAdditionalUsings()
        {
            var code = "using System.Reflection;" + 
                       "public string Main(){" +
                       "Assembly assembly = Assembly.LoadFrom (\"testdll.dll\");" +
                       "return \"a\";}";
            var r = this.codeGolfService.Score(
                code,
                new ChallengeSet<string>("a", "b", new Type[] { },
                    new[] { new Challenge<string>(new object[0], "Hello World") })).Result;
            r.ExtractErrors().Should()
                .BeEquivalentTo(
                    "(5,2): error CS1513: } expected",
                    "(6,5): error CS1529: A using clause must precede all other elements defined in the namespace except extern alias declarations", 
                    "(10,3): error CS1022: Type or namespace definition, or end-of-file expected", 
                    "(8,5): error CS0246: The type or namespace name 'Assembly' could not be found (are you missing a using directive or an assembly reference?)", 
                    "(8,25): error CS0103: The name 'Assembly' does not exist in the current context");
        }

        [Fact]
        public void HandleDoubleClasses()
        {
            var code = "}" +
                       "using System.Reflection;" +
                       "public class Naughty {" +
                       "public string Main(){" +
                       "return \"a\";}";
            var r = this.codeGolfService.Score(
                code,
                new ChallengeSet<string>("a", "b", new Type[] { },
                    new[] { new Challenge<string>(new object[0], "Hello World") })).Result;
            r.ExtractErrors().Should()
                .BeEquivalentTo(
                    "(6,6): error CS1529: A using clause must precede all other elements defined in the namespace except extern alias declarations");
        }
    }
}