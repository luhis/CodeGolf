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
        public ActionResult<string> Preview(string code)
        {
            return this.codeGolfService.WrapCode(code);
        }
    }
}