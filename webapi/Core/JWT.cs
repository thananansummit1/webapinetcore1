using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using webapi.Attributes;
using webapi.Models;

namespace webapi.Core
{
    public static class JWT
    {
        private static string secret = "1WV6ZDK&-m5c2f#?";
        private static int webTtl = 86400;            // 1 day
        private static int mobileTtl = 604800;           // 1 week

        private static bool IsMobile = false;

        public static bool IsBypass = false;
        public static string Version;


        public static string GenerateToken(User user, bool isMobile)
        {
            byte[] key = Encoding.ASCII.GetBytes(JWT.secret);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);

            int ttl = JWT.webTtl;
            if (isMobile)
            {
                ttl = JWT.mobileTtl;
            }
            JWT.IsMobile = isMobile;

            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Sid, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Version, Version )
                }),
                Expires = DateTime.UtcNow.AddMinutes(ttl),
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);

            // Also add to header
            return handler.WriteToken(token);
        }

        public static User ValidateToken(HttpContext context, AuthorizationType type = AuthorizationType.None, AuthorizationRole role = AuthorizationRole.None)
        {
            if (!context.Request.Headers.Keys.Contains("Authorization"))
            {
                throw new SecurityTokenException("Need Authorization");
            }

            string token = context.Request.Headers["Authorization"];
            token = token.Replace("Bearer ", "");

            return DecryptionToken(token, type, role);
        }

        public static User ValidateToken(string token, AuthorizationType type = AuthorizationType.None, AuthorizationRole role = AuthorizationRole.None)
        {
            return DecryptionToken(token, type, role);
        }

        private static User DecryptionToken(string token, AuthorizationType type, AuthorizationRole role)
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
                if (jwtToken == null)
                {
                    throw new SecurityTokenException("Invalid Authorization");
                }

                byte[] key = Encoding.ASCII.GetBytes(JWT.secret);
                TokenValidationParameters parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
                SecurityToken securityToken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, parameters, out securityToken);
                if (principal == null)
                {
                    throw new SecurityTokenException("Invalid ClaimsPrincipal");
                }

                Claim version = principal.FindFirst(ClaimTypes.Version);

                if (version == null)
                {
                    throw new SecurityTokenException("Invalid Version");
                }

                if (version.Value != JWT.Version)
                {
                    throw new SecurityTokenException("Invalid Version");
                }

                Claim id = principal.FindFirst(ClaimTypes.Sid);

                if (id == null)
                {
                    throw new SecurityTokenException("Invalid ClaimType");
                }

                User testUsers = new User ();

                var listUser = testUsers.createUserTest();

                User findingUser = listUser.Where(x=>x.Id == int.Parse(id.Value)).FirstOrDefault();


                if (findingUser == null)
                {
                    throw new SecurityTokenException("User Not Found");
                }

                if ((findingUser.TypeId & (int)type) == 0)
                {
                    throw new SecurityTokenException("Invalid User Type, \"" + type.GetType().GetEnumName(type) + "\" is required.");
                }

                if (role != AuthorizationRole.None && (findingUser.RoleId & (int)role) == 0)
                {
                    throw new SecurityTokenException("Invalid User Role, \"" + role.GetType().GetEnumName(role) + "\" is required.");
                }

                return findingUser;
            }
            catch (Exception e)
            {
                throw new SecurityTokenException(e.ToString());
            }
        }

        public static void ExtendToken(HttpContext context, User user)
        {
            string token = JWT.GenerateToken(user, context.Request.Headers.ContainsKey(HttpContextItemKey.IsMobile));

            context.Response.Headers.Remove("Authorization");
            context.Response.Headers.Add("Authorization", token);
        }
    }
}
