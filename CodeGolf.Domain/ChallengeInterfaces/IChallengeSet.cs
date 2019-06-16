using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeGolf.Domain.ChallengeInterfaces
{
    public interface IChallengeSet
    {
        Task<IReadOnlyList<ChallengeResult>> GetResults(CompileResult t);

        IReadOnlyList<Type> Params { get; }

        Type ReturnType { get; }

        string Title { get; }

        string Description { get; }

        IReadOnlyList<IChallenge> Challenges { get; }
    }
}