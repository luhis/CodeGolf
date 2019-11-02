namespace CodeGolf.Domain
{
    public class ChallengeResult
    {
        public ChallengeResult(Error error)
        {
            this.Error = error;
        }

        public Error Error { get; }
    }
}
