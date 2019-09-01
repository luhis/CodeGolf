namespace CodeGolf.Web.Models
{
    using System.Collections.Generic;

    public class ChallengeDto
    {
        public ChallengeDto(IReadOnlyList<string> args, string expectedResult)
        {
            this.Args = args;
            this.ExpectedResult = expectedResult;
        }

        public IReadOnlyList<string> Args { get; }

        public string ExpectedResult { get; }
    }
}
