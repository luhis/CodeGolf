using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CodeGolf.Domain;
using CodeGolf.Service;
using CodeGolf.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Optional.Linq;

namespace CodeGolf.Web.Controllers
{
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
        public async Task<ActionResult<IReadOnlyList<Attempt>>> Results(CancellationToken cancellationToken)
        {
            var r = (await this.gameService.GetCurrentHole(cancellationToken)).Select(a => a.Attempts);
            return r.Match(some => new ActionResult<IReadOnlyList<Attempt>>(some), () => this.NotFound());
        }
    }
}