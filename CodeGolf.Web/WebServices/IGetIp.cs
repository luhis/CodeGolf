using System.Net;
using Microsoft.AspNetCore.Http;

namespace CodeGolf.Web.WebServices
{
    public interface IGetIp
    {
        IPAddress GetIp(HttpRequest req);
    }
}