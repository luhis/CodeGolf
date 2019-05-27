using System;
using CodeGolf.Domain;
using CodeGolf.Service;
using CodeGolf.Unit.Test.Tooling;
using FluentAssertions;
using Xunit;

namespace CodeGolf.Unit.Test.Services
{
    public class CodeGolfServiceShould
    {
        private readonly ICodeGolfService codeGolfService = new CodeGolfService(new Runner(new SyntaxTreeTransformer(new CancellationTokenInjector())), new Scorer());

        [Fact]
        public void ReturnCorrectResultForHelloWorld()
        {
            var r = this.codeGolfService.Score(
                "public string Main() => \"Hello World\";",
                new ChallengeSet<string>("a", "b", new Type[] { }, new[]
                {
                    new Challenge<string>(new object[0], "Hello World")
                })).Result;
            r.ExtractSuccess().Should().Be(33);
        }

        [Fact]
        public void ReturnACompileFailure()
        {
            var r = this.codeGolfService.Score(
                "public string Main() => \"Hello World\";;",
                new ChallengeSet<string>("a", "b", new Type[] { }, new[]
                {
                    new Challenge<string>(new object[0], "Hello World")
                })).Result;
            r.ExtractErrors().Should().BeEquivalentTo("(6,92): error CS1519: Invalid token ';' in class, struct, or interface member declaration");
        }

        [Fact]
        public void ReturnAResultFailure()
        {
            var r = this.codeGolfService.Score(
                "public string Main() => \"Hello X World\";",
                new ChallengeSet<string>("a", "b", new Type[] { }, new[]
                {
                    new Challenge<string>(new object[0], "Hello World")
                })).Result;
            r.ExtractErrors().Should().BeEquivalentTo("Return value incorrect. Expected: Hello World, Found: Hello X World");
        }
    }
}