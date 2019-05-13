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
        private readonly ICodeGolfService codeGolfService = new CodeGolfService(new Runner(), new Scorer());

        [Fact]
        public void NotAllowFileAccess()
        {
            var r = this.codeGolfService.Score(
                "public string Main(){System.IO.File.ReadAllBytes(\"a.txt\");return \"a\";}",
                new ChallengeSet<string>("a", "b", new Type[] { },
                    new[] {new Challenge<string>(new object[0], "Hello World")})).Result;
            r.ExtractErrors().Should()
                .BeEquivalentTo(
                    "(6,22): error CS0234: The type or namespace name 'File' does not exist in the namespace 'System.IO' (are you missing an assembly reference?)");
        }
    }
}