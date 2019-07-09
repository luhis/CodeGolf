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
    using System.Linq;

    using CodeGolf.Persistence.Static;

    using Optional.Unsafe;

    public class CodeGolfServiceShould
    {
        private readonly ICodeGolfService codeGolfService = new CodeGolfService(
            new Runner(new SyntaxTreeTransformer(new CancellationTokenInjector()), new ExecutionService(), new ErrorMessageTransformer()),
            new Scorer(), new ChallengeRepository());

        private readonly IReadOnlyList<ParamDescription> noParams =
            new ParamDescription[] { };

        [Fact]
        public void ReturnCorrectResultForHelloWorld()
        {
            var r = this.codeGolfService.Score(
                "public string Main() => \"Hello World\";",
                new ChallengeSet<string>(
                    Guid.NewGuid(),
                    "a",
                    "b",
                    this.noParams,
                    new[] { new Challenge<string>(new object[0], "Hello World") }),
                CancellationToken.None).Result;
            r.AsT0.Should().Be(33);
        }

        [Fact]
        public void ReturnACompileFailure()
        {
            var r = this.codeGolfService.Score(
                "public string Main() => \"Hello World\";;",
                new ChallengeSet<string>(
                    Guid.NewGuid(),
                    "a",
                    "b",
                    this.noParams,
                    new[] { new Challenge<string>(new object[0], "Hello World") }),
                CancellationToken.None).Result;
            r.AsT2.Errors.Should().BeEquivalentTo(
                "(1,38): error CS1519: Invalid token ';' in class, struct, or interface member declaration");
        }

        [Fact]
        public void ReturnAResultFailure()
        {
            var r = this.codeGolfService.Score(
                "public string Main() => \"Hello X World\";",
                new ChallengeSet<string>(
                    Guid.NewGuid(),
                    "a",
                    "b",
                    this.noParams,
                    new[] { new Challenge<string>(new object[0], "Hello World") }),
                CancellationToken.None).Result;
            r.AsT1.First().Error.ValueOrFailure().Should().BeEquivalentTo(
                "Return value incorrect. Expected: \"Hello World\", Found: \"Hello X World\"");
        }
    }
}
