using SpaceApi.Models.Communication.Request;
using SpaceApi.Models.Communication.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceApi.Servizi
{
    public interface IUserManager
    {
        public Task<UserTokenResponse> AddUserToken(UserTokenRequest req);
        public Task<UserTokenResponse> GetUserToken(string Username);

        public Task<UserTokenResponse> DeleteUserToken(string Username);
    }
}
