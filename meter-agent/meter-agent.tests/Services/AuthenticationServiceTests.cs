namespace meter_agent.tests.Services;

public class AuthenticationServiceTests
{
    [Fact]
    public async Task Login_ShouldThrow_WhenResponseIsNotSuccessful()
    {
        // Arrange
        var handler = new FakeHttpMessageHandler(async _ =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                Content = new StringContent("{}", System.Text.Encoding.UTF8, "application/json")
            };

            return await Task.FromResult(response);
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://example.com/")
        };

        var service = new AuthenticationService(httpClient);

        AgentLoginRequest requestBody = default!;

        // Act
        var act = async () => await service.Login(requestBody);

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task Login_ShouldThrowInvalidOperationException_WhenResponseBodyDeserializesToNull()
    {
        // Arrange
        var handler = new FakeHttpMessageHandler(async _ =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("null", System.Text.Encoding.UTF8, "application/json")
            };

            return await Task.FromResult(response);
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://example.com/")
        };

        var service = new AuthenticationService(httpClient);

        AgentLoginRequest requestBody = default!;

        // Act
        var act = async () => await service.Login(requestBody);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Failed to deserialize login response.");
    }

    private sealed class FakeHttpMessageHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> handlerFunc) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return handlerFunc(request);
        }
    }
}