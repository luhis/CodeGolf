namespace CodeGolf.Web.Tooling
{
    using CodeGolf.Web.Attributes;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;

    public class GameAdminChecker : IGameAdminChecker
    {
        private readonly GameAdminSettings gameAdminSettings;
        private readonly IIdentityTools identityTools;

        public GameAdminChecker(IOptions<GameAdminSettings> gameAdminSettings, IIdentityTools identityTools)
        {
            this.gameAdminSettings = gameAdminSettings.Value;
            this.identityTools = identityTools;
        }

        bool IGameAdminChecker.IsAdmin(HttpContext hc)
        {
            var id = this.identityTools.GetIdentity(hc);
            return id.Match(
                currentUsername =>
                    {
                        if (this.gameAdminSettings.AdminGithubNames.Contains(currentUsername.LoginName))
                        {
                            return true;
                        }

                        return false;
                    },
                () => false);
        }
    }
}
