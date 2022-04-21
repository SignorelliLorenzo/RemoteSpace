using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
namespace Api_Pcto.Models
{
    public class UserModel : IdentityUser
    {
        public bool Admin { get; set; }
    }
}

