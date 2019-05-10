using System;
using EnsureThat;

namespace CodeGolf.Domain
{
    public class Attempt
    {
        public Attempt(Guid userId, string code, int score)
        {
            this.UserId = EnsureArg.IsNotEmpty(userId, nameof(userId));
            this.Code = EnsureArg.IsNotNull(code, nameof(code));
            this.Score = score;
        }

        public Guid UserId { get; }

        public string Code { get; }

        public int Score { get; }
    }
}
