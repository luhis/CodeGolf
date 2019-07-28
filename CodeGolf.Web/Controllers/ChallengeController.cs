namespace CodeGolf.Web.Controllers
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using CodeGolf.Service;
    using CodeGolf.Service.Dtos;
    using CodeGolf.Web.Mappers;
    using CodeGolf.Web.Models;

    using Microsoft.AspNetCore.Mvc;

    using OneOf;

    using ChallengeResult = CodeGolf.Domain.ChallengeResult;

    [Route("api/[controller]")]
    [ApiController]
    public class ChallengeController : ControllerBase
    {
        private readonly ICodeGolfService codeGolfService;
        private readonly ChallengeSetMapper challengeSetMapper;

        public ChallengeController(ICodeGolfService codeGolfService, ChallengeSetMapper challengeSetMapper)
        {
            this.codeGolfService = codeGolfService;
            this.challengeSetMapper = challengeSetMapper;
        }

        [HttpGet("[action]")]
        public ChallengeSetDto DemoChallenge()
        {
            return this.challengeSetMapper.Map(this.codeGolfService.GetDemoChallenge());
        }

        [HttpPost("[action]")]
        public Task<OneOf<int, IReadOnlyList<ChallengeResult>, IReadOnlyList<CompileErrorMessage>>> SubmitDemo(string code, CancellationToken cancellationToken)
        {
            var demo = this.codeGolfService.GetDemoChallenge();
            return this.codeGolfService.Score(code ?? string.Empty, demo, cancellationToken);
        }
    }
}