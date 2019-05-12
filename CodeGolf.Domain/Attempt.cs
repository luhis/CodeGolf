using System;
using EnsureThat;

namespace CodeGolf.Domain
{
    public class Attempt
    {
        public Attempt(Guid userId, Guid roundId, string code, int score)
        {
            this.UserId = EnsureArg.IsNotEmpty(userId, nameof(userId));
            this.RoundId = EnsureArg.IsNotEmpty(roundId, nameof(roundId));
            this.Code = EnsureArg.IsNotNull(code, nameof(code));
            this.Score = score;
        }

        public Guid UserId { get; }

        public Guid RoundId { get; }

        public string Code { get; }

        public int Score { get; }
    }
}
