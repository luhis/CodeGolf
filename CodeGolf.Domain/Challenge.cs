using System;
using EnsureThat;

namespace CodeGolf.Domain
{
    public class Challenge<T> : IChallenge
    {
        public Challenge(object[] args, T expectedResult)
        {
            this.Args = EnsureArg.IsNotNull(args, nameof(args));
            this.ExpectedResult = expectedResult;
            if (typeof(T).IsArray)
            {
                throw new Exception("Challenge should not be used with challenges returning array types");
            }
        }

        public object[] Args { get; }

        public T ExpectedResult { get; }

        object IChallenge.ExpectedResult => this.ExpectedResult;
    }

    public class ChallengeArr<T> : IChallenge
    {
        public ChallengeArr(object[] args, T[] expectedResult)
        {
            this.Args = EnsureArg.IsNotNull(args, nameof(args));
            this.ExpectedResult = expectedResult;
        }

        public object[] Args { get; }

        public T[] ExpectedResult { get; }

        object IChallenge.ExpectedResult => this.ExpectedResult;
    }
}