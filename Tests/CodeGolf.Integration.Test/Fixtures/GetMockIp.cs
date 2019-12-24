namespace CodeGolf.Integration.Test.Fixtures
{
    using System.Net;
    using CodeGolf.Web.WebServices;
    using Microsoft.AspNetCore.Http;

    public class GetMockIp : IGetIp
    {
        IPAddress IGetIp.GetIp(HttpContext httpContext)
        {
            return IPAddress.Loopback;
        }
    }
}
