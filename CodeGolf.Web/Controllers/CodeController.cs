﻿using System.Threading;

using CodeGolf.Service;

using Microsoft.AspNetCore.Mvc;

namespace CodeGolf.Web.Controllers
{
    using System;
    using System.Linq;

    using CodeGolf.Domain;
    using CodeGolf.Web.Models;
    using CodeGolf.Web.Tooling;

    using Optional.Unsafe;

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

        [HttpPost("[action]/{id}")]
        public JsonResult TryCompile(Guid id, [FromBody] string code, CancellationToken cancellationToken)
        {
            return new JsonResult(
                this.codeGolfService.TryCompile(id, code ?? string.Empty, cancellationToken).Match(
                    a => a.Where(x => true).Select(
                        b => new ErrorItem(b.Line, b.Col, b.EndCol)).ToArray(),
                    () => new ErrorItem[0]));
        }
    }
}