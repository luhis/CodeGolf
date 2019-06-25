namespace CodeGolf.Web.Tooling
{
    using Microsoft.AspNetCore.Http;

    public interface IGameAdminChecker
    {
        bool IsAdmin(HttpContext hc);
    }
}