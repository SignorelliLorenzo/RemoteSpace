using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainSite.Data
{
    public class UserIdentityCompleted:IdentityUser
    {
        [Required]
        public byte[] IV { get; set; }
        [Required]
        public byte[] Key { get; set; }
    }
}
