using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace company.Models.UsersSec
{
    public class AppUser : IdentityUser
    {
        // User Identification 
        public string FullName { get; set; }
    }
}
