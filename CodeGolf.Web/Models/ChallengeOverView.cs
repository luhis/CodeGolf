namespace CodeGolf.Web.Models
{
    using System;

    public class ChallengeOverView
    {
        public ChallengeOverView(Guid id, string title, string description)
        {
            this.Id = id;
            this.Title = title;
            this.Description = description;
        }

        public Guid Id { get; }

        public string Title { get; }

        public string Description { get; }
    }
}
