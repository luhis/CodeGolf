namespace CodeGolf.Integration.Test.Fixtures
{
    using System.Net;
    using System.Threading.Tasks;
    using CodeGolf.Recaptcha;

    public class MockRecaptchaVerifier : IRecaptchaVerifier
    {
        Task<bool> IRecaptchaVerifier.IsValid(string response, IPAddress ip)
        {
            return Task.FromResult(true);
        }
    }
}
