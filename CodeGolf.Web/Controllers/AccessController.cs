namespace CodeGolf.Web.Controllers
{
    using System.Threading.Tasks;
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
        private readonly IGameAdminChecker gameAdminChecker;

        public AccessController(IOptions<GameAdminSettings> gameAdminSettings, IIdentityTools identityTools, IGameAdminChecker gameAdminChecker)
        {
            this.gameAdminSettings = gameAdminSettings.Value;
            this.identityTools = identityTools;
            this.gameAdminChecker = gameAdminChecker;
        }

        [HttpGet("[action]")]
        public AccessDto GetAccess()
        {
            var id = this.identityTools.GetIdentity(this.HttpContext);
            var isAdmin = this.gameAdminChecker.IsAdmin(this.HttpContext);
            return new AccessDto(id.HasValue, isAdmin, isAdmin || this.gameAdminSettings.AllowNonAdminDashboard);
        }

        [HttpPost("[action]")]
        public Task SignOut()
        {
            return this.HttpContext.SignOutAsync();
        }
    }
}
