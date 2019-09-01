namespace CodeGolf.Web.Tooling
{
    using CodeGolf.Domain;
    using Microsoft.AspNetCore.Http;
    using Optional;

    public interface IIdentityTools
    {
       Option<User> GetIdentity(HttpContext hc);
    }
}
