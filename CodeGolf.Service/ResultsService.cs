namespace CodeGolf.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using CodeGolf.Domain.Repositories;
    using CodeGolf.Service.Dtos;

    using Optional.Unsafe;

    public class ResultsService : IResultsService
    {
        private readonly IGameRepository gameRepository;

        private readonly IUserRepository userRepository;

        private readonly IBestAttemptsService bestAttemptsService;

        public ResultsService(IGameRepository gameRepository, IUserRepository userRepository, IBestAttemptsService bestAttemptsService)
        {
            this.gameRepository = gameRepository;
            this.userRepository = userRepository;
            this.bestAttemptsService = bestAttemptsService;
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

        async Task<IReadOnlyList<ResultDto>> IResultsService.GetFinalScores(CancellationToken cancellationToken)
        {
            var holes = await Task.WhenAll(
                            this.gameRepository.GetGame().Holes.Select(
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
                                        user.Map(a => a.LoginName).ValueOrDefault(),
                                        user.Match(a => a.AvatarUri, () => string.Empty),
                                        r.Sum(a => PosToPoints(a.Item1)));
                                }))).ToList();
        }
    }
}