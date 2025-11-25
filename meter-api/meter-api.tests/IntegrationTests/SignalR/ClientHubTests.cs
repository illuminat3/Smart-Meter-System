namespace meter_api.tests.IntegrationTests.SignalR;

public class ClientHubTests(MockDatabaseContainer db, MeterApiApplicationFactory factory) : IClassFixture<MockDatabaseContainer>, IClassFixture<MeterApiApplicationFactory>
{
    private HttpClient CreateClient()
    {
        GC.KeepAlive(db);
        Environment.SetEnvironmentVariable("DATABASE__CONNECTIONURL", MockDatabaseContainer.BaseUrl + "/");
        return factory.CreateClient();
    }

    [Fact]
    public async Task ClientHub_WithValidClientToken_Connects()
    {
        // Arrange
        var httpClient = CreateClient();
        var loginRequest = new ClientLoginRequest
        {
            Username = "Client1",
            Password = "password_client_1"
        };

        var loginResponse = await httpClient.PostAsJsonAsync("/Auth/client/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();

        var loginBody = await loginResponse.Content.ReadFromJsonAsync<ClientLoginResponse>();
        loginBody.Should().NotBeNull();
        loginBody!.AuthenticationToken.Should().NotBeNullOrWhiteSpace();

        var token = loginBody.AuthenticationToken;

        var connection = new HubConnectionBuilder()
            .WithUrl(new Uri(httpClient.BaseAddress!, "/hub/clients"), options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(token)!;
                options.HttpMessageHandlerFactory = _ => factory.Server.CreateHandler();
            })
            .WithAutomaticReconnect()
            .Build();

        // Act
        await connection.StartAsync();

        // Assert
        connection.State.Should().Be(HubConnectionState.Connected);

        await connection.StopAsync();
        await connection.DisposeAsync();
    }

    [Fact]
    public async Task ClientHub_WithValidClientToken_Disconnects()
    {
        // Arrange
        var httpClient = CreateClient();

        var loginRequest = new ClientLoginRequest
        {
            Username = "Client1",
            Password = "password_client_1"
        };

        var loginResponse = await httpClient.PostAsJsonAsync("/Auth/client/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();

        var loginBody = await loginResponse.Content.ReadFromJsonAsync<ClientLoginResponse>();
        loginBody.Should().NotBeNull();
        loginBody!.AuthenticationToken.Should().NotBeNullOrWhiteSpace();

        var token = loginBody.AuthenticationToken;

        var connection = new HubConnectionBuilder()
            .WithUrl(new Uri(httpClient.BaseAddress!, "/hub/clients"), options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(token)!;
                options.HttpMessageHandlerFactory = _ => factory.Server.CreateHandler();
            })
            .WithAutomaticReconnect()
            .Build();

        await connection.StartAsync();
        connection.State.Should().Be(HubConnectionState.Connected);

        // Act
        await connection.StopAsync();
        await Task.Delay(100);

        // Assert
        connection.State.Should().Be(HubConnectionState.Disconnected);

        await connection.DisposeAsync();
    }
}