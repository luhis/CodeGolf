using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnsureThat;
using Optional;

namespace CodeGolf.Domain
{
    public class ChallengeSet<T> : IChallengeSet
    {
        public ChallengeSet(string title, string description, IReadOnlyList<Type> ps,
            IReadOnlyList<Challenge<T>> challenges)
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

        Type IChallengeSet.ReturnType => typeof(T);

        private IReadOnlyList<Challenge<T>> Challenges { get; }

        IReadOnlyList<IChallenge> IChallengeSet.Challenges => this.Challenges;

        async Task<IReadOnlyList<Tuple<Option<IReadOnlyList<string>>, IChallenge>>> IChallengeSet.GetResults(
            Func<IChallenge, Task<Option<object, ErrorSet>>> t)
        {
            return (await Task.WhenAll(this.Challenges.Select(async a =>
            {
                var r = await t(a);
                var errors = r.Match(success =>
                {
                    var res = (T) success;
                    if (!res.Equals(a.ExpectedResult))
                    {
                        return Option.Some<IReadOnlyList<string>>(new List<string>
                            {$"Return value incorrect. Expected: {a.ExpectedResult}, Found: {res}"});
                    }

                    return Option.None<IReadOnlyList<string>>();
                }, err => Option.Some(err.Errors));
                return Tuple.Create(errors, (IChallenge) a);
            }))).ToList();
        }
    }
}
