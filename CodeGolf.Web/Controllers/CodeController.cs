﻿namespace CodeGolf.Web.Controllers
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

        [HttpPost("[action]")]
        public ActionResult<string> Preview(string code, CancellationToken cancellationToken)
        {
            return this.codeGolfService.WrapCode(code ?? string.Empty, cancellationToken);
        }

        [HttpPost("[action]")]
        public ActionResult<string> Debug(string code, CancellationToken cancellationToken)
        {
            return this.codeGolfService.DebugCode(code ?? string.Empty, cancellationToken);
        }

        [HttpPost("[action]")]
        public ActionResult<CompileErrorMessage[]> TryCompile([FromBody] string code, CancellationToken cancellationToken)
        {
            return this.codeGolfService.TryCompile(code ?? string.Empty, cancellationToken).Match(
                    a => a.ToArray(),
                    Array.Empty<CompileErrorMessage>);
        }
    }
}
