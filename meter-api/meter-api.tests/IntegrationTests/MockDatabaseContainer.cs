namespace meter_api.tests.IntegrationTests;

public class MockDatabaseContainer : IAsyncLifetime
{
    public IContainer Container { get; private set; } = default!;
    public string BaseUrl => $"http://localhost:{Container.GetMappedPublicPort(3000)}";

    public async Task InitializeAsync()
    {
        var jsonPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "IntegrationTests",
            "Data",
            "db.json"
        );

        if (!File.Exists(jsonPath))
        {
            throw new FileNotFoundException("db.json not found", jsonPath);
        }

        Container = new ContainerBuilder()
            .WithImage("ghcr.io/illuminat3/smart-meter-system-database:latest")
            .WithPortBinding(3000, assignRandomHostPort: true)
            .WithBindMount(jsonPath, "/src/db.json")
            .WithWaitStrategy(
                Wait.ForUnixContainer()
                    .UntilInternalTcpPortIsAvailable(3000)
            )
            .Build();

        await Container.StartAsync();
    }

    public async Task DisposeAsync()
    {
        if (Container != null)
        {
            await Container.DisposeAsync();
        }
    }
}