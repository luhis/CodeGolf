namespace CodeGolf.Domain
{
    using CodeGolf.Domain.ChallengeInterfaces;

    public class ChallengeResult
    {
        public ChallengeResult(Error error, IChallenge challenge)
        {
            this.Error = error;
            this.Challenge = challenge;
        }

        public Error Error { get; }

        public IChallenge Challenge { get; }
    }
}
