using Microsoft.AspNetCore.Http;
using Optional;

namespace CodeGolf.Web.Tooling
{
    public class IdentityTools : IIdentityTools
    {
        Option<string> IIdentityTools.GetIdentity(HttpContext hc)
        {
            var user = hc.User;
            if (user.Identity.IsAuthenticated)
            {
                var loginName = user.FindFirst(c => c.Type == "urn:github:login").Value;
                return Option.Some(loginName);
            }

            return Option.None<string>();
        }
    }
}