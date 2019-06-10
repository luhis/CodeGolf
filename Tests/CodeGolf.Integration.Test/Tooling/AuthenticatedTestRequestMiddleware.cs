using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CodeGolf.Integration.Test.Tooling
{
    public class AuthenticatedTestRequestMiddleware
    {
        private const string TestingCookieAuthentication = "TestCookieAuthentication";
        public const string TestingHeader = "X-Integration-Testing";
        public const string TestingHeaderValue = "abcde-12345";

        private readonly RequestDelegate next;

        public AuthenticatedTestRequestMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Headers.Keys.Contains(TestingHeader) &&
                context.Request.Headers[TestingHeader].First().Equals(TestingHeaderValue))
            {
                var claimsIdentity = new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "matt"),
                    new Claim(ClaimTypes.NameIdentifier, "mattID"),
                }, TestingCookieAuthentication);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                context.User = claimsPrincipal;
            }

            await this.next(context);
        }
    }
}