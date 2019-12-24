namespace CodeGolf.Web.Attributes
{
    using System;
    using System.Threading.Tasks;
    using CodeGolf.Recaptcha;
    using CodeGolf.Web.WebServices;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RecaptchaAttribute : Attribute, IAsyncPageFilter, IAsyncActionFilter
    {
        private readonly IRecaptchaVerifier recaptchaVerifier;
        private readonly IGetCaptchaValue getCaptchaValue;
        private readonly IGetIp getIp;

        public RecaptchaAttribute(IRecaptchaVerifier recaptchaVerifier, IGetCaptchaValue getCaptchaValue, IGetIp getIp)
        {
            this.recaptchaVerifier = recaptchaVerifier;
            this.getCaptchaValue = getCaptchaValue;
            this.getIp = getIp;
        }

        Task IAsyncPageFilter.OnPageHandlerSelectionAsync(PageHandlerSelectedContext context) => Task.CompletedTask;

        async Task IAsyncPageFilter.OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            if (string.Equals(context.HttpContext.Request.Method, "POST", StringComparison.InvariantCultureIgnoreCase))
            {
                var ip = this.getIp.GetIp(context.HttpContext);
                var recaptcha = this.getCaptchaValue.Get(context.HttpContext.Request);
                if (!await this.recaptchaVerifier.IsValid(recaptcha, ip))
                {
                    context.ModelState.AddModelError(string.Empty, "Recaptcha is invalid");
                }
            }

            await next();
        }

        async Task IAsyncActionFilter.OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (string.Equals(context.HttpContext.Request.Method, "POST", StringComparison.InvariantCultureIgnoreCase))
            {
                var ip = this.getIp.GetIp(context.HttpContext);
                var recaptcha = context.HttpContext.Request.Headers["g-recaptcha-response"];
                if (!await this.recaptchaVerifier.IsValid(recaptcha, ip))
                {
                    context.Result = new UnauthorizedResult();
                }
                else
                {
                    await next();
                }
            }
            else
            {
                await next();
            }
        }
    }
}
