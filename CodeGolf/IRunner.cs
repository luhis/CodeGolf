using System;
using System.Collections.Generic;
using CodeGolf.Dtos;
using Optional;

namespace CodeGolf
{
    public interface IRunner
    {
        Option<Func<object[], Option<T, ErrorSet>>, ErrorSet> Compile<T>(string function, IReadOnlyList<Type> paramTypes);
    }
}