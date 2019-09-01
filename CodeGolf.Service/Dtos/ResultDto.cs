namespace CodeGolf.Service.Dtos
{
    using EnsureThat;

    public class ResultDto
    {
        public ResultDto(int rank, string loginName, string avatarUri, int score)
        {
            this.Rank = rank;
            this.LoginName = EnsureArg.IsNotEmpty(loginName, nameof(loginName));
            this.Score = score;
            this.AvatarUri = EnsureArg.IsNotNull(avatarUri, nameof(avatarUri));
        }

        public int Rank { get; }

        public string LoginName { get; }

        public string AvatarUri { get; }

        public int Score { get; }
    }
}
