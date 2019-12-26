namespace CodeGolf.Domain
{
    using System.Collections.Generic;
    using EnsureThat;

    public class ErrorSet
    {
        public ErrorSet(params string[] s)
        {
            this.Errors = EnsureArg.IsNotNull(s, nameof(s));
        }

        public IReadOnlyList<string> Errors { get; }
    }
}
