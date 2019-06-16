using CodeGolf.Domain.ChallengeInterfaces;
using Optional;

namespace CodeGolf.Domain
{
    public class ChallengeResult
    {
        public ChallengeResult(Option<string> error, IChallenge challenge)
        {
            this.Error = error;
            this.Challenge = challenge;
        }

        public Option<string> Error { get; }

        public IChallenge Challenge { get; }
    }
}