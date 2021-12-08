using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace company.Models.UsersSec
{
    public class AppIdentityDbContext : IdentityDbContext<AppUser>
    {
        // Identification Context
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) : base (options)
        {

        }
    }
}
