namespace CodeGolf.Web.Attributes
{
    using System;
    using System.Threading.Tasks;
    using CodeGolf.Web.Tooling;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class GameAdminAuthAttribute : Attribute, IAsyncActionFilter
    {
        private readonly IGameAdminChecker gameAdminChecker;

        public GameAdminAuthAttribute(IGameAdminChecker gameAdminChecker)
        {
            this.gameAdminChecker = gameAdminChecker;
        }

        Task IAsyncActionFilter.OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!this.gameAdminChecker.IsAdmin(context.HttpContext))
            {
                context.Result = new ForbidResult();
                return Task.CompletedTask;
            }
            else
            {
                return next();
            }
        }
    }
}
