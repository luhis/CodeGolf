using System.Net;
using Microsoft.AspNetCore.Http;

namespace CodeGolf.Web
{
    public class GetIp : IGetIp
    {
        IPAddress IGetIp.GetIp(HttpRequest req) => req.HttpContext.Connection.RemoteIpAddress;
    }
}