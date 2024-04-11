using Databases.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.IServices;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class TokenService : ITokenService
    {
        private readonly string jwtSecretKey;

        public TokenService(IConfiguration configuration)
        {
            jwtSecretKey = configuration["JwtSecretKey"] ?? string.Empty;
        }

        public string GenerateToken(Employee employee)
        {
            var claims = new List<Claim>()
            {
                new (JwtRegisteredClaimNames.Name, employee.AccountName),
                new (JwtRegisteredClaimNames.NameId, employee.Id),
                new Claim(ClaimTypes.Role, employee.Role.ToString())
                
            };
            var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new(symmetricKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}

