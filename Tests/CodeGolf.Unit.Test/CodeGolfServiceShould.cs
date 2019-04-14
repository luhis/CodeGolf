using CodeGolf.Dtos;
using FluentAssertions;
using FluentAssertions.OneOf;
using Xunit;

namespace CodeGolf.Unit.Test
{
    public class CodeGolfServiceShould
    {
        private readonly ICodeGolfService codeGolfService = new CodeGolfService();

        [Fact]
        public void ReturnCorrectResultForHelloWorld()
        {
            var r = this.codeGolfService.Score<string>("public string Main() => \"Hello World\";", new Challenge<string>(new object[0], a => a == "Hello World"));
            r.Should().Be<int>().And.Should().Be(38);
        }
    }
}