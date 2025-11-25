using System.Net;

namespace meter_api.tests.IntegrationTests.Controllers;

[Collection("IntegrationTests")]
public class AuthControllerTests(MockDatabaseContainer db, MeterApiApplicationFactory factory) : IClassFixture<MockDatabaseContainer>, IClassFixture<MeterApiApplicationFactory>
{
    private HttpClient CreateClient()
    {
        Environment.SetEnvironmentVariable("DATABASE__CONNECTIONURL", db.BaseUrl + "/");
        return factory.CreateClient();
    }

    [Fact]
    public async Task ClientLogin_WithValidCredentials_ReturnsOkAndToken()
    {
        // Arrange
        using var client = CreateClient();
        var request = new ClientLoginRequest
        {
            Username = "Client1",
            Password = "password_client_1"
        };

        // Act
        var response = await client.PostAsJsonAsync("/Auth/client/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<ClientLoginResponse>();
        body.Should().NotBeNull();
        body!.Username.Should().Be("Client1");
        body.AuthenticationToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task ClientLogin_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        using var client = CreateClient();
        var request = new ClientLoginRequest
        {
            Username = "Client1",
            Password = "wrong_password"
        };

        // Act
        var response = await client.PostAsJsonAsync("/Auth/client/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AgentLogin_WithValidCredentials_ReturnsOkAndToken()
    {
        // Arrange
        using var client = CreateClient();
        var request = new AgentLoginRequest
        {
            MeterId = "1",
            Username = "username_agent_1",
            Password = "password_agent_1"
        };

        // Act
        var response = await client.PostAsJsonAsync("/Auth/agent/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<AgentLoginResponse>();
        body.Should().NotBeNull();
        body!.MeterId.Should().Be("1");
        body.Username.Should().Be("username_agent_1");
        body.AuthenticationToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task AgentLogin_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        using var client = CreateClient();
        var request = new AgentLoginRequest
        {
            MeterId = "1",
            Username = "username_agent_1",
            Password = "wrong_password"
        };

        // Act
        var response = await client.PostAsJsonAsync("/Auth/agent/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
