using CodeGolf.Dtos;
using FluentAssertions;
using Xunit;

namespace CodeGolf.Unit.Test
{
    public class CodeGolfServiceShould
    {
        private readonly ICodeGolfService codeGolfService = new CodeGolfService();

        [Fact]
        public void ReturnCorrectResultForHelloWorld()
        {
            var r = this.codeGolfService.Score("public string Main() => \"Hello World\";", new Challenge<string>(new object[0], "Hello World"));
            r.ExtractSuccess().Should().Be(38);
        }
    }
}