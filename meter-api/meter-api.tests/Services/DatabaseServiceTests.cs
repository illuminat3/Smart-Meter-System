using Microsoft.Extensions.Options;

namespace meter_api.tests.Services;

public class DatabaseServiceTests
{
    private class UnsupportedDbObject : IDatabaseObject
    {
        public string Id { get; set; } = string.Empty;
    }

    [Fact]
    public async Task GetFullMeterAgentFromId_ReturnsCombinedDataFromTables()
    {
        // Arrange
        var databaseClient = Substitute.For<IDatabaseHttpClient>();
        var database = new Database();
        var options = Options.Create(new DatabaseOptions { ConnectionUrl = "http://test/" });
        using var semaphore = new SemaphoreSlim(1, 1);

        var meterId = "1";

        var agent = new MeterAgent
        {
            Id = meterId,
            DisplayName = "Main Meter",
            TotalUsage = 123.45m,
            TotalBilling = 67.89m
        };

        var reading = new MeterAgentReading
        {
            Id = "r1",
            MeterId = meterId,
            PreviousReadingId = "",
            TimestampUtc = DateTime.UtcNow,
            Usage = 10m,
            Billing = 2m
        };

        var credentials = new MeterAgentCredentials
        {
            Id = "c1",
            MeterId = meterId,
            Username = "meter-user",
            HashedPassword = "hashed"
        };

        database.MeterAgents.Add(agent);
        database.MeterAgentReadings.Add(reading);
        database.MeterAgentCredentials.Add(credentials);

        var sut = new DatabaseService(databaseClient, database, options, semaphore);

        // Act
        var result = await sut.GetFullMeterAgentFromId(meterId);

        // Assert
        result.Id.Should().Be(meterId);
        result.DisplayName.Should().Be(agent.DisplayName);
        result.TotalUsage.Should().Be(agent.TotalUsage);
        result.TotalBilling.Should().Be(agent.TotalBilling);
        result.Credentials.Should().BeEquivalentTo(credentials);
        result.Readings.Should().ContainSingle().Which.Id.Should().Be("r1");
    }

    [Fact]
    public async Task InitialiseDatabase_WhenNotInitialised_PopulatesTablesAndSetsFlag()
    {
        // Arrange
        var databaseClient = Substitute.For<IDatabaseHttpClient>();
        var database = new Database();
        var options = Options.Create(new DatabaseOptions { ConnectionUrl = "http://test/" });
        using var semaphore = new SemaphoreSlim(1, 1);

        var clients = new List<Client> { new() { Id = "1", Name = "Client 1" } };
        var clientCredentials = new List<ClientCredentials>
        {
            new() { Id = "cc1", ClientId = "1", Username = "clientuser", HashedPassword = "hash" }
        };
        var meterAgents = new List<MeterAgent>
        {
            new() { Id = "m1", DisplayName = "Meter 1", TotalUsage = 10m, TotalBilling = 2m }
        };
        var meterAgentCredentials = new List<MeterAgentCredentials>
        {
            new() { Id = "mac1", MeterId = "m1", Username = "meteruser", HashedPassword = "hash" }
        };
        var meterAgentReadings = new List<MeterAgentReading>
        {
            new() { Id = "r1", MeterId = "m1", PreviousReadingId = "", Usage = 5m, Billing = 1m }
        };

        databaseClient.GetListAsync<Client>(Arg.Any<string>()).Returns(clients);
        databaseClient.GetListAsync<ClientCredentials>(Arg.Any<string>()).Returns(clientCredentials);
        databaseClient.GetListAsync<MeterAgent>(Arg.Any<string>()).Returns(meterAgents);
        databaseClient.GetListAsync<MeterAgentCredentials>(Arg.Any<string>()).Returns(meterAgentCredentials);
        databaseClient.GetListAsync<MeterAgentReading>(Arg.Any<string>()).Returns(meterAgentReadings);

        var sut = new DatabaseService(databaseClient, database, options, semaphore);

        // Act
        await sut.InitialiseDatabase();

        // Assert
        database.IsInitialised.Should().BeTrue();
        database.Clients.Should().BeEquivalentTo(clients);
        database.ClientCredentials.Should().BeEquivalentTo(clientCredentials);
        database.MeterAgents.Should().BeEquivalentTo(meterAgents);
        database.MeterAgentCredentials.Should().BeEquivalentTo(meterAgentCredentials);
        database.MeterAgentReadings.Should().BeEquivalentTo(meterAgentReadings);
    }

    [Fact]
    public async Task InitialiseDatabase_WhenAlreadyInitialised_DoesNotCallHttpClient()
    {
        // Arrange
        var databaseClient = Substitute.For<IDatabaseHttpClient>();
        var database = new Database { IsInitialised = true };
        var options = Options.Create(new DatabaseOptions { ConnectionUrl = "http://test/" });
        using var semaphore = new SemaphoreSlim(1, 1);
        var sut = new DatabaseService(databaseClient, database, options, semaphore);

        // Act
        await sut.InitialiseDatabase();

        // Assert
        await databaseClient.DidNotReceive().GetListAsync<Client>(Arg.Any<string>());
        await databaseClient.DidNotReceive().GetListAsync<ClientCredentials>(Arg.Any<string>());
        await databaseClient.DidNotReceive().GetListAsync<MeterAgent>(Arg.Any<string>());
        await databaseClient.DidNotReceive().GetListAsync<MeterAgentCredentials>(Arg.Any<string>());
        await databaseClient.DidNotReceive().GetListAsync<MeterAgentReading>(Arg.Any<string>());
    }

    [Fact]
    public async Task Create_WhenIdIsEmpty_AssignsNextNumericIdAndAddsToTable()
    {
        // Arrange
        var databaseClient = Substitute.For<IDatabaseHttpClient>();
        var database = new Database
        {
            Clients =
            [
                new Client { Id = "1", Name = "Existing 1" },
                new Client { Id = "2", Name = "Existing 2" }
            ]
        };
        var options = Options.Create(new DatabaseOptions { ConnectionUrl = "http://test/" });
        using var semaphore = new SemaphoreSlim(1, 1);
        var sut = new DatabaseService(databaseClient, database, options, semaphore);

        var newClient = new Client { Name = "New Client" };

        // Act
        var created = await sut.Create(newClient);

        // Assert
        created.Id.Should().Be("3");
        database.Clients.Should().ContainSingle(c => c.Id == "3" && c.Name == "New Client");
    }

    [Fact]
    public async Task Create_WhenIdProvided_DoesNotChangeIdAndAddsToTable()
    {
        // Arrange
        var databaseClient = Substitute.For<IDatabaseHttpClient>();
        var database = new Database();
        var options = Options.Create(new DatabaseOptions { ConnectionUrl = "http://test/" });
        using var semaphore = new SemaphoreSlim(1, 1);
        var sut = new DatabaseService(databaseClient, database, options, semaphore);

        var newClient = new Client { Id = "10", Name = "Explicit Id Client" };

        // Act
        var created = await sut.Create(newClient);

        // Assert
        created.Id.Should().Be("10");
        database.Clients.Should().ContainSingle(c => c.Id == "10" && c.Name == "Explicit Id Client");
    }

    [Fact]
    public async Task Put_WhenEntityDoesNotExist_AddsToTable()
    {
        // Arrange
        var databaseClient = Substitute.For<IDatabaseHttpClient>();
        var database = new Database();
        var options = Options.Create(new DatabaseOptions { ConnectionUrl = "http://test/" });
        using var semaphore = new SemaphoreSlim(1, 1);
        var sut = new DatabaseService(databaseClient, database, options, semaphore);

        var client = new Client { Id = "1", Name = "Client 1" };

        // Act
        var result = await sut.Put(client);

        // Assert
        result.Should().Be(client);
        database.Clients.Should().ContainSingle(c => c.Id == "1" && c.Name == "Client 1");
    }

    [Fact]
    public async Task Put_WhenEntityExists_UpdatesExistingEntity()
    {
        // Arrange
        var databaseClient = Substitute.For<IDatabaseHttpClient>();
        var existing = new Client { Id = "1", Name = "Old Name" };
        var database = new Database
        {
            Clients = [existing]
        };
        var options = Options.Create(new DatabaseOptions { ConnectionUrl = "http://test/" });
        using var semaphore = new SemaphoreSlim(1, 1);
        var sut = new DatabaseService(databaseClient, database, options, semaphore);

        var updated = new Client { Id = "1", Name = "New Name" };

        // Act
        var result = await sut.Put(updated);

        // Assert
        result.Should().Be(updated);
        database.Clients.Should().ContainSingle(c => c.Id == "1" && c.Name == "New Name");
    }

    [Fact]
    public async Task Get_WhenMatchingEntityExists_ReturnsEntity()
    {
        // Arrange
        var databaseClient = Substitute.For<IDatabaseHttpClient>();
        var client = new Client { Id = "1", Name = "Client One" };
        var database = new Database
        {
            Clients = [client]
        };
        var options = Options.Create(new DatabaseOptions { ConnectionUrl = "http://test/" });
        using var semaphore = new SemaphoreSlim(1, 1);
        var sut = new DatabaseService(databaseClient, database, options, semaphore);

        var parameters = new Dictionary<string, string>
        {
            { "id", "1" }
        };

        // Act
        var result = await sut.Get<Client>(parameters);

        // Assert
        result.Should().Be(client);
    }

    [Fact]
    public async Task Get_WhenNoMatchingEntity_ThrowsKeyNotFoundException()
    {
        // Arrange
        var databaseClient = Substitute.For<IDatabaseHttpClient>();
        var database = new Database();
        var options = Options.Create(new DatabaseOptions { ConnectionUrl = "http://test/" });
        using var semaphore = new SemaphoreSlim(1, 1);
        var sut = new DatabaseService(databaseClient, database, options, semaphore);

        var parameters = new Dictionary<string, string>
        {
            { "id", "missing" }
        };

        // Act
        var act = async () => await sut.Get<Client>(parameters);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task GetCollection_WhenMatchingEntitiesExist_ReturnsAllMatches()
    {
        // Arrange
        var databaseClient = Substitute.For<IDatabaseHttpClient>();
        var client1 = new Client { Id = "1", Name = "Shared" };
        var client2 = new Client { Id = "2", Name = "Shared" };
        var client3 = new Client { Id = "3", Name = "Other" };
        var database = new Database
        {
            Clients = [client1, client2, client3]
        };
        var options = Options.Create(new DatabaseOptions { ConnectionUrl = "http://test/" });
        using var semaphore = new SemaphoreSlim(1, 1);
        var sut = new DatabaseService(databaseClient, database, options, semaphore);

        var parameters = new Dictionary<string, string>
        {
            { "name", "Shared" }
        };

        // Act
        var result = await sut.GetCollection<Client>(parameters);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(client1);
        result.Should().Contain(client2);
    }

    [Fact]
    public async Task GetCollection_WhenNoMatchingEntities_ThrowsKeyNotFoundException()
    {
        // Arrange
        var databaseClient = Substitute.For<IDatabaseHttpClient>();
        var database = new Database();
        var options = Options.Create(new DatabaseOptions { ConnectionUrl = "http://test/" });
        using var semaphore = new SemaphoreSlim(1, 1);
        var sut = new DatabaseService(databaseClient, database, options, semaphore);

        var parameters = new Dictionary<string, string>
        {
            { "name", "Missing" }
        };

        // Act
        var act = async () => await sut.GetCollection<Client>(parameters);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Create_WithUnsupportedType_ThrowsNotSupportedException()
    {
        // Arrange
        var databaseClient = Substitute.For<IDatabaseHttpClient>();
        var database = new Database();
        var options = Options.Create(new DatabaseOptions { ConnectionUrl = "http://test/" });
        using var semaphore = new SemaphoreSlim(1, 1);
        var sut = new DatabaseService(databaseClient, database, options, semaphore);

        var unsupported = new UnsupportedDbObject();

        // Act
        var act = async () => await sut.Create(unsupported);

        // Assert
        await act.Should().ThrowAsync<NotSupportedException>();
    }
}
