namespace CodeGolf.Recaptcha
{
    using Microsoft.Extensions.DependencyInjection;

    public static class DiModule
    {
        public static void Add(IServiceCollection services)
        {
            services.AddSingleton<IGetCaptchaValue, GetCaptchaValue>();
            services.AddSingleton<IRecaptchaVerifier, RecaptchaVerifier>();
        }
    }
}