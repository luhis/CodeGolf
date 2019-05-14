using System;
using Microsoft.AspNetCore.Http;

namespace CodeGolf.Web.Tooling
{
    public class IdentityTools : IIdentityTools
    {
        Guid IIdentityTools.GetIdentity(HttpRequest hr) => Guid.NewGuid();
    }
}