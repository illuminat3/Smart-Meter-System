namespace meter_api.tests.Services
{
    public class MeterAgentServiceTests
    {
        private static MeterAgentService CreateSut(
            out IDatabaseService databaseService,
            out IBillingService billingService)
        {
            databaseService = Substitute.For<IDatabaseService>();
            billingService = Substitute.For<IBillingService>();
            return new MeterAgentService(databaseService, billingService);
        }

        private readonly static MeterAgentCredentials credentials = new()
        {
            MeterId = "client-1",
            Username = "username",
            HashedPassword = "hashed-password"
        };

        [Fact]
        public void AgentConnectedAndDisconnected_UpdatesConnectionState()
        {
            // Arrange
            var sut = CreateSut(out var _, out var _);
            var meterId = "meter-1";

            // Act
            var beforeConnect = sut.IsMeterAgentConnected(meterId);
            sut.AgentConnected(meterId, "conn-1");
            var afterConnect = sut.IsMeterAgentConnected(meterId);
            sut.AgentDisconnected(meterId, "conn-1");
            var afterDisconnect = sut.IsMeterAgentConnected(meterId);

            // Assert
            beforeConnect.Should().BeFalse();
            afterConnect.Should().BeTrue();
            afterDisconnect.Should().BeFalse();
        }

        [Fact]
        public void AgentConnected_MultipleConnections_OnlyBecomesDisconnectedWhenAllRemoved()
        {
            // Arrange
            var sut = CreateSut(out var _, out var _);
            var meterId = "meter-1";

            // Act
            sut.AgentConnected(meterId, "conn-1");
            sut.AgentConnected(meterId, "conn-2");
            var whileBothConnected = sut.IsMeterAgentConnected(meterId);
            sut.AgentDisconnected(meterId, "conn-1");
            var afterOneDisconnect = sut.IsMeterAgentConnected(meterId);
            sut.AgentDisconnected(meterId, "conn-2");
            var afterAllDisconnect = sut.IsMeterAgentConnected(meterId);

            // Assert
            whileBothConnected.Should().BeTrue();
            afterOneDisconnect.Should().BeTrue();
            afterAllDisconnect.Should().BeFalse();
        }

        [Fact]
        public async Task HandleUsageUpdate_CreatesReadingAndUpdatesAgentTotals()
        {
            // Arrange
            var sut = CreateSut(out var databaseService, out var billingService);
            var meterId = "meter-1";

            var previousReading = new MeterAgentReading
            {
                Id = "r0",
                MeterId = meterId,
                PreviousReadingId = "",
                TimestampUtc = DateTime.UtcNow.AddMinutes(-10),
                Usage = 5m,
                Billing = 1m
            };

            var fullAgent = new FullMeterAgent
            {
                Id = meterId,
                DisplayName = "Test Meter",
                Credentials = credentials,
                Readings = [previousReading],
                TotalUsage = 5m,
                TotalBilling = 1m
            };

            var agentUsage = new AgentUsage
            {
                EnergyUsedKWh = 3m
            };

            databaseService.GetFullMeterAgentFromId(meterId).Returns(fullAgent);

            MeterAgentReading? createdReading = null;
            databaseService
                .Create(Arg.Any<MeterAgentReading>(), true)
                .Returns(ci =>
                {
                    createdReading = ci.Arg<MeterAgentReading>();
                    return createdReading;
                });

            billingService
                .CalculateCost(Arg.Any<MeterAgentReading>(), previousReading)
                .Returns(2m);

            databaseService
                .GetCollection<MeterAgentReading>(Arg.Any<Dictionary<string, string>>(), true)
                .Returns(ci =>
                [
                    previousReading,
                    createdReading!
                ]);

            var meterAgent = new MeterAgent
            {
                Id = meterId,
                DisplayName = "Test Meter",
                TotalUsage = 0m,
                TotalBilling = 0m
            };

            MeterAgent? updatedAgent = null;
            databaseService
                .Get<MeterAgent>(Arg.Any<Dictionary<string, string>>(), true)
                .Returns(meterAgent);

            databaseService
                .Put<MeterAgent>(Arg.Any<MeterAgent>(), true)
                .Returns(ci =>
                {
                    updatedAgent = ci.Arg<MeterAgent>();
                    return updatedAgent;
                });

            // Act
            await sut.HandleUsageUpdate(meterId, agentUsage);

            // Assert
            createdReading.Should().NotBeNull();
            createdReading!.MeterId.Should().Be(meterId);
            createdReading.PreviousReadingId.Should().Be(previousReading.Id);
            createdReading.Usage.Should().Be(3m);
            createdReading.Billing.Should().Be(2m);

            billingService.Received(1)
                .CalculateCost(Arg.Is<MeterAgentReading>(r => r == createdReading), previousReading);

            updatedAgent.Should().NotBeNull();
            updatedAgent!.TotalUsage.Should().Be(previousReading.Usage + createdReading.Usage);
            updatedAgent.TotalBilling.Should().Be(previousReading.Billing + createdReading.Billing);
        }
    }
}
