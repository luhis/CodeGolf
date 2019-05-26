using System;
using System.Threading.Tasks;
using CodeGolf.Recaptcha;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CodeGolf.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RecaptchaAttribute : Attribute, IAsyncPageFilter
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
                var ip = this.getIp.GetIp(context.HttpContext.Request);
                var recaptcha = this.getCaptchaValue.Get(context.HttpContext.Request);
                if (!await this.recaptchaVerifier.IsValid(recaptcha, ip))
                {
                    context.ModelState.AddModelError(string.Empty, "Recaptcha is invalid");
                }
            }

            await next();
        }
    }
}