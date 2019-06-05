namespace CodeGolf.Domain
{
    public interface IChallenge
    {
        object[] Args { get; }

        object ExpectedResult { get; }
    }
}