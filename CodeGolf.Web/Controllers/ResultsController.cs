using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CodeGolf.Service;
using CodeGolf.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeGolf.Web.Controllers
{
    using CodeGolf.Service.Dtos;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(GameAdminAuthAttribute))]
    public class ResultsController : ControllerBase
    {
        private readonly IGameService gameService;

        public ResultsController(IGameService gameService)
        {
            this.gameService = gameService;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<IReadOnlyList<AttemptDto>>> Results(CancellationToken cancellationToken)
        {
            return (await this.gameService.GetAttempts(cancellationToken)).Match(some => new ActionResult<IReadOnlyList<AttemptDto>>(some), () => this.NotFound());
        }
    }
}