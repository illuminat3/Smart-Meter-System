using System.Data;

namespace meter_api.tests.UnitTests.Services;

public class SnapshotServiceTests
{
    private static SnapshotService CreateSut(
        out IDatabaseService databaseService,
        out IMeterAgentService meterAgentService)
    {
        databaseService = Substitute.For<IDatabaseService>();
        meterAgentService = Substitute.For<IMeterAgentService>();
        return new SnapshotService(databaseService, meterAgentService);
    }

    private static readonly MeterAgentCredentials credentials = new()
    {
        MeterId = "client-1",
        Username = "username",
        HashedPassword = "hashed-password"
    };

    [Fact]
    public async Task GetMeterSnapshot_WhenConnectedAndHasPreviousReading_ReturnsOpenSnapshotWithCurrentUsage()
    {
        // Arrange
        var sut = CreateSut(out var databaseService, out var meterAgentService);
        var meterId = "meter-1";

        var previousReading = new MeterAgentReading
        {
            Id = "r1",
            MeterId = meterId,
            PreviousReadingId = "",
            TimestampUtc = DateTime.UtcNow,
            Usage = 12.5m,
            Billing = 3m
        };

        var fullMeterAgent = new FullMeterAgent
        {
            Id = meterId,
            DisplayName = "Test Meter",
            Credentials = credentials,
            Readings = [previousReading],
            TotalUsage = 100m,
            TotalBilling = 25m
        };

        databaseService.GetFullMeterAgentFromId(meterId).Returns(fullMeterAgent);
        meterAgentService.IsMeterAgentConnected(meterId).Returns(true);

        // Act
        var snapshot = await sut.GetMeterSnapshot(meterId);

        // Assert
        snapshot.MeterId.Should().Be(meterId);
        snapshot.DisplayName.Should().Be("Test Meter");
        snapshot.ConnectionState.Should().Be(ConnectionState.Open);
        snapshot.CurrentUsage.Should().Be(12.5m);
        snapshot.TotalUsage.Should().Be(100m);
        snapshot.TotalCost.Should().Be(25m);
    }

    [Fact]
    public async Task GetMeterSnapshot_WhenNotConnectedAndNoPreviousReading_UsesZeroUsageAndClosedState()
    {
        // Arrange
        var sut = CreateSut(out var databaseService, out var meterAgentService);
        var meterId = "meter-2";

        var fullMeterAgent = new FullMeterAgent
        {
            Id = meterId,
            DisplayName = "No Previous",
            Credentials = credentials,
            Readings = [],
            TotalUsage = 50m,
            TotalBilling = 10m
        };

        databaseService.GetFullMeterAgentFromId(meterId).Returns(fullMeterAgent);
        meterAgentService.IsMeterAgentConnected(meterId).Returns(false);

        // Act
        var snapshot = await sut.GetMeterSnapshot(meterId);

        // Assert
        snapshot.MeterId.Should().Be(meterId);
        snapshot.ConnectionState.Should().Be(ConnectionState.Closed);
        snapshot.CurrentUsage.Should().Be(0m);
        snapshot.TotalUsage.Should().Be(50m);
        snapshot.TotalCost.Should().Be(10m);
    }

    [Fact]
    public async Task GetMeterSnapshotsForClient_WhenClientHasNoMeters_ReturnsEmptyList()
    {
        // Arrange
        var sut = CreateSut(out var databaseService, out var meterAgentService);
        var clientId = "client-1";

        var client = new Client
        {
            Id = clientId,
            Name = "No Meters",
            MeterIds = []
        };

        databaseService
            .Get<Client>(Arg.Any<Dictionary<string, string>>(), true)
            .Returns(client);

        // Act
        var snapshots = await sut.GetMeterSnapshotsForClient(clientId);

        // Assert
        snapshots.Should().BeEmpty();
        await databaseService.DidNotReceive().GetFullMeterAgentFromId(Arg.Any<string>());
        meterAgentService.DidNotReceive().IsMeterAgentConnected(Arg.Any<string>());
    }

    [Fact]
    public async Task GetMeterSnapshotsForClient_WhenClientHasMeters_ReturnsSnapshotForEachMeter()
    {
        // Arrange
        var sut = CreateSut(out var databaseService, out var meterAgentService);
        var clientId = "client-2";

        var client = new Client
        {
            Id = clientId,
            Name = "Has Meters",
            MeterIds = ["m1", "m2"]
        };

        databaseService
            .Get<Client>(Arg.Any<Dictionary<string, string>>(), true)
            .Returns(client);

        var agent1 = new FullMeterAgent
        {
            Id = "m1",
            DisplayName = "Meter 1",
            Credentials = credentials,
            Readings = [],
            TotalUsage = 10m,
            TotalBilling = 2m
        };

        var agent2 = new FullMeterAgent
        {
            Id = "m2",
            DisplayName = "Meter 2",
            Credentials = credentials,
            Readings = [],
            TotalUsage = 20m,
            TotalBilling = 4m
        };

        databaseService.GetFullMeterAgentFromId("m1").Returns(agent1);
        databaseService.GetFullMeterAgentFromId("m2").Returns(agent2);

        meterAgentService.IsMeterAgentConnected("m1").Returns(true);
        meterAgentService.IsMeterAgentConnected("m2").Returns(false);

        // Act
        var snapshots = await sut.GetMeterSnapshotsForClient(clientId);

        // Assert
        snapshots.Should().HaveCount(2);

        var snap1 = snapshots.Single(s => s.MeterId == "m1");
        snap1.DisplayName.Should().Be("Meter 1");
        snap1.ConnectionState.Should().Be(ConnectionState.Open);
        snap1.TotalUsage.Should().Be(10m);
        snap1.TotalCost.Should().Be(2m);

        var snap2 = snapshots.Single(s => s.MeterId == "m2");
        snap2.DisplayName.Should().Be("Meter 2");
        snap2.ConnectionState.Should().Be(ConnectionState.Closed);
        snap2.TotalUsage.Should().Be(20m);
        snap2.TotalCost.Should().Be(4m);
    }
}
