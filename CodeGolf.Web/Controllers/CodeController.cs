using System.Threading;
using CodeGolf.Service;
using Microsoft.AspNetCore.Mvc;

namespace CodeGolf.Web.Controllers
{
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
            return this.codeGolfService.WrapCode(code, cancellationToken);
        }

        [HttpGet("[action]")]
        public ActionResult<string> Debug(string code, CancellationToken cancellationToken)
        {
            return this.codeGolfService.DebugCode(code, cancellationToken);
        }
    }
}