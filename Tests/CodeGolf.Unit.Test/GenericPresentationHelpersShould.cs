namespace CodeGolf.Unit.Test
{
    using System;
    using System.Collections.Generic;
    using CodeGolf.Domain;
    using FluentAssertions;
    using Xunit;

    public class GenericPresentationHelpersShould
    {
        private readonly IReadOnlyList<ParamDescription> intAndString =
            new ParamDescription[]
                {
                    new ParamDescription(typeof(int), "i"), new ParamDescription(typeof(string), "s"),
                };

        [Fact]
        public void GetFunctionTemplateNoParams()
        {
            var r = GenericPresentationHelpers.GetFuncTemplate(
                new ChallengeSet<string[]>(
                    Guid.NewGuid(), "a", "b", Array.Empty<ParamDescription>(), new Challenge<string[]>[] { }));
            r.Should().BeEquivalentTo("string[] Main() { return ... ; }");
        }

        [Fact]
        public void GetFunctionTemplateWithParams()
        {
            var r = GenericPresentationHelpers.GetFuncTemplate(
                new ChallengeSet<string[]>(
                    Guid.NewGuid(), "a", "b", this.intAndString, new Challenge<string[]>[] { }));
            r.Should().BeEquivalentTo("string[] Main(int i, string s) { return ... ; }");
        }
    }
}
