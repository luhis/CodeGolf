using EnsureThat;

namespace CodeGolf.Service.Dtos
{
    public class Challenge<T>
    {
        public Challenge(object[] args, T expectedResult)
        {
            this.Args = EnsureArg.IsNotNull(args, nameof(args));
            this.ExpectedResult = expectedResult;
        }

        public object[] Args { get; }

        public T ExpectedResult { get; }
    }
}