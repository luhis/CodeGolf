namespace CodeGolf.Web.Attributes
{
    using System;
    using System.Threading.Tasks;
    using CodeGolf.Recaptcha;
    using CodeGolf.Web.WebServices;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class RecaptchaAttribute : Attribute, IAsyncActionFilter
    {
        private readonly IRecaptchaVerifier recaptchaVerifier;
        private readonly IGetIp getIp;

        public RecaptchaAttribute(IRecaptchaVerifier recaptchaVerifier, IGetIp getIp)
        {
            this.recaptchaVerifier = recaptchaVerifier;
            this.getIp = getIp;
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
