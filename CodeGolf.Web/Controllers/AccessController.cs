namespace CodeGolf.Web.Controllers
{
    using CodeGolf.Web.Attributes;
    using CodeGolf.Web.Tooling;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;

    [Route("api/[controller]")]
    [ApiController]
    public class AccessController : ControllerBase
    {
        private readonly GameAdminSettings gameAdminSettings;
        private readonly IIdentityTools identityTools;

        public AccessController(IOptions<GameAdminSettings> gameAdminSettings, IIdentityTools identityTools)
        {
            this.gameAdminSettings = gameAdminSettings.Value;
            this.identityTools = identityTools;
        }

        [HttpGet("[action]")]
        public bool IsLoggedIn()
        {
            return this.identityTools.GetIdentity(this.HttpContext).HasValue;
        }

        [HttpGet("[action]")]
        public bool IsAdmin()
        {
            var id = this.identityTools.GetIdentity(this.HttpContext);
            return id.Match(some => this.gameAdminSettings.AdminGithubNames.Contains(some.LoginName), () => false);
        }
    }
}
