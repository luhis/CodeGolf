using System.Net;
using System.Threading.Tasks;

namespace CodeGolf.Recaptcha
{
    public interface IRecaptchaVerifier
    {
        Task<bool> IsValid(string response, IPAddress ip);
    }
}