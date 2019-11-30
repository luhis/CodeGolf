namespace CodeGolf.Web.Models
{
    public class AccessDto
    {
        public AccessDto(bool isLoggedIn, bool isAdmin, bool showDashboard)
        {
            this.IsLoggedIn = isLoggedIn;
            this.IsAdmin = isAdmin;
            this.ShowDashboard = showDashboard;
        }

        public bool IsLoggedIn { get; }

        public bool IsAdmin { get; }

        public bool ShowDashboard { get; }
    }
}
