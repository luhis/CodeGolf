using CodeGolf.Domain.ChallengeInterfaces;
using EnsureThat;

namespace CodeGolf.Domain
{
    public class Challenge<T> : IChallenge
    {
        public Challenge(object[] args, T expectedResult)
        {
            this.Args = EnsureArg.IsNotNull(args, nameof(args));
            this.ExpectedResult = expectedResult;
        }

        public object[] Args { get; }

        public T ExpectedResult { get; }

        object IChallenge.ExpectedResult => this.ExpectedResult;
    }
}