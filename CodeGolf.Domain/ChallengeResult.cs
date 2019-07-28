using CodeGolf.Domain.ChallengeInterfaces;
using Optional;

namespace CodeGolf.Domain
{
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