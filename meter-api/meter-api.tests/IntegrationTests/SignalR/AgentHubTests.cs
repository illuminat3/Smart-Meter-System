using meter_api.Datatypes.Messages.Agent;

namespace meter_api.tests.IntegrationTests.SignalR;

[Collection("IntegrationTests")]
public class AgentHubTests(MockDatabaseContainer db, MeterApiApplicationFactory factory) : IClassFixture<MockDatabaseContainer>, IClassFixture<MeterApiApplicationFactory>
{
    private HttpClient CreateClient()
    {
        Environment.SetEnvironmentVariable("DATABASE__CONNECTIONURL", db.BaseUrl + "/");
        return factory.CreateClient();
    }

    private async Task<string> GetAgentTokenAsync()
    {
        var httpClient = CreateClient();

        var loginRequest = new AgentLoginRequest
        {
            MeterId = "1",
            Username = "username_agent_1",
            Password = "password_agent_1"
        };

        var loginResponse = await httpClient.PostAsJsonAsync("/Auth/agent/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();

        var loginBody = await loginResponse.Content.ReadFromJsonAsync<AgentLoginResponse>();
        loginBody.Should().NotBeNull();
        loginBody!.AuthenticationToken.Should().NotBeNullOrWhiteSpace();

        return loginBody.AuthenticationToken!;
    }

    private HubConnection CreateConnection(HttpClient httpClient, string token)
    {
        var connection = new HubConnectionBuilder()
            .WithUrl(new Uri(httpClient.BaseAddress!, "/hub/agents"), options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(token)!;
                options.HttpMessageHandlerFactory = _ => factory.Server.CreateHandler();
            })
            .WithAutomaticReconnect()
            .Build();

        return connection;
    }

    [Fact]
    public async Task AgentHub_WithValidAgentToken_Connects()
    {
        // Arrange
        var httpClient = CreateClient();
        var token = await GetAgentTokenAsync();
        var connection = CreateConnection(httpClient, token);

        // Act
        await connection.StartAsync();

        // Assert
        connection.State.Should().Be(HubConnectionState.Connected);

        await connection.StopAsync();
        await connection.DisposeAsync();
    }

    [Fact]
    public async Task AgentHub_WithValidAgentToken_Disconnects()
    {
        // Arrange
        var httpClient = CreateClient();
        var token = await GetAgentTokenAsync();
        var connection = CreateConnection(httpClient, token);

        // Act
        await connection.StartAsync();
        connection.State.Should().Be(HubConnectionState.Connected);
        await connection.StopAsync();

        // Assert
        connection.State.Should().Be(HubConnectionState.Disconnected);

        await connection.DisposeAsync();
    }

    [Fact]
    public async Task AgentHub_WithValidAgentToken_CanHandle_AgentErrorUpdate_Message()
    {
        // Arrange
        var httpClient = CreateClient();
        var token = await GetAgentTokenAsync();
        var connection = CreateConnection(httpClient, token);
        await connection.StartAsync();

        var message = new AgentErrorUpdateMessage
        {
            Body = new AgentError
            {
                ErrorMessage = "Something went wrong"
            }
        };
        var rawMessage = JsonSerializer.Serialize(message);

        // Act
        Func<Task> act = async () => await connection.InvokeAsync("ReceiveMessage", rawMessage);

        // Assert
        await act.Should().NotThrowAsync();
        connection.State.Should().Be(HubConnectionState.Connected);

        await connection.StopAsync();
        await connection.DisposeAsync();
    }

    [Fact]
    public async Task AgentHub_WithValidAgentToken_CanHandle_AgentUsageUpdate_Message()
    {
        // Arrange
        var httpClient = CreateClient();
        var token = await GetAgentTokenAsync();
        var connection = CreateConnection(httpClient, token);
        await connection.StartAsync();

        var message = new AgentUsageUpdateMessage
        {
            Body = new AgentUsage
            {
                EnergyUsedKWh = 123.45m
            }
        };
        var rawMessage = JsonSerializer.Serialize(message);

        // Act
        Func<Task> act = async () => await connection.InvokeAsync("ReceiveMessage", rawMessage);

        // Assert
        await act.Should().NotThrowAsync();
        connection.State.Should().Be(HubConnectionState.Connected);

        await connection.StopAsync();
        await connection.DisposeAsync();
    }
}