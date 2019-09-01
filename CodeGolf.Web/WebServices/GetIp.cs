namespace CodeGolf.Web.WebServices
{
    using System.Net;
    using Microsoft.AspNetCore.Http;

    public class GetIp : IGetIp
    {
        IPAddress IGetIp.GetIp(HttpRequest req) => req.HttpContext.Connection.RemoteIpAddress;
    }
}
