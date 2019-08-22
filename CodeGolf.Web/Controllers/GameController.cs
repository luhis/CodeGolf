namespace CodeGolf.Web.Controllers
{
    using System;
    using System.Threading.Tasks;
    using CodeGolf.Service.Handlers;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IMediator mediator;

        public GameController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<Guid>> GetGameId(string accessKey)
        {
            var g = await this.mediator.Send(new GetGame(accessKey));
            return g.Match<ActionResult<Guid>>(s => s, () => this.NotFound());
        }
    }
}
