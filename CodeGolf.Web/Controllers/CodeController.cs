namespace CodeGolf.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading;
    using CodeGolf.Service;
    using CodeGolf.Service.Dtos;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class CodeController : ControllerBase
    {
        private readonly ICodeGolfService codeGolfService;

        public CodeController(ICodeGolfService codeGolfService)
        {
            this.codeGolfService = codeGolfService;
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

        [HttpPost("[action]/{challengeId}")]
        public ActionResult<CompileErrorMessage[]> TryCompile(Guid challengeId, [FromBody] string code, CancellationToken cancellationToken)
        {
            return this.codeGolfService.TryCompile(challengeId, code ?? string.Empty, cancellationToken).Match(
                    a => a.ToArray(),
                    Array.Empty<CompileErrorMessage>);
        }
    }
}