namespace meter_api.tests.Services
{
    public class BillingServiceTests
    {
        [Fact]
        public void CalculateCost_PreviousReadingNull_ReturnsZeroAndDoesNotCallRateService()
        {
            // Arrange
            var billingRateService = Substitute.For<IBillingRateService>();
            var sut = new BillingService(billingRateService);

            var current = new MeterAgentReading
            {
                MeterId = "meter-1",
                PreviousReadingId = string.Empty,
                TimestampUtc = DateTime.UtcNow,
                Usage = 100m
            };

            // Act
            var cost = sut.CalculateCost(current, previousReading: null);

            // Assert
            cost.Should().Be(0.0m);
            billingRateService.Received(0).GetRate(Arg.Any<DateTime>());
        }

        [Fact]
        public void CalculateCost_CurrentTimestampNotAfterPrevious_ReturnsZeroAndDoesNotCallRateService()
        {
            // Arrange
            var billingRateService = Substitute.For<IBillingRateService>();
            var sut = new BillingService(billingRateService);

            var timestamp = new DateTime(2025, 1, 1, 10, 0, 0, DateTimeKind.Utc);

            var previous = new MeterAgentReading
            {
                MeterId = "meter-1",
                PreviousReadingId = string.Empty,
                TimestampUtc = timestamp,
                Usage = 100m
            };

            var current = new MeterAgentReading
            {
                MeterId = "meter-1",
                PreviousReadingId = previous.Id,
                TimestampUtc = timestamp,
                Usage = 150m
            };

            // Act
            var cost = sut.CalculateCost(current, previous);

            // Assert
            cost.Should().Be(0.0m);
            billingRateService.Received(0).GetRate(Arg.Any<DateTime>());
        }

        [Fact]
        public void CalculateCost_ValidReadings_ComputesCostUsingUsageAndRate()
        {
            // Arrange
            var billingRateService = Substitute.For<IBillingRateService>();
            var sut = new BillingService(billingRateService);

            var previousTime = new DateTime(2025, 1, 1, 10, 0, 0, DateTimeKind.Utc);
            var currentTime = previousTime.AddHours(1);

            var previous = new MeterAgentReading
            {
                MeterId = "meter-1",
                PreviousReadingId = string.Empty,
                TimestampUtc = previousTime,
                Usage = 0m
            };

            var current = new MeterAgentReading
            {
                MeterId = "meter-1",
                PreviousReadingId = previous.Id,
                TimestampUtc = currentTime,
                Usage = 3600m 
            };

            billingRateService
                .GetRate(currentTime)
                .Returns(0.5m);

            // Act
            var cost = sut.CalculateCost(current, previous);

            // Assert
            cost.Should().Be(0.5m);

            billingRateService
                .Received(1)
                .GetRate(currentTime);
        }
    }
}
