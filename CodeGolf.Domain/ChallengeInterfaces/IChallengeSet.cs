namespace CodeGolf.Domain.ChallengeInterfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IChallengeSet
    {
        Guid Id { get; }

        IReadOnlyList<ParamDescription> Params { get; }

        Type ReturnType { get; }

        string Title { get; }

        string Description { get; }

        IReadOnlyList<IChallenge> Challenges { get; }

        Task<IReadOnlyList<ChallengeResult>> GetResults(CompileRunner t);
    }
}
