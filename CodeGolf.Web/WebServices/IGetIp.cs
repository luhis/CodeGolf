namespace CodeGolf.Web.WebServices
{
    using System.Net;
    using Microsoft.AspNetCore.Http;

    public interface IGetIp
    {
        IPAddress GetIp(HttpContext httpContext);
    }
}
