namespace CodeGolf.Web.Attributes
{
    using System.Collections.Generic;

    public class GameAdminSettings
    {
        public List<string> AdminGithubNames { get; private set; } = new List<string>();

        public bool AllowNonAdminDashboard { get; private set; }
    }
}
