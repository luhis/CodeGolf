using EnsureThat;

namespace CodeGolf.Domain
{
    public class ChallengeArray<T> : IChallenge
    {
        public ChallengeArray(object[] args, T[] expectedResult)
        {
            this.Args = EnsureArg.IsNotNull(args, nameof(args));
            this.ExpectedResult = expectedResult;
        }

        public object[] Args { get; }

        public T[] ExpectedResult { get; }

        object IChallenge.ExpectedResult => this.ExpectedResult;
    }
}