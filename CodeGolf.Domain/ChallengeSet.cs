namespace CodeGolf.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CodeGolf.Domain.ChallengeInterfaces;
    using EnsureThat;

    public class ChallengeSet<T> : IChallengeSet
    {
        public ChallengeSet(
            Guid id,
            string title,
            string description,
            IReadOnlyList<ParamDescription> ps,
            IReadOnlyList<Challenge<T>> challenges)
        {
            this.Id = EnsureArg.IsNotEmpty(id, nameof(id));
            this.Title = EnsureArg.IsNotNull(title, nameof(title));
            this.Description = EnsureArg.IsNotNull(description, nameof(description));
            this.Params = EnsureArg.IsNotNull(ps, nameof(ps));
            this.Challenges = EnsureArg.IsNotNull(challenges, nameof(challenges));
            foreach (var challenge in challenges)
            {
                this.ValidateParameters(challenge);
            }
        }

        public string Title { get; }

        public string Description { get; }

        public IReadOnlyList<ParamDescription> Params { get; }

        Type IChallengeSet.ReturnType => typeof(T);

        IReadOnlyList<IChallenge> IChallengeSet.Challenges => this.Challenges;

        public Guid Id { get; }

        private IReadOnlyList<Challenge<T>> Challenges { get; }

        async Task<IReadOnlyList<ChallengeResult>> IChallengeSet.GetResults(CompileRunner t)
        {
            var reses = await t.Func(this.Challenges.Select(a => a.Args).ToArray());
            var challRes = reses.Zip(this.Challenges, ValueTuple.Create);

            return challRes.Select(
                challenge =>
                    {
                        var errors = challenge.Item1.Match(
                            success =>
                                {
                                    var res = success;
                                    if (!AreEqual(challenge.Item2.ExpectedResult, res))
                                    {
                                        return new Error(
                                            "Return value incorrect.", GenericPresentationHelpers.WrapIfArray(challenge.Item2.ExpectedResult, typeof(T)), GenericPresentationHelpers.WrapIfArray(res, typeof(T)));
                                    }

                                    return null;
                                },
                            s => new Error(s));
                        return new ChallengeResult(errors);
                    }).ToList();
        }

        private static bool AreEqual(object expect, object actual)
        {
            return expect.Equals(actual);
        }

        private static bool IsMisMatched(ValueTuple<object, Type> t) => t.Item1.GetType() != t.Item2;

        private void ValidateParameters(IChallenge challenge)
        {
            if (challenge.Args.Length != this.Params.Count)
            {
                throw new Exception("Incorrect number of parameters");
            }

            var misMatched = challenge.Args.Zip(this.Params.Select(a => a.Type), ValueTuple.Create).Where(IsMisMatched);
            if (misMatched.Any())
            {
                throw new Exception("Mismatched parameters");
            }
        }
    }
}
