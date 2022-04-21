using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceApi.Models.Communication.Response
{
    public class UserTokenResponse
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public DateTime CreationTime { get; set; }

        public List<string> Errors { get; set; }
    }
}
