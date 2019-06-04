namespace CodeGolf.Domain
{
    public interface IChallenge
    {
        object[] Args { get; }

        string ExpectedResult { get; }
    }
}