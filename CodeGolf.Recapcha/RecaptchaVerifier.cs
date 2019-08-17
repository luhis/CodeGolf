namespace CodeGolf.Recaptcha
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    public class RecaptchaVerifier : IRecaptchaVerifier
    {
        private static readonly string Api = "https://www.google.com/recaptcha/api/siteverify";
        private readonly RecaptchaSettings settings;

        public RecaptchaVerifier(IOptions<RecaptchaSettings> settings)
        {
            this.settings = settings.Value;
        }

        async Task<bool> IRecaptchaVerifier.IsValid(string response, IPAddress ip)
        {
            using (var client = new HttpClient())
            {
                using (var result = await client.PostAsync(
                                        Api,
                                        new FormUrlEncodedContent(
                                            new Dictionary<string, string>()
                                                {
                                                    { "secret", this.settings.SecretKey },
                                                    { "response", response },
                                                    { "ipaddress", ip.ToString() },
                                                })))
                {
                    var s = await result.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ReCaptchaResponse>(s).Success;
                }
            }
        }
    }
}