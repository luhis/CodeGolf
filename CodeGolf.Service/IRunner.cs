using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CodeGolf.Domain;
using Optional;

namespace CodeGolf.Service
{
    public interface IRunner
    {
        Option<Func<object[], Task<Option<object, string>>>, ErrorSet> Compile(string function, IReadOnlyList<Type> paramTypes, Type returnType, CancellationToken cancellationToken);

        string Wrap(string function, CancellationToken cancellationToken);

        string DebugCode(string function, CancellationToken cancellationToken);

        void WakeUpCompiler(CancellationToken cancellationToken);
    }
}