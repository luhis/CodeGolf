using Microsoft.Extensions.DependencyInjection;

namespace CodeGolf.Recaptcha
{
    public static class DiModule
    {
        public static void Add(IServiceCollection services)
        {
            services.AddSingleton<IGetCaptchaValue, GetCaptchaValue>();
            services.AddSingleton<IRecaptchaVerifier, RecaptchaVerifier>();
        }
    }
}