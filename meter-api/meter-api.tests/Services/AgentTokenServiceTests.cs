using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;

namespace meter_api.tests.Services
{
    public class AgentTokenServiceTests
    {
        private static IOptions<JwtOptions> CreateOptions(int expiryMinutes = 60)
        {
            return Options.Create(new JwtOptions
            {
                Secret = "super_secret_test_key_12345_super_secret_test_key_12345",
                Issuer = "test-issuer",
                Audience = "test-audience",
                Expiry = expiryMinutes
            });
        }

        [Fact]
        public void GetAgentToken_NullAgent_ThrowsArgumentNullException()
        {
            // Arrange
            var options = CreateOptions();
            var sut = new AgentTokenService(options);

            // Act
            var act = () => sut.GetAgentToken(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetAgentToken_ReturnsTokenWithExpectedClaims()
        {
            // Arrange
            var options = CreateOptions();
            var sut = new AgentTokenService(options);
            var agent = new MeterAgent
            {
                Id = "42",
                DisplayName = "Test Agent",
                TotalUsage = 10m,
                TotalBilling = 5m
            };

            // Act
            var tokenString = sut.GetAgentToken(agent);
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenString);

            // Assert
            tokenString.Should().NotBeNullOrWhiteSpace();
            token.Claims.First(c => c.Type == "agent_id").Value.Should().Be("42");
            token.Claims.First(c => c.Type == "agent_display_name").Value.Should().Be("Test Agent");
            token.Claims.Any(c => c.Type == "issued_at").Should().BeTrue();
            token.Issuer.Should().Be("test-issuer");
            token.Audiences.Should().Contain("test-audience");
        }

        [Fact]
        public void IsAgentTokenValid_ReturnsTrueForValidToken()
        {
            // Arrange
            var options = CreateOptions();
            var sut = new AgentTokenService(options);
            var agent = new MeterAgent
            {
                Id = "1",
                DisplayName = "Agent",
                TotalUsage = 0m,
                TotalBilling = 0m
            };
            var token = sut.GetAgentToken(agent);

            // Act
            var valid = sut.IsAgentTokenValid(token);

            // Assert
            valid.Should().BeTrue();
        }

        [Fact]
        public void IsAgentTokenValid_ReturnsFalseForInvalidTokenString()
        {
            // Arrange
            var options = CreateOptions();
            var sut = new AgentTokenService(options);

            // Act
            var valid = sut.IsAgentTokenValid("not_a_real_token");

            // Assert
            valid.Should().BeFalse();
        }

        [Fact]
        public void IsAgentTokenValid_ReturnsFalseForExpiredToken()
        {
            // Arrange
            var options = CreateOptions(expiryMinutes: -10);
            var sut = new AgentTokenService(options);
            var agent = new MeterAgent
            {
                Id = "1",
                DisplayName = "Agent",
                TotalUsage = 0m,
                TotalBilling = 0m
            };
            var token = sut.GetAgentToken(agent);

            // Act
            var valid = sut.IsAgentTokenValid(token);

            // Assert
            valid.Should().BeFalse();
        }
    }
}
