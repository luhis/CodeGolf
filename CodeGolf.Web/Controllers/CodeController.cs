using System;
using System.Threading;
using System.Threading.Tasks;
using CodeGolf.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeGolf.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CodeController : ControllerBase
    {
        private readonly ICodeGolfService codeGolfService;
        private readonly IGameService gameService;

        public CodeController(ICodeGolfService codeGolfService, IGameService gameService)
        {
            this.codeGolfService = codeGolfService;
            this.gameService = gameService;
        }

        [HttpGet("[action]")]
        public ActionResult<string> Preview(string code, CancellationToken cancellationToken)
        {
            return this.codeGolfService.WrapCode(code ?? string.Empty, cancellationToken);
        }

        [HttpGet("[action]")]
        public ActionResult<string> Debug(string code, CancellationToken cancellationToken)
        {
            return this.codeGolfService.DebugCode(code ?? string.Empty, cancellationToken);
        }

        [Authorize]
        [HttpGet("[action]")]
        public async Task<ActionResult<string>> Attempt(Guid id, CancellationToken cancellationToken)
        {
            var attempt = await this.gameService.GetAttemptById(id, cancellationToken);
            return attempt.Code;
        }
    }
}