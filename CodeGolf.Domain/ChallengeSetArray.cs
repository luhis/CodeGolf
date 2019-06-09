using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnsureThat;
using Optional;

namespace CodeGolf.Domain
{
    public class ChallengeSetArray<T> : IChallengeSet
    {
        public ChallengeSetArray(string title, string description, IReadOnlyList<Type> ps,
            IReadOnlyList<ChallengeArray<T>> challenges)
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

        private void ValidateParameters(IChallenge challenge)
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

        Type IChallengeSet.ReturnType => typeof(T[]);

        private IReadOnlyList<ChallengeArray<T>> Challenges { get; }

        IReadOnlyList<IChallenge> IChallengeSet.Challenges => this.Challenges;


        async Task<IReadOnlyList<ChallengeResult>> IChallengeSet.GetResults(
            CompileResult t)
        {
            return (await Task.WhenAll(this.Challenges.Select(async challenge =>
            {
                var r = await t.Func(challenge.Args);
                var errors = r.Match(success =>
                {
                    var x = success;
                    var res = x != null ? (T[])x : null;
                    if (!AreEqual(challenge.ExpectedResult, res))
                    {
                        return Option.Some(
                            $"Return value incorrect. Expected: {GenericPresentationHelpers.WrapIfArray(challenge.ExpectedResult, typeof(T[]))}, Found: {GenericPresentationHelpers.WrapIfArray(res, typeof(T[]))}");
                    }

                    return Option.None<string>();
                }, Option.Some);
                return new ChallengeResult(errors, challenge);
            }))).ToList();
        }

        private static bool AreEqual(T[] expect, T[] actual)
        {
            if (actual == null)
            {
                return false;
            }

            if (expect.Length != actual.Length)
            {
                return false;
            }

            return expect.Zip(actual, Tuple.Create).All(t => string.Equals(t.Item1, t.Item2));
        }
    }
}