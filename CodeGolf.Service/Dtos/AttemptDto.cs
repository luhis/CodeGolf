using System;
using EnsureThat;

namespace CodeGolf.Service.Dtos
{
    public class AttemptDto
    {
        public AttemptDto(int rank, Guid id, int userId, string avatar, int score, string timeStamp)
        {
            this.Rank = rank;
            this.Id = EnsureArg.IsNotEmpty(id, nameof(id));
            this.UserId = EnsureArg.IsNotDefault(userId, nameof(userId));
            this.Score = score;
            this.Avatar = EnsureArg.IsNotEmpty(avatar, nameof(avatar));
            this.TimeStamp = EnsureArg.IsNotNull(timeStamp, nameof(timeStamp));
        }

        public int Rank { get; }

        public Guid Id { get; }

        public int UserId { get; }

        public string Avatar { get; }

        public int Score { get; }

        public string TimeStamp { get; }
    }
}