using System.Security.Claims;
using CodeGolf.Domain;
using Microsoft.AspNetCore.Http;
using Optional;

namespace CodeGolf.Web.Tooling
{
    public class IdentityTools : IIdentityTools
    {
        private static string GetByKey(ClaimsPrincipal cp, string key) => cp.FindFirst(c => c.Type == key).Value;

        Option<User> IIdentityTools.GetIdentity(HttpContext hc)
        {
            var user = hc.User;
            if (user.Identity.IsAuthenticated)
            {
                var identifier = GetByKey(user, ClaimTypes.NameIdentifier);
                var loginName = GetByKey(user, "urn:github:login");
                var realName = GetByKey(user, ClaimTypes.Name);
                var avatar = GetByKey(user, "urn:github:avatar");
                var u = new User(int.Parse(identifier), loginName, realName, avatar);
                return Option.Some(u);
            }

            return Option.None<User>();
        }
    }
}