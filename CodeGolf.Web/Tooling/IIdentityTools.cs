using CodeGolf.Domain;
using Microsoft.AspNetCore.Http;
using Optional;

namespace CodeGolf.Web.Tooling
{
    public interface IIdentityTools
    {
       Option<User> GetIdentity(HttpContext hc);
    }
}