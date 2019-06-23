using System;
using EnsureThat;

namespace CodeGolf.Service.Dtos
{
    public class AttemptDto
    {
        public AttemptDto(int rank, Guid id, string loginName, string avatar, int score, DateTime timeStamp)
        {
            this.Rank = rank;
            this.Id = EnsureArg.IsNotEmpty(id, nameof(id));
            this.LoginName = EnsureArg.IsNotEmpty(loginName, nameof(loginName));
            this.Score = score;
            this.Avatar = EnsureArg.IsNotEmpty(avatar, nameof(avatar));
            this.TimeStamp = EnsureArg.IsNotDefault(timeStamp, nameof(timeStamp));
        }

        public int Rank { get; }

        public Guid Id { get; }

        public string LoginName { get; }

        public string Avatar { get; }

        public int Score { get; }

        public DateTime TimeStamp { get; }
    }
}