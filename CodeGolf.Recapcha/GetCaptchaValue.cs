using EnsureThat;
using Microsoft.AspNetCore.Http;

namespace CodeGolf.Recaptcha
{
    public class GetCaptchaValue : IGetCaptchaValue
    {
        string IGetCaptchaValue.Get(HttpRequest req) => Ensure.String.IsNotNull(req.HttpContext.Request.Form["g-recaptcha-response"]);
    }
}