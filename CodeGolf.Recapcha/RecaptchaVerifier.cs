﻿namespace CodeGolf.Recaptcha
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;

    public class RecaptchaVerifier : IRecaptchaVerifier
    {
        private const string Api = "https://www.google.com/recaptcha/api/siteverify";
        private readonly RecaptchaSettings settings;

        public RecaptchaVerifier(IOptions<RecaptchaSettings> settings)
        {
            this.settings = settings.Value;
        }

        async Task<bool> IRecaptchaVerifier.IsValid(string response, IPAddress ip)
        {
            using (var client = new HttpClient())
            {
                using (var content = new FormUrlEncodedContent(
                    new Dictionary<string, string>()
                    {
                        { "secret", this.settings.SecretKey },
                        { "response", response },
                        { "ipaddress", ip.ToString() },
                    }))
                using (var result = await client.PostAsync(Api, content))
                {
                    var s = await result.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<ReCaptchaResponse>(s, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }).Success;
                }
            }
        }
    }
}
