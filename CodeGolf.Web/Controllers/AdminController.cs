namespace CodeGolf.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using CodeGolf.Service;
    using CodeGolf.Service.Dtos;
    using CodeGolf.Web.Attributes;
    using CodeGolf.Web.Mappers;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(GameAdminAuthAttribute))]
    public class AdminController : ControllerBase
    {
        private readonly IDashboardService dashboardService;
        private readonly IResultsService resultsService;
        private readonly ChallengeSetMapper challengeSetMapper;

        public AdminController(IDashboardService dashboardService, ChallengeSetMapper challengeSetMapper, IResultsService resultsService)
        {
            this.dashboardService = dashboardService;
            this.challengeSetMapper = challengeSetMapper;
            this.resultsService = resultsService;
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
        public async Task NextHole(CancellationToken cancellationToken)
        {
            await this.dashboardService.NextHole(cancellationToken);
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
        public async Task<ActionResult<IReadOnlyList<ResultDto>>> GetFinalScores(CancellationToken cancellationToken)
        {
            var r = await this.resultsService.GetFinalScores(cancellationToken);
            return new ActionResult<IReadOnlyList<ResultDto>>(r);
        }

        [HttpGet("[action]/{attemptId}")]
        public async Task<ActionResult<AttemptCodeDto>> GetAttempt(Guid attemptId, CancellationToken cancellationToken)
        {
            var res = await this.dashboardService.GetAttemptById(attemptId, cancellationToken);
            return res.Match(
                some => new ActionResult<AttemptCodeDto>(some),
                () => this.NotFound());
        }
    }
}