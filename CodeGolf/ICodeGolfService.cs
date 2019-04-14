using System;
using System.Collections.Generic;

namespace CodeGolf
{
    public interface ICodeGolfService
    {
        OneOf.OneOf<int, IReadOnlyList<string>> Score<T>(string code, Func<T, bool> validator);
    }
}