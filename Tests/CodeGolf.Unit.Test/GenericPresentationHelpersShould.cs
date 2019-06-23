using System;
using System.Collections.Generic;
using CodeGolf.Domain;
using FluentAssertions;
using Xunit;

namespace CodeGolf.Unit.Test
{
    public class GenericPresentationHelpersShould
    {
        private readonly IReadOnlyList<ParamDescription> intAndString =
            new ParamDescription[]
                {
                    new ParamDescription(typeof(int), "i"), new ParamDescription(typeof(string), "s"),
                };

        [Fact]
        public void DealWithBasicExample()
        {
            var r = GenericPresentationHelpers.DisplayFunction(
                new Challenge<string>(new object[] { 1, "a" }, "hello"),
                this.intAndString,
                typeof(string));
            r.Should().BeEquivalentTo("(1, \"a\") => \"hello\"");
        }

        [Fact]
        public void DealWithIntArrayResponses()
        {
            var r = GenericPresentationHelpers.DisplayFunction(
                new Challenge<int[]>(new object[] { 1, "a" }, new[] { 1, 2, 3 }),
                this.intAndString,
                typeof(int[]));
            r.Should().BeEquivalentTo("(1, \"a\") => [1, 2, 3]");
        }

        [Fact]
        public void DealWithIntArrayNullResponses()
        {
            var r = GenericPresentationHelpers.DisplayFunction(
                new Challenge<int[]>(new object[] { 1, "a" }, null),
                this.intAndString,
                typeof(int[]));
            r.Should().BeEquivalentTo("(1, \"a\") => null");
        }

        [Fact]
        public void DealWithStringArrayResponses()
        {
            var r = GenericPresentationHelpers.DisplayFunction(
                new Challenge<string[]>(new object[] { 1, "a" }, new[] { "a", "b" }),
                this.intAndString,
                typeof(string[]));
            r.Should().BeEquivalentTo("(1, \"a\") => [\"a\", \"b\"]");
        }

        [Fact]
        public void GetFunctionTemplateNoParams()
        {
            var r = GenericPresentationHelpers.GetFuncTemplate(
                new ChallengeSet<string[]>("a", "b", new List<ParamDescription>() { }, new Challenge<string[]>[] { }));
            r.Should().BeEquivalentTo("string[] Main() { return ... ; }");
        }

        [Fact]
        public void GetFunctionTemplateWithParams()
        {
            var r = GenericPresentationHelpers.GetFuncTemplate(
                new ChallengeSet<string[]>("a", "b", this.intAndString, new Challenge<string[]>[] { }));
            r.Should().BeEquivalentTo("string[] Main(int i, string s) { return ... ; }");
        }
    }
}
