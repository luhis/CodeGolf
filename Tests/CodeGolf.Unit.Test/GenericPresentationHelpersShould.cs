using CodeGolf.Domain;
using FluentAssertions;
using Xunit;

namespace CodeGolf.Unit.Test
{
    public class GenericPresentationHelpersShould
    {
        [Fact]
        public void DealWithBasicExample()
        {
            var r = GenericPresentationHelpers.DisplayFunction(new Challenge<string>(new object[] {1, "a"}, "hello"), new[] { typeof(int), typeof(string)}, typeof(string));
            r.Should().BeEquivalentTo("(1, \"a\") => \"hello\"");
        }

        [Fact]
        public void DealWithIntArrayResponses()
        {
            var r = GenericPresentationHelpers.DisplayFunction(new ChallengeArr<int>(new object[] { 1, "a" }, new[] {1, 2, 3}), new[] { typeof(int), typeof(string) }, typeof(int[]));
            r.Should().BeEquivalentTo("(1, \"a\") => [1, 2, 3]");
        }

        [Fact]
        public void DealWithStringArrayResponses()
        {
            var r = GenericPresentationHelpers.DisplayFunction(new ChallengeArr<string>(new object[] { 1, "a" }, new[] { "a", "b" }), new[] { typeof(int), typeof(string) }, typeof(string[]));
            r.Should().BeEquivalentTo("(1, \"a\") => [a, b]");
        }
    }
}
