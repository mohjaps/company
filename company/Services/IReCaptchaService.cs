using company.Models.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace company.Services
{
    public interface IReCaptchaService
    {
        ReCaptchaSettings Config { get; }
        bool ValidateRecaptcha(string token);
    }
}
