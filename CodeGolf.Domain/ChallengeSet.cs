using System;
using System.Collections.Generic;
using System.Linq;
using EnsureThat;

namespace CodeGolf.Domain
{
    public class ChallengeSet<T>
    {
        public ChallengeSet(string title, string description, IReadOnlyList<Type> ps, IReadOnlyList<Challenge<T>> challenges)
        {
            this.Title = EnsureArg.IsNotNull(title, nameof(title));
            this.Description = EnsureArg.IsNotNull(description, nameof(description));
            this.Params = EnsureArg.IsNotNull(ps, nameof(ps));
            this.Challenges = EnsureArg.IsNotNull(challenges, nameof(challenges));
            foreach (var challenge in challenges)
            {
                this.ValidateParameters(challenge);
            }
        }

        private static bool IsMisMatched(ValueTuple<object, Type> t) => t.Item1.GetType() != t.Item2;

        private void ValidateParameters(Challenge<T> challenge)
        {
            if (challenge.Args.Length != this.Params.Count)
            {
                throw new Exception("Incorrect number of parameters");
            }

            var misMatched = challenge.Args.Zip(this.Params, ValueTuple.Create).Where(IsMisMatched);
            if (misMatched.Any())
            {
                throw new Exception("Mismatched parameters");
            }
        }

        public string Title { get; }

        public string Description { get; }

        public IReadOnlyList<Type> Params { get; }

        public IReadOnlyList<Challenge<T>> Challenges { get; }

        public IReadOnlyList<Tuple<bool, Challenge<T>>> GetResult(Func<object[],T> t)
        {
            return this.Challenges.Select(a => Tuple.Create(t(a.Args).Equals(a.ExpectedResult), a)).ToList();
        }
    }
}