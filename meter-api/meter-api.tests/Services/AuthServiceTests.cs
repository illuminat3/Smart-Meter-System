using meter_api.Datatypes.Requests;
using Microsoft.AspNetCore.Http;

namespace meter_api.tests.Services;

public class AuthServiceTests
{
    private static AuthService CreateSut(
        out IDatabaseService databaseService,
        out IHashService hashService,
        out IJwtService jwtService,
        out IAgentTokenService agentTokenService)
    {
        databaseService = Substitute.For<IDatabaseService>();
        hashService = Substitute.For<IHashService>();
        jwtService = Substitute.For<IJwtService>();
        agentTokenService = Substitute.For<IAgentTokenService>();
        return new AuthService(databaseService, hashService, jwtService, agentTokenService);
    }

    [Fact]
    public async Task AgentLogin_ValidCredentials_ReturnsResponseWithToken()
    {
        // Arrange
        var sut = CreateSut(out var databaseService, out var hashService, out var _, out var agentTokenService);

        var request = new AgentLoginRequest
        {
            MeterId = "meter-1",
            Username = "agentuser",
            Password = "password123"
        };

        var storedCredentials = new MeterAgentCredentials
        {
            Id = "cred-1",
            MeterId = "meter-1",
            Username = "agentuser",
            HashedPassword = "hashed-password"
        };

        var fullAgent = new FullMeterAgent
        {
            Id = "meter-1",
            DisplayName = "Agent One",
            Credentials = storedCredentials,
            Readings = [],
            TotalUsage = 0m,
            TotalBilling = 0m
        };

        databaseService
            .Get<MeterAgentCredentials>(Arg.Any<Dictionary<string, string>>(), true)
            .Returns(storedCredentials);

        hashService
            .GetHash("password123")
            .Returns("hashed-password");

        databaseService
            .GetFullMeterAgentFromId("meter-1")
            .Returns(fullAgent);

        agentTokenService
            .GetAgentToken(fullAgent)
            .Returns("agent-token");

        // Act
        var response = await sut.AgentLogin(request);

        // Assert
        response.MeterId.Should().Be("meter-1");
        response.Username.Should().Be("agentuser");
        response.AuthenticationToken.Should().Be("agent-token");
    }

    [Fact]
    public async Task AgentLogin_InvalidPassword_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var sut = CreateSut(out var databaseService, out var hashService, out var _, out var agentTokenService);

        var request = new AgentLoginRequest
        {
            MeterId = "meter-1",
            Username = "agentuser",
            Password = "wrong-password"
        };

        var storedCredentials = new MeterAgentCredentials
        {
            Id = "cred-1",
            MeterId = "meter-1",
            Username = "agentuser",
            HashedPassword = "correct-hash"
        };

        databaseService
            .Get<MeterAgentCredentials>(Arg.Any<Dictionary<string, string>>(), true)
            .Returns(storedCredentials);

        hashService
            .GetHash("wrong-password")
            .Returns("wrong-hash");

        // Act
        var act = async () => await sut.AgentLogin(request);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
        await databaseService.DidNotReceive().GetFullMeterAgentFromId(Arg.Any<string>());
        agentTokenService.DidNotReceive().GetAgentToken(Arg.Any<FullMeterAgent>());
    }

    [Fact]
    public async Task ClientLogin_ValidCredentials_ReturnsResponseWithToken()
    {
        // Arrange
        var sut = CreateSut(out var databaseService, out var hashService, out var jwtService, out var _);

        var request = new ClientLoginRequest
        {
            Username = "clientuser",
            Password = "password123"
        };

        var storedCredentials = new ClientCredentials
        {
            Id = "cred-1",
            ClientId = "client-1",
            Username = "clientuser",
            HashedPassword = "hashed-password"
        };

        var client = new Client
        {
            Id = "client-1",
            Name = "Client One",
            MeterIds = ["m1", "m2"]
        };

        databaseService
            .Get<ClientCredentials>(Arg.Any<Dictionary<string, string>>(), true)
            .Returns(storedCredentials);

        hashService
            .GetHash("password123")
            .Returns("hashed-password");

        databaseService
            .Get<Client>(Arg.Any<Dictionary<string, string>>(), true)
            .Returns(client);

        jwtService
            .GetClientJwt(client)
            .Returns("client-jwt");

        // Act
        var response = await sut.ClientLogin(request);

        // Assert
        response.Username.Should().Be("clientuser");
        response.AuthenticationToken.Should().Be("client-jwt");
    }

    [Fact]
    public async Task ClientLogin_InvalidPassword_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var sut = CreateSut(out var databaseService, out var hashService, out var jwtService, out var _);

        var request = new ClientLoginRequest
        {
            Username = "clientuser",
            Password = "wrong-password"
        };

        var storedCredentials = new ClientCredentials
        {
            Id = "cred-1",
            ClientId = "client-1",
            Username = "clientuser",
            HashedPassword = "correct-hash"
        };

        databaseService
            .Get<ClientCredentials>(Arg.Any<Dictionary<string, string>>(), true)
            .Returns(storedCredentials);

        hashService
            .GetHash("wrong-password")
            .Returns("wrong-hash");

        // Act
        var act = async () => await sut.ClientLogin(request);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
        await databaseService.DidNotReceive().Get<Client>(Arg.Any<Dictionary<string, string>>(), true);
        jwtService.DidNotReceive().GetClientJwt(Arg.Any<Client>());
    }

    [Fact]
    public void TryGetBearerToken_NullOrEmpty_ReturnsNull()
    {
        // Arrange
        var sut = CreateSut(out var _, out var _, out var _, out var _);

        // Act
        var fromNull = sut.TryGetBearerToken(null);
        var fromEmpty = sut.TryGetBearerToken(string.Empty);

        // Assert
        fromNull.Should().BeNull();
        fromEmpty.Should().BeNull();
    }

    [Fact]
    public void TryGetBearerToken_InvalidPrefix_ReturnsNull()
    {
        // Arrange
        var sut = CreateSut(out var _, out var _, out var _, out var _);

        // Act
        var result = sut.TryGetBearerToken("Token abc123");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void TryGetBearerToken_ValidBearerHeader_ReturnsTokenPart()
    {
        // Arrange
        var sut = CreateSut(out var _, out var _, out var _, out var _);

        // Act
        var result = sut.TryGetBearerToken("Bearer abc123 ");

        // Assert
        result.Should().Be("abc123");
    }

    [Fact]
    public void IsTokenAuthorised_NullOrWhitespace_ReturnsFalse()
    {
        // Arrange
        var sut = CreateSut(out var _, out var _, out var jwtService, out var agentTokenService);

        // Act
        var resultNull = sut.IsTokenAuthorised(null);
        var resultEmpty = sut.IsTokenAuthorised(" ");

        // Assert
        resultNull.Should().BeFalse();
        resultEmpty.Should().BeFalse();
        jwtService.DidNotReceive().IsValidJwt(Arg.Any<string>());
        agentTokenService.DidNotReceive().IsAgentTokenValid(Arg.Any<string>());
    }

    [Fact]
    public void IsTokenAuthorised_ReturnsTrueIfJwtValid()
    {
        // Arrange
        var sut = CreateSut(out var _, out var _, out var jwtService, out var agentTokenService);
        jwtService.IsValidJwt("jwt-token").Returns(true);
        agentTokenService.IsAgentTokenValid("jwt-token").Returns(false);

        // Act
        var result = sut.IsTokenAuthorised("jwt-token");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsTokenAuthorised_ReturnsTrueIfAgentTokenValid()
    {
        // Arrange
        var sut = CreateSut(out var _, out var _, out var jwtService, out var agentTokenService);
        jwtService.IsValidJwt("agent-token").Returns(false);
        agentTokenService.IsAgentTokenValid("agent-token").Returns(true);

        // Act
        var result = sut.IsTokenAuthorised("agent-token");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsTokenAuthorised_ReturnsFalseIfBothInvalid()
    {
        // Arrange
        var sut = CreateSut(out var _, out var _, out var jwtService, out var agentTokenService);
        jwtService.IsValidJwt("token").Returns(false);
        agentTokenService.IsAgentTokenValid("token").Returns(false);

        // Act
        var result = sut.IsTokenAuthorised("token");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsAuthorised_WithValidBearerHeader_ReturnsTrue()
    {
        // Arrange
        var sut = CreateSut(out var _, out var _, out var jwtService, out var agentTokenService);
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Authorization = "Bearer valid-token";
        jwtService.IsValidJwt("valid-token").Returns(true);
        agentTokenService.IsAgentTokenValid("valid-token").Returns(false);

        // Act
        var result = sut.IsAuthorised(httpContext);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsAuthorised_WithNoAuthorizationHeader_ReturnsFalse()
    {
        // Arrange
        var sut = CreateSut(out var _, out var _, out var _, out var _);
        var httpContext = new DefaultHttpContext();

        // Act
        var result = sut.IsAuthorised(httpContext);

        // Assert
        result.Should().BeFalse();
    }
}
