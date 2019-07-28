namespace CodeGolf.Web.Models
{
    using System;
    using System.Collections.Generic;

    public class ChallengeSetDto
    {
        public ChallengeSetDto(Guid id, string title, string description, string returnType, IReadOnlyList<ParamsDescriptionDto> @params, IReadOnlyList<ChallengeDto> challenges)
        {
            this.Id = id;
            this.Title = title;
            this.Description = description;
            this.ReturnType = returnType;
            this.Params = @params;
            this.Challenges = challenges;
        }

        public Guid Id { get; }

        public string Title { get; }

        public string Description { get; }

        public string ReturnType { get; }

        public IReadOnlyList<ParamsDescriptionDto> Params { get; }

        public IReadOnlyList<ChallengeDto> Challenges { get; }
    }
}