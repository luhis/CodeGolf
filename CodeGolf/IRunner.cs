using System;
using System.Collections.Generic;
using Optional;

namespace CodeGolf
{
    public interface IRunner
    {
        Option<Func<object[], Option<T, IReadOnlyList<string>>>, IReadOnlyList<string>> Compile<T>(string function, IReadOnlyList<Type> paramTypes);
    }
}