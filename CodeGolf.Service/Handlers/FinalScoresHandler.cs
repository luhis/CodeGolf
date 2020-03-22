namespace CodeGolf.Service.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CodeGolf.Domain.Repositories;
    using CodeGolf.Service.Dtos;
    using MediatR;

    public class FinalScoresHandler : IRequestHandler<FinalScores, IReadOnlyList<ResultDto>>
    {
        private readonly IGameRepository gameRepository;

        private readonly IUserRepository userRepository;

        private readonly IBestAttemptsService bestAttemptsService;

        public FinalScoresHandler(IGameRepository gameRepository, IUserRepository userRepository, IBestAttemptsService bestAttemptsService)
        {
            this.gameRepository = gameRepository;
            this.userRepository = userRepository;
            this.bestAttemptsService = bestAttemptsService;
        }

        async Task<IReadOnlyList<ResultDto>> IRequestHandler<FinalScores, IReadOnlyList<ResultDto>>.Handle(FinalScores request, CancellationToken cancellationToken)
        {
            var holes = await Task.WhenAll(
                            this.gameRepository.GetGame(cancellationToken).Holes.Select(
                                async h => (await this.bestAttemptsService.GetBestAttempts(h.HoleId, cancellationToken))
                                    .Select((a, b) => ValueTuple.Create(b, a))));
            var ranks = holes.SelectMany(a => a);
            return (await Task.WhenAll(
                        ranks.GroupBy(a => a.Item2.UserId).Select(
                            async (r, i) =>
                                {
                                    var user = await this.userRepository.GetByUserId(r.Key, cancellationToken);
                                    return new ResultDto(
                                        i + 1,
                                        user.Map(a => a.LoginName).ValueOr(string.Empty),
                                        user.Match(a => a.AvatarUri.ToString(), () => string.Empty),
                                        r.Sum(a => PosToPoints(a.Item1)));
                                }))).ToList();
        }

        private static int PosToPoints(int i)
        {
            switch (i)
            {
                case 0:
                    return 3;
                case 1:
                    return 2;
                case 2:
                    return 1;
                default:
                    return 0;
            }
        }
    }
}
