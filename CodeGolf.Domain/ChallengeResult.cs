namespace CodeGolf.Domain
{
    using CodeGolf.Domain.ChallengeInterfaces;

    public class ChallengeResult
    {
        public ChallengeResult(string error, IChallenge challenge)
        {
            this.Error = error;
            this.Challenge = challenge;
        }

        public string Error { get; }

        public IChallenge Challenge { get; }
    }
}