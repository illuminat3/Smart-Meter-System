using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;

namespace meter_api.tests.Services;

public class JwtServiceTests
{
    private static IOptions<JwtOptions> CreateOptions(int expiryMinutes = 60)
    {
        return Options.Create(new JwtOptions
        {
            Secret = "another_super_secret_test_key_98765",
            Issuer = "test-issuer",
            Audience = "test-audience",
            Expiry = expiryMinutes
        });
    }

    [Fact]
    public void GetClientJwt_ReturnsTokenWithExpectedClaims()
    {
        // Arrange
        var options = CreateOptions();
        var sut = new JwtService(options);
        var client = new Client
        {
            Id = "10",
            Name = "Test Client",
            MeterIds = { "m1", "m2" }
        };

        // Act
        var jwt = sut.GetClientJwt(client);
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);

        // Assert
        jwt.Should().NotBeNullOrWhiteSpace();
        token.Claims.First(c => c.Type == "id").Value.Should().Be("10");
        token.Claims.First(c => c.Type == "name").Value.Should().Be("Test Client");
        token.Claims.Where(c => c.Type == "meterId").Select(c => c.Value)
            .Should().BeEquivalentTo("m1", "m2");
        token.Issuer.Should().Be("test-issuer");
        token.Audiences.Should().Contain("test-audience");
    }

    [Fact]
    public void IsValidJwt_ReturnsTrueForValidClientToken()
    {
        // Arrange
        var options = CreateOptions();
        var sut = new JwtService(options);
        var client = new Client
        {
            Id = "1",
            Name = "Client Name",
            MeterIds = { "m1" }
        };
        var jwt = sut.GetClientJwt(client);

        // Act
        var valid = sut.IsValidJwt(jwt);

        // Assert
        valid.Should().BeTrue();
    }

    [Fact]
    public void IsValidJwt_ReturnsFalseForInvalidString()
    {
        // Arrange
        var options = CreateOptions();
        var sut = new JwtService(options);

        // Act
        var valid = sut.IsValidJwt("this_is_not_a_jwt");

        // Assert
        valid.Should().BeFalse();
    }

    [Fact]
    public void IsValidJwt_ReturnsFalseForExpiredToken()
    {
        // Arrange
        var options = CreateOptions(expiryMinutes: -10);
        var sut = new JwtService(options);
        var client = new Client
        {
            Id = "1",
            Name = "Client",
            MeterIds = { "m1" }
        };
        var jwt = sut.GetClientJwt(client);

        // Act
        var valid = sut.IsValidJwt(jwt);

        // Assert
        valid.Should().BeFalse();
    }
}
