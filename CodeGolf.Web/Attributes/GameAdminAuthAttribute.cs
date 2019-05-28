using System;
using CodeGolf.Web.Tooling;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace CodeGolf.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class GameAdminAuthAttribute : Attribute, IPageFilter
    {
        private readonly GameAdminSettings gameAdminSettings;
        private readonly IIdentityTools identityTools;

        public GameAdminAuthAttribute(IOptions<GameAdminSettings> gameAdminSettings, IIdentityTools identityTools)
        {
            this.identityTools = identityTools;
            this.gameAdminSettings = gameAdminSettings.Value;
        }

        void IPageFilter.OnPageHandlerSelected(PageHandlerSelectedContext context)
        {
        }

        void IPageFilter.OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            var user = this.identityTools.GetIdentity(context.HttpContext);
            user.Match(some =>
            {
                if (!some.Equals(this.gameAdminSettings.AdminGithubName))
                {
                    context.Result = new ForbidResult();
                }
            }, () => { context.Result = new ForbidResult(); });
        }

        void IPageFilter.OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {
        }
    }
}