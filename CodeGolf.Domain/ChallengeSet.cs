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

        async Task<IReadOnlyList<ChallengeResult>> IChallengeSet.GetResults(
            Func<object[], Task<Option<object, string>>> t)
        {
            return (await Task.WhenAll(this.Challenges.Select(async challenge =>
            {
                var r = await t(challenge.Args);
                var errors = r.Match(success =>
                {
                    var res = (T) success;
                    if (!AreEqual(challenge.ExpectedResult, res))
                    {
                        return Option.Some(
                            $"Return value incorrect. Expected: {GenericPresentationHelpers.WrapIfArray(challenge.ExpectedResult, typeof(T))}, Found: {GenericPresentationHelpers.WrapIfArray(res, typeof(T))}");
                    }

                    return Option.None<string>();
                }, Option.Some);
                return new ChallengeResult(errors, challenge);
            }))).ToList();
        }

        private static bool AreEqual(object expect, object actual)
        {
            if (typeof(T).IsArray)
            {
                if (actual == null)
                {
                    return false;
                }
                if (expect is int[] intArr)
                {
                    return intArr.Zip((int[]) actual, Tuple.Create).All(t => t.Item1 == t.Item2);
                }
                else
                {
                    return ((string[])expect).Zip((string[])actual, Tuple.Create).All(t => t.Item1 == t.Item2);
                }
            }
            else
            {
                return expect.Equals(actual);
            }
        }
    }
}
