using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeGolf.Domain.ChallengeInterfaces
{
    public interface IChallengeSet
    {
        Guid Id { get; }

        Task<IReadOnlyList<ChallengeResult>> GetResults(CompileResult t);

        IReadOnlyList<ParamDescription> Params { get; }

        Type ReturnType { get; }

        string Title { get; }

        string Description { get; }

        IReadOnlyList<IChallenge> Challenges { get; }
    }
}