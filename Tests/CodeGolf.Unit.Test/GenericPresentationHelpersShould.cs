using System;
using System.Collections.Generic;
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
            var r = GenericPresentationHelpers.DisplayFunction(new ChallengeArray<int>(new object[] { 1, "a" }, new[] {1, 2, 3}), new[] { typeof(int), typeof(string) }, typeof(int[]));
            r.Should().BeEquivalentTo("(1, \"a\") => [1, 2, 3]");
        }

        [Fact]
        public void DealWithIntArrayNullResponses()
        {
            var r = GenericPresentationHelpers.DisplayFunction(new ChallengeArray<int>(new object[] { 1, "a" }, null), new[] { typeof(int), typeof(string) }, typeof(int[]));
            r.Should().BeEquivalentTo("(1, \"a\") => null");
        }

        [Fact]
        public void DealWithStringArrayResponses()
        {
            var r = GenericPresentationHelpers.DisplayFunction(new ChallengeArray<string>(new object[] { 1, "a" }, new[] { "a", "b" }), new[] { typeof(int), typeof(string) }, typeof(string[]));
            r.Should().BeEquivalentTo("(1, \"a\") => [\"a\", \"b\"]");
        }

        [Fact]
        public void GetFunctionTemplateNoParams()
        {
            var r = GenericPresentationHelpers.GetFuncTemplate(new ChallengeSet<string[]>("a", "b",
                new List<Type>() { }, new Challenge<string[]>[] { }));
            r.Should().BeEquivalentTo("string[] Main() { return ...; }");
        }

        [Fact]
        public void GetFunctionTemplateWithParams()
        {
            var r = GenericPresentationHelpers.GetFuncTemplate(new ChallengeSet<string[]>("a", "b",
                new List<Type>() { typeof(int), typeof(string) }, new Challenge<string[]>[] { }));
            r.Should().BeEquivalentTo("string[] Main(int a, string b) { return ...; }");
        }
    }
}
