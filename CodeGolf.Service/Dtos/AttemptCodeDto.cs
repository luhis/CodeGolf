namespace CodeGolf.Service.Dtos
{
    using System;

    using EnsureThat;

    public class AttemptCodeDto
    {
        public AttemptCodeDto(Guid id, string loginName, string avatar, int score, string timeStamp, string code)
        {
            this.Id = EnsureArg.IsNotEmpty(id, nameof(id));
            this.LoginName = EnsureArg.IsNotEmpty(loginName, nameof(loginName));
            this.Score = score;
            this.Code = EnsureArg.IsNotNull(code, nameof(code));
            this.Avatar = EnsureArg.IsNotEmpty(avatar, nameof(avatar));
            this.TimeStamp = EnsureArg.IsNotNull(timeStamp, nameof(timeStamp));
        }

        public Guid Id { get; }

        public string LoginName { get; }

        public string Avatar { get; }

        public int Score { get; }

        public string Code { get; }

        public string TimeStamp { get; }
    }
}
