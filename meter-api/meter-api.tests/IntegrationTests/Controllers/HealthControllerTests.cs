using System.Net;

namespace meter_api.tests.IntegrationTests.Controllers;

[Collection("IntegrationTests")]
public class HealthControllerTests(MockDatabaseContainer db, MeterApiApplicationFactory factory) : IClassFixture<MockDatabaseContainer>, IClassFixture<MeterApiApplicationFactory>
{
    private HttpClient CreateClient()
    {
        Environment.SetEnvironmentVariable("DATABASE__CONNECTIONURL", db.BaseUrl + "/");
        return factory.CreateClient();
    }

    [Fact]
    public async Task GetHealthStatus_ReturnsHealthyStatus()
    {
        // Arrange
        using var client = CreateClient();

        // Act
        var response = await client.GetAsync("/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<HealthStatusResponse>();
        body.Should().NotBeNull();
        body!.Status.Should().Be("Healthy");
    }
}
