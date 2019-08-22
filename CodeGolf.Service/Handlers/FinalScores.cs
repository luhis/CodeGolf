namespace CodeGolf.Service.Handlers
{
    using System;
    using System.Collections.Generic;
    using CodeGolf.Service.Dtos;
    using MediatR;

    public class FinalScores : IRequest<IReadOnlyList<ResultDto>>
    {
        public FinalScores(Guid gameId)
        {
            this.GameId = gameId;
        }

        public Guid GameId { get; }
    }
}
