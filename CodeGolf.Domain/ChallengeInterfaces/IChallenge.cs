namespace CodeGolf.Domain.ChallengeInterfaces
{
    public interface IChallenge
    {
        object[] Args { get; }

        object ExpectedResult { get; }
    }
}