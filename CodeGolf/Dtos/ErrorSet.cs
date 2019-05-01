using System.Collections.Generic;

namespace CodeGolf.Dtos
{
    public class ErrorSet
    {
        public ErrorSet(IReadOnlyList<string> errors)
        {
            this.Errors = errors;
        }

        public ErrorSet(params string[] s)
        {
            this.Errors = s;
        }

        public IReadOnlyList<string> Errors { get; }
    }
}