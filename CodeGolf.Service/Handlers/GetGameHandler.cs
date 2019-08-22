namespace CodeGolf.Service.Handlers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using CodeGolf.Domain.Repositories;
    using MediatR;
    using Optional;

    public class GetGameHandler : IRequestHandler<GetGame, Option<Guid>>
    {
        private readonly IGameRepository gameRepository;

        public GetGameHandler(IGameRepository gameRepository)
        {
            this.gameRepository = gameRepository;
        }

        Task<Option<Guid>> IRequestHandler<GetGame, Option<Guid>>.Handle(GetGame request, CancellationToken cancellationToken)
        {
            var game = this.gameRepository.GetByAccessKey(request.AccessKey, cancellationToken);
            return Task.FromResult(game.Map(a => a.Id));
        }
    }
}
