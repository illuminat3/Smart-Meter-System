using meter_api.Datatypes.Database;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace meter_api.Services
{
    public class JwtService(IOptions<JwtOptions> options) : IJwtService
    {
        private readonly JwtOptions _options = options.Value;

        public string GetClientJwt(Client client)
        {
            var claims = new List<Claim>
            {
                new("id", client.Id),
                new("name", client.Name),
            };

            foreach (var meterId in client.MeterIds)
            {
                claims.Add(new Claim("meterId", meterId));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_options.Expiry),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool IsValidJwt(string jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_options.Secret);

            try
            {
                handler.ValidateToken(jwt, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = _options.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _options.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = true
                }, out _);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
