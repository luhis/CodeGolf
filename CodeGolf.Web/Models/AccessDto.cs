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

        private AccessDto()
        {
        }

        public bool IsLoggedIn { get; private set; }

        public bool IsAdmin { get; private set; }

        public bool ShowDashboard { get; private set; }
    }
}
