using meter_api.Datatypes;
using meter_api.Datatypes.Database;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace meter_api.Services
{
    public class AgentTokenService(IOptions<JwtOptions> options) : IAgentTokenService
    {
        private readonly JwtOptions _options = options.Value;

        public string GetAgentToken(MeterAgent agent)
        {
            ArgumentNullException.ThrowIfNull(agent);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim("agent_id", agent.Id.ToString()),
            new Claim("agent_display_name", agent.DisplayName ?? string.Empty),
            new Claim("issued_at", DateTime.UtcNow.ToString("O"))
        };

            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_options.Expiry),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool isAgentTokenValid(string agentToken)
        {
            if (string.IsNullOrWhiteSpace(agentToken))
                return false;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_options.Secret);

            try
            {
                tokenHandler.ValidateToken(agentToken, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = _options.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _options.Audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuerSigningKey = true
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
