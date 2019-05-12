using System;

namespace CodeGolf.Domain
{
    public class User
    {
        public User(Guid userId, string name)
        {
            this.UserId = userId;
            this.Name = name;
        }

        public Guid UserId { get; }

        public string Name { get; }
    }
}