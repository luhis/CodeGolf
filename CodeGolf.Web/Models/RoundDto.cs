namespace CodeGolf.Web.Models
{
    using System;

    public class RoundDto
    {
        public RoundDto(Guid id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public Guid Id { get; }

        public string Name { get; }
    }
}
