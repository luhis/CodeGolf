namespace CodeGolf.Recaptcha
{
    using Microsoft.Extensions.DependencyInjection;

    public static class DiModule
    {
        public static void Add(IServiceCollection services)
        {
            services.AddHttpClient<IRecaptchaVerifier, RecaptchaVerifier>();
        }
    }
}
