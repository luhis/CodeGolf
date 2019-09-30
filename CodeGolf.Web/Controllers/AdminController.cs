namespace CodeGolf.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CodeGolf.Domain.ChallengeInterfaces;
    using CodeGolf.Service;
    using CodeGolf.Service.Dtos;
    using CodeGolf.Web.Attributes;
    using CodeGolf.Web.Mappers;
    using CodeGolf.Web.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using GameDto = CodeGolf.Web.Models.GameDto;
    using RoundDto = CodeGolf.Web.Models.RoundDto;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(GameAdminAuthAttribute))]
    public class AdminController : ControllerBase
    {
        private readonly IDashboardService dashboardService;
        private readonly IResultsService resultsService;
        private readonly ChallengeSetMapper challengeSetMapper;
        private readonly IAdminService adminService;

        public AdminController(IDashboardService dashboardService, ChallengeSetMapper challengeSetMapper, IResultsService resultsService, IAdminService adminService)
        {
            this.dashboardService = dashboardService;
            this.challengeSetMapper = challengeSetMapper;
            this.resultsService = resultsService;
            this.adminService = adminService;
        }

        [HttpPost("[action]")]
        public async Task EndHole(CancellationToken cancellationToken)
        {
            var curr = await this.dashboardService.GetCurrentHole(cancellationToken);
            await curr.Match(
                some => this.dashboardService.EndHole(some.Hole.HoleId),
                () => Task.CompletedTask);
        }

        [HttpPost("[action]")]
        public Task NextHole(CancellationToken cancellationToken)
        {
            return this.dashboardService.NextHole(cancellationToken);
        }

        [HttpGet("[action]/{holeId}")]
        public async Task<ActionResult<IReadOnlyList<AttemptDto>>> Results(Guid holeId, CancellationToken cancellationToken)
        {
            return (await this.dashboardService.GetAttempts(holeId, cancellationToken)).Match(some => new ActionResult<IReadOnlyList<AttemptDto>>(some), () => this.NotFound());
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<Models.HoleDto>> CurrentHole(CancellationToken cancellationToken)
        {
            return (await this.dashboardService.GetCurrentHole(cancellationToken)).Match<ActionResult<Models.HoleDto>>(
                some => new Models.HoleDto(some.Hole, some.Start, some.End, some.ClosedAt, some.HasNext, this.challengeSetMapper.Map(some.ChallengeSet)),
                () => null);
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<IReadOnlyList<ResultDto>>> FinalScores(CancellationToken cancellationToken)
        {
            var r = await this.resultsService.GetFinalScores(cancellationToken);
            return new ActionResult<IReadOnlyList<ResultDto>>(r);
        }

        [HttpGet("[action]/{attemptId}")]
        public async Task<ActionResult<AttemptCodeDto>> Attempt(Guid attemptId, CancellationToken cancellationToken)
        {
            var res = await this.dashboardService.GetAttemptById(attemptId, cancellationToken);
            return res.Match(
                some => new ActionResult<AttemptCodeDto>(some),
                () => this.NotFound());
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<IReadOnlyList<GameDto>>> MyGames(CancellationToken cancellationToken)
        {
            // var user = this.identityTools.GetIdentity(this.HttpContext).ValueOrFailure();
            var r = await this.adminService.GetAllGames(cancellationToken);
            var x = r.Select(a =>
                new GameDto(
                    Guid.NewGuid(),
                    "aa",
                    a.Holes.Select(h => new RoundDto(h.HoleId, "bb")).ToList())).ToList();
            return new ActionResult<IReadOnlyList<GameDto>>(x);
        }

        [HttpPost("[action]/{gameId}")]
        public Task Reset(CancellationToken cancellationToken)
        {
            return this.adminService.ResetGame();
        }

        [HttpGet("[action]")]
        public async Task<IReadOnlyList<ChallengeOverView>> AllChallenges(CancellationToken cancellationToken)
        {
            return (await this.adminService.GetAllChallenges(cancellationToken)).Select(a => new ChallengeOverView(a.Id, a.Title, a.Description)).ToArray();
        }
    }
}
