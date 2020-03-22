namespace CodeGolf.Recaptcha
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;

    public class RecaptchaVerifier : IRecaptchaVerifier
    {
        private const string Api = "https://www.google.com/recaptcha/api/siteverify";
        private readonly RecaptchaSettings settings;
        private readonly HttpClient httpClient;

        public RecaptchaVerifier(IOptions<RecaptchaSettings> settings, HttpClient httpClient)
        {
            this.httpClient = httpClient;
            this.settings = settings.Value;
        }

        async Task<bool> IRecaptchaVerifier.IsValid(string response, IPAddress ip, CancellationToken cancellationToken)
        {
            using (var content = new FormUrlEncodedContent(
                new Dictionary<string, string>()
                {
                    { "secret", this.settings.SecretKey },
                    { "response", response },
                    { "ipaddress", ip.ToString() },
                }))
            using (var result = await this.httpClient.PostAsync(Api, content, cancellationToken))
            {
                var s = await result.Content.ReadAsStringAsync();
                return JsonSerializer
                    .Deserialize<ReCaptchaResponse>(s, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true })
                    .Success;
            }
        }
    }
}
