namespace meter_api.tests.IntegrationTests;

public class MeterApiApplicationFactory : WebApplicationFactory<Program>
{
    public MeterApiApplicationFactory()
    {
        Environment.SetEnvironmentVariable("JWT__SECRET", "test_secret_12345678901234567890");
        Environment.SetEnvironmentVariable("JWT__ISSUER", "test-issuer");
        Environment.SetEnvironmentVariable("JWT__AUDIENCE", "test-audience");
        Environment.SetEnvironmentVariable("JWT__EXPIRY", "60");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
    }
}
