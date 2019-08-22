namespace CodeGolf.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CodeGolf.Service;
    using CodeGolf.Service.Dtos;
    using CodeGolf.Service.Handlers;
    using CodeGolf.Web.Attributes;
    using CodeGolf.Web.Mappers;
    using CodeGolf.Web.Models;
    using CodeGolf.Web.Tooling;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Optional.Unsafe;
    using GameDto = CodeGolf.Service.Dtos.GameDto;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly IDashboardService dashboardService;
        private readonly IMediator mediator;
        private readonly ChallengeSetMapper challengeSetMapper;
        private readonly IAdminService adminService;
        private readonly IIdentityTools identityTools;

        public AdminController(IDashboardService dashboardService, ChallengeSetMapper challengeSetMapper, IAdminService adminService, IMediator mediator, IIdentityTools identityTools)
        {
            this.dashboardService = dashboardService;
            this.challengeSetMapper = challengeSetMapper;
            this.mediator = mediator;
            this.adminService = adminService;
            this.identityTools = identityTools;
        }

        [ServiceFilter(typeof(GameAdminAuthAttribute))]
        [HttpPost("[action]/{gameId}")]
        public async Task EndHole(Guid gameId, CancellationToken cancellationToken)
        {
            var curr = await this.dashboardService.GetCurrentHole(cancellationToken);
            await curr.Match(
                some => this.dashboardService.EndHole(some.Hole.HoleId, cancellationToken),
                () => Task.CompletedTask);
        }

        [ServiceFilter(typeof(GameAdminAuthAttribute))]
        [HttpPost("[action]")]
        public Task NextHole(Guid gameId, CancellationToken cancellationToken)
        {
            return this.dashboardService.NextHole(gameId, cancellationToken);
        }

        [HttpGet("[action]/{holeId}")]
        public async Task<ActionResult<IReadOnlyList<AttemptDto>>> Results(Guid holeId, CancellationToken cancellationToken)
        {
            return (await this.dashboardService.GetAttempts(holeId, cancellationToken)).Match(some => new ActionResult<IReadOnlyList<AttemptDto>>(some), () => this.NotFound());
        }

        [HttpGet("[action]/{gameId}")]
        public async Task<ActionResult<Models.HoleDto>> CurrentHole(Guid gameId, CancellationToken cancellationToken)
        {
            return (await this.dashboardService.GetCurrentHole(cancellationToken)).Match<ActionResult<Models.HoleDto>>(
                some => new Models.HoleDto(some.Hole, some.Start, some.End, some.ClosedAt, some.HasNext, this.challengeSetMapper.Map(some.ChallengeSet)),
                () => null);
        }

        [ServiceFilter(typeof(GameAdminAuthAttribute))]
        [HttpGet("[action]/{gameId}")]
        public async Task<ActionResult<IReadOnlyList<ResultDto>>> FinalScores(Guid gameId, CancellationToken cancellationToken)
        {
            var r = await this.mediator.Send(new FinalScores(gameId), cancellationToken);
            return new ActionResult<IReadOnlyList<ResultDto>>(r);
        }

        [ServiceFilter(typeof(GameAdminAuthAttribute))]
        [HttpGet("[action]/{attemptId}")]
        public async Task<ActionResult<AttemptCodeDto>> Attempt(Guid attemptId, CancellationToken cancellationToken)
        {
            var res = await this.dashboardService.GetAttemptById(attemptId, cancellationToken);
            return res.Match(
                some => new ActionResult<AttemptCodeDto>(some),
                () => this.NotFound());
        }

        [ServiceFilter(typeof(GameAdminAuthAttribute))]
        [HttpGet("[action]")]
        public async Task<ActionResult<IReadOnlyList<GameDto>>> MyGames(CancellationToken cancellationToken)
        {
            var user = this.identityTools.GetIdentity(this.HttpContext).ValueOrFailure();
            var r = await this.adminService.GetAllGames(user, cancellationToken);
            return new ActionResult<IReadOnlyList<GameDto>>(r);
        }

        [ServiceFilter(typeof(GameAdminAuthAttribute))]
        [HttpPost("[action]")]
        public Task CreateGame(GameDto challenges, string accessKey, CancellationToken cancellationToken)
        {
            var user = this.identityTools.GetIdentity(this.HttpContext).ValueOrFailure();
            return this.adminService.CreateGame(challenges, accessKey, user, cancellationToken);
        }

        [ServiceFilter(typeof(GameAdminAuthAttribute))]
        [HttpGet("[action]")]
        public async Task<IReadOnlyList<ChallengeOverView>> AllChallenges(CancellationToken cancellationToken)
        {
            return (await this.adminService.GetAllChallenges(cancellationToken)).Select(a => new ChallengeOverView(a.Id, a.Title, a.Description)).ToArray();
        }
    }
}
