namespace CodeGolf.Service.Dtos
{
    using EnsureThat;

    public class ResultDto
    {
        public ResultDto(string loginName, int score)
        {
            this.LoginName = EnsureArg.IsNotEmpty(loginName, nameof(loginName));
            this.Score = score;
        }

        public string LoginName { get; }

        public int Score { get; }
    }
}