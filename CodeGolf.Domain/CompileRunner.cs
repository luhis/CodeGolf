using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnsureThat;
using Optional;

namespace CodeGolf.Domain
{
    public class CompileRunner
    {
        public CompileRunner(Func<object[][], Task<IReadOnlyList<Option<object, string>>>> func)
        {
            this.Func = EnsureArg.IsNotNull(func, nameof(func));
        }

        public Func<object[][], Task<IReadOnlyList<Option<object, string>>>> Func { get; }
    }
}