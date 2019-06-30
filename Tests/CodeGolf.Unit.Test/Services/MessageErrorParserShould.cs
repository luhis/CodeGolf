namespace CodeGolf.Unit.Test.Services
{
    using CodeGolf.Service;
    using CodeGolf.Service.Dtos;

    using FluentAssertions;

    using Xunit;

    public class ErrorMessageParserShould
    {
        [Fact]
        public void ParseSuccess()
        {
            var r  = ErrorMessageParser.Parse("(7,35): error CS0103: The name 'a' does not exist in the current context");
            r.Should().BeEquivalentTo(new ErrorMessage(7, 35, "error CS0103: The name 'a' does not exist in the current context"));
        }
    }
}