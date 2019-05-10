using System;
using Microsoft.AspNetCore.Http;

namespace CodeGolf.Web.Tooling
{
    public interface IIdentityTools
    {
        Guid GetIdentity(HttpRequest hr);
    }
}