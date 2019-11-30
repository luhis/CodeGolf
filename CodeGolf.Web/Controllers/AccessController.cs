namespace CodeGolf.Web.Controllers
{
    using System.Threading.Tasks;
    using CodeGolf.Web.Attributes;
    using CodeGolf.Web.Models;
    using CodeGolf.Web.Tooling;
    using Microsoft.AspNetCore.Authentication;
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
        public AccessDto GetAccess()
        {
            var id = this.identityTools.GetIdentity(this.HttpContext);
            var isAdmin = id.Match(some => this.gameAdminSettings.AdminGithubNames.Contains(some.LoginName), () => false);
            return new AccessDto(id.HasValue, isAdmin, isAdmin || this.gameAdminSettings.AllowNonAdminDashboard);
        }

        [HttpPost("[action]")]
        public Task SignOut()
        {
            return this.HttpContext.SignOutAsync();
        }
    }
}
