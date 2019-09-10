namespace CodeGolf.Web.Tooling
{
    using System.Security.Claims;
    using CodeGolf.Domain;
    using Microsoft.AspNetCore.Http;
    using Optional;

    public class IdentityTools : IIdentityTools
    {
        Option<User> IIdentityTools.GetIdentity(HttpContext hc)
        {
            var user = hc.User;
            if (user.Identity.IsAuthenticated)
            {
                var identifier = GetByKey(user, ClaimTypes.NameIdentifier);
                var loginName = GetByKey(user, "urn:github:login");
                var realName = GetByKey(user, ClaimTypes.Name);
                var avatar = GetByKey(user, "urn:github:avatar");
                var u = new User(int.Parse(identifier), loginName, realName ?? loginName, avatar);
                return Option.Some(u);
            }

            return Option.None<User>();
        }

        private static string GetByKey(ClaimsPrincipal cp, string key) => cp.FindFirst(c => c.Type == key)?.Value;
    }
}
