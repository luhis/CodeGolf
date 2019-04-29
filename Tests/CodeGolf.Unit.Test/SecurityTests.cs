using CodeGolf.Dtos;
using FluentAssertions;
using Xunit;

namespace CodeGolf.Unit.Test
{
    public class SecurityTests
    {
        private readonly ICodeGolfService codeGolfService = new CodeGolfService();

        [Fact]
        public void NotAllowFileAccess()
        {
            var r = this.codeGolfService.Score(
                "public string Main(){System.IO.File.ReadAllBytes(\"a.txt\");return \"a\";}",
                new Challenge<string>(new object[0], "Hello World"));
            r.ExtractErrors().Should()
                .BeEquivalentTo(
                    "(1,57): error CS0234: The type or namespace name 'File' does not exist in the namespace 'System.IO' (are you missing an assembly reference?)");
        }
    }
}