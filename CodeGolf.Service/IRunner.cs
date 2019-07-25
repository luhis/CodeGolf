using System;
using System.Collections.Generic;
using System.Threading;
using CodeGolf.Domain;
using Optional;

namespace CodeGolf.Service
{
    using CodeGolf.Service.Dtos;

    public interface IRunner
    {
        Option<CompileRunner, IReadOnlyList<CompileErrorMessage>> Compile(string function, IReadOnlyList<Type> paramTypes, Type returnType, CancellationToken cancellationToken);

        Option<IReadOnlyList<CompileErrorMessage>> TryCompile(string function, IReadOnlyList<Type> paramTypes, Type returnType, CancellationToken cancellationToken);

        string Wrap(string function, CancellationToken cancellationToken);

        string DebugCode(string function, CancellationToken cancellationToken);

        void WakeUpCompiler(CancellationToken cancellationToken);
    }
}