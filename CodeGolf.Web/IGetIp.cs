using System.Net;
using Microsoft.AspNetCore.Http;

namespace CodeGolf.Web
{
    public interface IGetIp
    {
        IPAddress GetIp(HttpRequest req);
    }
}