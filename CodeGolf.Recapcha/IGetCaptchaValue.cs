namespace CodeGolf.Recaptcha
{
    using Microsoft.AspNetCore.Http;

    public interface IGetCaptchaValue
    {
        string Get(HttpRequest req);
    }
}
