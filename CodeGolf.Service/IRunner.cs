using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeGolf.Service.Dtos;
using Optional;

namespace CodeGolf.Service
{
    public interface IRunner
    {
        Option<Func<object[], Task<Option<T, ErrorSet>>>, ErrorSet> Compile<T>(string function, IReadOnlyList<Type> paramTypes);

        string Wrap(string function);

        string DebugCode(string function);
    }
}