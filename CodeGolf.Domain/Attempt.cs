using System;
using EnsureThat;

namespace CodeGolf.Domain
{
    public class Attempt
    {
        public Attempt(Guid id, string userId, Guid holeId, string code, int score, DateTime timeStamp)
        {
            this.Id = EnsureArg.IsNotEmpty(id, nameof(id));
            this.UserId = EnsureArg.IsNotEmpty(userId, nameof(userId));
            this.HoleId = EnsureArg.IsNotEmpty(holeId, nameof(holeId));
            this.Code = EnsureArg.IsNotNull(code, nameof(code));
            this.Score = score;
            this.TimeStamp = EnsureArg.IsNotDefault(timeStamp, nameof(timeStamp));
        }

        public Guid Id { get; private set; }

        public string UserId { get; private set; }

        public Guid HoleId { get; private set; }

        public string Code { get; private set; }

        public int Score { get; private set; }

        public DateTime TimeStamp { get; private set; }
    }
}
