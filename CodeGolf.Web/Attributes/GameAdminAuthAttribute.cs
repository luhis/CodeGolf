namespace CodeGolf.Web.Attributes
{
    using System;
    using System.Threading.Tasks;
    using CodeGolf.Web.Tooling;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Options;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class GameAdminAuthAttribute : Attribute, IPageFilter, IAsyncActionFilter
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
            user.Match(
                currentUsername =>
            {
                if (!this.gameAdminSettings.AdminGithubNames.Contains(currentUsername.LoginName))
                {
                    context.Result = new ForbidResult();
                }
            },
                () => { context.Result = new ForbidResult(); });
        }

        void IPageFilter.OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {
        }

        Task IAsyncActionFilter.OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var user = this.identityTools.GetIdentity(context.HttpContext);
            user.Match(
                currentUsername =>
                {
                    if (!this.gameAdminSettings.AdminGithubNames.Contains(currentUsername.LoginName))
                    {
                        context.Result = new ForbidResult();
                    }
                },
                () => { context.Result = new ForbidResult(); });
            return next();
        }
    }
}
