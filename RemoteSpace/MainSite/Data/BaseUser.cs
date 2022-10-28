using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MainSite.Data
{
    public class BaseUser:IdentityUser
    {
        [Required, PersonalData]
        public bool Admin { get; set; }
        [Required, PersonalData]
        public int Space { get; set; }
    }
}
