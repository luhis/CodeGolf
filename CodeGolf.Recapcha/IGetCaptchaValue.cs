using Microsoft.AspNetCore.Http;

namespace CodeGolf.Recaptcha
{
    public interface IGetCaptchaValue
    {
        string Get(HttpRequest req);
    }
}