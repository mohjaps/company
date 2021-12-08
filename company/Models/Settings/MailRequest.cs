using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace company.Models.Settings
{
    public class MailRequest
    {
        public string ToEMail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
