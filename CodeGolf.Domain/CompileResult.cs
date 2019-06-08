using System;
using System.Threading.Tasks;
using EnsureThat;
using OneOf;
using Optional;

namespace CodeGolf.Domain
{
    public class CompileResult
    {
        public CompileResult(Func<object[], Task<Option<OneOf<object, object[]>, string>>> func)
        {
            this.Func = EnsureArg.IsNotNull(func, nameof(func));
        }

        public Func<object[], Task<Option<OneOf<object, object[]>, string>>> Func { get; }
    }
}