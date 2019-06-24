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
        private readonly IDashboardService dashboardService;

        public CodeController(ICodeGolfService codeGolfService, IDashboardService dashboardService)
        {
            this.codeGolfService = codeGolfService;
            this.dashboardService = dashboardService;
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
    }
}