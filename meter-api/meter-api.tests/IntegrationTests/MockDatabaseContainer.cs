namespace meter_api.tests.IntegrationTests;

public class MockDatabaseContainer : IAsyncLifetime
{
    public IContainer Container { get; private set; } = default!;
    public static string BaseUrl => "http://localhost:3030";

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
            .WithName("mock-database-test")
            .WithPortBinding(3030, 3000)
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