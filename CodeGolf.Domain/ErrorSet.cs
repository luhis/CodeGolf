namespace CodeGolf.Domain
{
    using System.Collections.Generic;
    using EnsureThat;

    public class ErrorSet
    {
        public ErrorSet(IReadOnlyList<string> errors)
        {
            this.Errors = EnsureArg.IsNotNull(errors, nameof(errors));
        }

        public ErrorSet(params string[] s)
        {
            this.Errors = s;
        }

        public IReadOnlyList<string> Errors { get; }
    }
}