namespace CodeGolf.Web.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using CodeGolf.Service;
    using CodeGolf.Web.Attributes;
    using CodeGolf.Web.Mappers;
    using CodeGolf.Web.Models;
    using CodeGolf.Web.Tooling;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using Optional.Unsafe;

    [Route("api/[controller]")]
    [ApiController]
    public class ChallengeController : ControllerBase
    {
        private readonly ICodeGolfService codeGolfService;
        private readonly IGameService gameService;
        private readonly ChallengeSetMapper challengeSetMapper;
        private readonly IIdentityTools identityTools;

        public ChallengeController(ICodeGolfService codeGolfService, ChallengeSetMapper challengeSetMapper, IGameService gameService, IIdentityTools identityTools)
        {
            this.codeGolfService = codeGolfService;
            this.challengeSetMapper = challengeSetMapper;
            this.gameService = gameService;
            this.identityTools = identityTools;
        }

        [HttpGet("[action]")]
        public ChallengeSetDto DemoChallenge()
        {
            return this.challengeSetMapper.Map(this.codeGolfService.GetDemoChallenge());
        }

        [Authorize]
        [HttpGet("[action]")]
        public async Task<ActionResult<HoleDto>> CurrentChallenge(CancellationToken cancellationToken)
        {
            return (await this.gameService.GetCurrentHole(cancellationToken))
                .Match<ActionResult<HoleDto>>(some => new HoleDto(some.Hole, some.Start, some.End, some.ClosedAt, some.HasNext, this.challengeSetMapper.Map(some.ChallengeSet)), () => (HoleDto)null);
        }

        [HttpPost("[action]")]
        [ServiceFilter(typeof(RecaptchaAttribute))]
        public async Task<SubmissionResult> SubmitDemo([FromBody]string code, CancellationToken cancellationToken)
        {
            var demo = this.codeGolfService.GetDemoChallenge();
            return SubmissionResultMapper.Map(await this.codeGolfService.Score(code ?? string.Empty, demo, cancellationToken));
        }

        [HttpPost("[action]/{holeId}")]
        public async Task<SubmissionResult> SubmitChallenge([FromBody]string code, Guid holeId, CancellationToken cancellationToken)
        {
            var hole = await this.gameService.GetCurrentHole(cancellationToken);
            var res = await this.gameService.Attempt(
                          this.identityTools.GetIdentity(this.HttpContext).ValueOrFailure(),
                          holeId,
                          code,
                          hole.ValueOrFailure().ChallengeSet,
                          cancellationToken).ConfigureAwait(false);
            return SubmissionResultMapper.Map(res);
        }
    }
}