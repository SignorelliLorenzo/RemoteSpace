using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MainSite.Config
{
    public class SetUsers
    {
        public static void Initialize(IServiceProvider Provider)
        {
            var userManager = Provider.GetRequiredService<UserManager<IdentityUser>>();
            foreach(var user in userManager.Users)
            {
                userManager.UpdateSecurityStampAsync(user);
            }
        }
    }
}
