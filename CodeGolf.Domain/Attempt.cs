using System;
using EnsureThat;

namespace CodeGolf.Domain
{
    public class Attempt
    {
        public Attempt(Guid userId, Guid holeId, string code, int score)
        {
            this.UserId = EnsureArg.IsNotEmpty(userId, nameof(userId));
            this.HoleId = EnsureArg.IsNotEmpty(holeId, nameof(holeId));
            this.Code = EnsureArg.IsNotNull(code, nameof(code));
            this.Score = score;
        }

        public Guid UserId { get; }

        public Guid HoleId { get; }

        public string Code { get; }

        public int Score { get; }
    }
}
