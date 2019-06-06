using System;
using System.Linq;
using System.Threading;
using CodeGolf.Domain;
using CodeGolf.ExecutionServer;
using CodeGolf.Service;
using CodeGolf.Unit.Test.Tooling;
using FluentAssertions;
using Optional.Unsafe;
using Xunit;

namespace CodeGolf.Unit.Test.Services
{
    public class CodeGolfServiceShould
    {
        private readonly ICodeGolfService codeGolfService = new CodeGolfService(new Runner(new SyntaxTreeTransformer(new CancellationTokenInjector()), new ExecutionService()), new Scorer());

        [Fact]
        public void ReturnCorrectResultForHelloWorld()
        {
            var r = this.codeGolfService.Score(
                "public string Main() => \"Hello World\";",
                new ChallengeSet<string>("a", "b", new Type[] { }, new[]
                {
                    new Challenge<string>(new object[0], "Hello World")
                }), CancellationToken.None).Result;
            r.ExtractSuccess().ExtractSuccess().Should().Be(33);
        }

        [Fact]
        public void ReturnACompileFailure()
        {
            var r = this.codeGolfService.Score(
                "public string Main() => \"Hello World\";;",
                new ChallengeSet<string>("a", "b", new Type[] { }, new[]
                {
                    new Challenge<string>(new object[0], "Hello World")
                }), CancellationToken.None).Result;
            r.ExtractErrors().Should().BeEquivalentTo("(7,43): error CS1519: Invalid token ';' in class, struct, or interface member declaration");
        }

        [Fact]
        public void ReturnAResultFailure()
        {
            var r = this.codeGolfService.Score(
                "public string Main() => \"Hello X World\";",
                new ChallengeSet<string>("a", "b", new Type[] { }, new[]
                {
                    new Challenge<string>(new object[0], "Hello World")
                }), CancellationToken.None).Result;
            r.ExtractSuccess().ExtractErrors().First().Error.ValueOrFailure().Should().BeEquivalentTo("Return value incorrect. Expected: \"Hello World\", Found: \"Hello X World\"");
        }
    }
}