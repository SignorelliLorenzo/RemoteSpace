﻿using Api_Pcto.Models;
using Microsoft.AspNetCore.Authorization;
using SpaceApi.Servizi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SpaceApi.Config;
using Api_Pcto.Models.DTOS.Responses;
using SpaceApi.Models.Communication.Request;
using Api_Pcto.Config;
using Microsoft.AspNetCore.Identity;

namespace SpaceApi.Controllers
{
    [Route("api/Autentication")]
    [ApiController]
   
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly JwtConfig _jwtConfig;
        private readonly IUserManager userTokenManager;

        public AuthenticationController(
            UserManager<UserModel> userManager,
            IOptionsMonitor<JwtConfig> config,
            IUserManager TokenManager)
        {
            this._userManager = userManager;
            this._jwtConfig = config.CurrentValue;
            this.userTokenManager = TokenManager;
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("Register")]
        public async Task<ActionResult<UserRegistrationResponse>> Register(UserRegistrationRequest user)

        {
      
            if (!_userManager.Users.Where(x => x.Email == _userManager.GetUserId(User)).First().Admin)
            {
                return new UserRegistrationResponse()
                {
                    Errors = new List<string>()
                        {
                            "Non sei un admin"
                        },
                    Success = false
                };
            }
            if (ModelState.IsValid)
            {
                var ExistingUser = await _userManager.FindByEmailAsync(user.Email);
                if (ExistingUser != null)
                {
                    return new UserRegistrationResponse()
                    {
                        Errors = new List<string>()
                        {
                            "Email Gia In Utilizzo"
                        },
                        Success = false
                    };
                }
                var ExistingUserByUsername = await _userManager.FindByNameAsync(user.Email);
                if (ExistingUserByUsername != null)
                {
                    return new UserRegistrationResponse()
                    {
                        Errors = new List<string>()
                        {
                            "Username Gia in Utilizzo"
                        },
                        Success = false
                    };
                }
                var NewUser = new UserModel() { Email = user.Email, UserName = user.Email,Admin=user.Admin };
                var IsCreated = await _userManager.CreateAsync(NewUser, user.Password);
                var NewUserToken = new UserTokenRequest()
                {
                    Name = user.Email,
                    Token = GenerateJwtToken(NewUser)
                };

                if (IsCreated.Succeeded)
                {
                    await userTokenManager.AddUserToken(NewUserToken);
                    return new UserRegistrationResponse()
                    {
                        Success = true,
                        Token = NewUserToken.Token,
                        Errors = null,
                        Username = NewUser.UserName
                    };

                }
                else
                {
                    return new UserRegistrationResponse()
                    {
                        Errors = IsCreated.Errors.Select(x => x.Description).ToList(),
                        Success = false
                    };
                }
            }
            return new UserRegistrationResponse()
            {
                Errors = new List<string>()
                {
                    "Invalid payload"
                },
                Success = false
            };
        }

        private string GenerateJwtToken(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.SecretKey);
            var TokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id",user.Id),
                    new Claim(JwtRegisteredClaimNames.Email,user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub,user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
                }),
                //Nel caso si voglia aggiungere la scadenza al token
                // Expires = DateTime.UtcNow.AddHours(6),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var Token = jwtTokenHandler.CreateToken(TokenDescriptor);
            var JwtToken = jwtTokenHandler.WriteToken(Token);

            return JwtToken;
        }

        [HttpPost("Login")]
        public async Task<AuthResult> Login(UserLoginRequest req)
        {
            var User = await _userManager.FindByEmailAsync(req.Email);
            if (User != null)
            {
                var checkpasswd = await _userManager.CheckPasswordAsync(User, req.Password);
                if (checkpasswd)
                {
                    var UserToken = await userTokenManager.GetUserToken(User.UserName);
                    return new AuthResult()
                    {
                        Success = true,
                        Username = User.Email,
                        Token = UserToken.Token,
                        Errors = null
                    };
                }
                return new AuthResult()
                {
                    Success = false,
                    Token = null,
                    Errors = new List<string>()
                    {
                        "Password non corretta"
                    },
                    Username = null
                };
            }
            return new AuthResult()
            {
                Success = false,
                Token = null,
                Errors = new List<string>()
                {
                    "Utente non esistente"
                },
                Username = null
            };
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("Delete")]
        public async Task<AuthResult> Delete(UserDeleteRequest req)
        {
            var User = await _userManager.FindByEmailAsync(req.Email);
            if (User != null)
            {
                var checkpasswd = await _userManager.CheckPasswordAsync(User, req.Password);
                if (checkpasswd)
                {
                    await _userManager.DeleteAsync(User);
                    var a = await userTokenManager.DeleteUserToken(User.Email);
                    return new AuthResult()
                    {
                        Success = true,
                        Username = User.Email,
                        Token = a.Token,
                        Errors = null
                    };
                }
                return new AuthResult()
                {
                    Success = false,
                    Token = null,
                    Errors = new List<string>()
                    {
                        "Password non corretta"
                    },
                    Username = null
                };
            }
            return new AuthResult()
            {
                Success = false,
                Token = null,
                Errors = new List<string>()
                {
                    "Utente non esistente"
                },
                Username = null
            };
        }
    }
}
