using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceApi.Models.Communication.Request
{
    public class UserTokenRequest
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Token { get; set; }
    }
}
