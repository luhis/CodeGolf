using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeGolf.Dtos;
using Optional;

namespace CodeGolf
{
    public interface IRunner
    {
        Option<Func<object[], Task<Option<T, ErrorSet>>>, ErrorSet> Compile<T>(string function, IReadOnlyList<Type> paramTypes);
    }
}