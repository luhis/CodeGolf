using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnsureThat;
using Optional;

namespace CodeGolf.Domain
{
    public class CompileResult
    {
        public CompileResult(Func<object[][], Task<IReadOnlyList<Option<object, string>>>> func)
        {
            this.Func = EnsureArg.IsNotNull(func, nameof(func));
        }

        public Func<object[][], Task<IReadOnlyList<Option<object, string>>>> Func { get; }
    }
}