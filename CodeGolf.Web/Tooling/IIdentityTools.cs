using System;
using Microsoft.AspNetCore.Http;
using Optional;

namespace CodeGolf.Web.Tooling
{
    public interface IIdentityTools
    {
       Option<string> GetIdentity(HttpContext hc);
    }
}