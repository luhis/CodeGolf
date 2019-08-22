namespace CodeGolf.Domain
{
    using System;
    using EnsureThat;

    public class Game
    {
        public Game(string accessKey)
        {
            this.AccessKey = EnsureArg.IsNotNull(accessKey, nameof(accessKey));
        }

        public Guid Id { get; private set; }

        public string AccessKey { get; private set; }

        public int Owner { get; private set; }

        public DateTime Created { get; private set; }
    }
}
