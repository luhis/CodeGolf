using System;

namespace CodeGolf.Dtos
{
    public class Challenge<T>
    {
        public Challenge(object[] args, Func<T, bool> validator)
        {
            this.Args = args;
            this.Validator = validator;
        }

        public object[] Args { get; }

        public Func<T, bool> Validator { get; }
    }
}