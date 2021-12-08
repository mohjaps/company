using company.Models;
using company.Models.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace company.Services
{
    public class ReCaptchaService : IReCaptchaService
    {
        private readonly ReCaptchaSettings _settings;
        public ReCaptchaSettings Config { get { return _settings; } }
        public ReCaptchaService(IOptions<ReCaptchaSettings> settings)
        {
            _settings = settings.Value;
        }
        
        public bool ValidateRecaptcha(string token)
        {
            string url = "https://www.google.com/recaptcha/api/siteverify?";
            bool ret = false;
            HttpClient httpClient = new HttpClient();

            var res = httpClient.GetAsync($"{url}secret={_settings.Secret}&response={token}").Result;
            if (res.StatusCode == HttpStatusCode.OK)
            {
                string content = res.Content.ReadAsStringAsync().Result;
                CaptchaResponse response = JsonSerializer.Deserialize<CaptchaResponse>(content);

                if (response.success)
                {
                    ret = true;
                }
            }

            return ret;
        }
    }
}
