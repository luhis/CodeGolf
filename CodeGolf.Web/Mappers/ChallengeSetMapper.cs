namespace CodeGolf.Web.Mappers
{
    using System.Linq;

    using CodeGolf.Domain;
    using CodeGolf.Domain.ChallengeInterfaces;
    using CodeGolf.Web.Models;

    public class ChallengeSetMapper
    {
        public ChallengeSetDto Map(IChallengeSet cs)
        {
            return new ChallengeSetDto(
                cs.Id,
                cs.Title,
                cs.Description,
                GenericPresentationHelpers.GetAlias(cs.ReturnType),
                cs.Params.Select(
                        a => new ParamsDescriptionDto(GenericPresentationHelpers.GetAlias(a.Type), a.SuggestedName))
                    .ToArray(),
                cs.Challenges.Select(
                    a => new ChallengeDto(
                        a.Args.Select(b => b.ToString()).ToArray(),
                        GenericPresentationHelpers.WrapIfArray(a.ExpectedResult, cs.ReturnType))).ToArray());
        }
    }
}
