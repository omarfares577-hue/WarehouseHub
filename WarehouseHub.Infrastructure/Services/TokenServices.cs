using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WarehouseHub.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using WarehouseHub.Core.Contracts;
namespace WarehouseHub.Infrastructure.Services
{
    public class TokenServices : ITokenServices
    {
        private readonly IConfiguration _config;
        public TokenServices (IConfiguration config) { _config = config; }
        public async Task<string> CreateTokenAsync(AppUser user,UserManager<AppUser> admin)
        {
            var claims = new List<Claim> {
                new Claim (ClaimTypes.GivenName, user.DisplayName),
                new Claim (ClaimTypes.Email, user.Email!),
            };
            var Roles = await admin.GetRolesAsync(user);
            foreach (var role in Roles)
                claims.Add(new Claim(ClaimTypes.Role, role));
            var authkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]!));
            var token = new JwtSecurityToken(
    issuer: _config["JWT:ValidIssuer"]!,
    audience: _config["JWT:ValidAudience"],
    claims: claims,
    expires: DateTime.Now.AddDays(Double.Parse(_config["JWT:ExpiresInDays"]!)),
    signingCredentials: new SigningCredentials(authkey, SecurityAlgorithms.HmacSha256)
    );
            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
