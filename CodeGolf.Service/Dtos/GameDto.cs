namespace CodeGolf.Service.Dtos
{
    using System;
    using System.Collections.Generic;

    public class GameDto
    {
        public GameDto(Guid id, string accessKey, IReadOnlyList<RoundDto> rounds)
        {
            this.Id = id;
            this.AccessKey = accessKey;
            this.Rounds = rounds;
        }

        public Guid Id { get; }

        public string AccessKey { get; }

        public IReadOnlyList<RoundDto> Rounds { get; }
    }
}
