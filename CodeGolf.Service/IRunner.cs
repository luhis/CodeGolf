using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CodeGolf.Service.Dtos;
using Optional;

namespace CodeGolf.Service
{
    public interface IRunner
    {
        Option<Func<object[], Task<Option<T, ErrorSet>>>, ErrorSet> Compile<T>(string function, IReadOnlyList<Type> paramTypes, CancellationToken cancellationToken);

        string Wrap(string function, CancellationToken cancellationToken);

        string DebugCode(string function, CancellationToken cancellationToken);

        void WakeUpCompiler(CancellationToken cancellationToken);
    }
}