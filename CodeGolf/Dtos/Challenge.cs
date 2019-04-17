using System;

namespace CodeGolf.Dtos
{
    public class Challenge<T>
    {
        public Challenge(object[] args, T expectedResult)
        {
            this.Args = args;
            this.ExpectedResult = expectedResult;
        }

        public object[] Args { get; }

        public T ExpectedResult { get; }
    }
}