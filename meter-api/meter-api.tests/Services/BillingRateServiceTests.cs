namespace meter_api.tests.Services;

public class BillingRateServiceTests
{
    private readonly BillingRateService _sut = new();

    [Fact]
    public void GetRate_DuringPeakHours_ReturnsPeakRate()
    {
        // Arrange
        var timestamp = new DateTime(2025, 1, 1, 10, 0, 0, DateTimeKind.Utc);

        // Act
        var rate = _sut.GetRate(timestamp);

        // Assert
        rate.Should().Be(0.2635m);
    }

    [Fact]
    public void GetRate_OutsidePeakHours_ReturnsOffPeakRate()
    {
        // Arrange
        var timestamp = new DateTime(2025, 1, 1, 20, 0, 0, DateTimeKind.Utc);

        // Act
        var rate = _sut.GetRate(timestamp);

        // Assert
        rate.Should().Be(0.22m);
    }

    [Fact]
    public void GetRate_AtBoundaryHours_UsesCorrectRates()
    {
        // Arrange
        var atNine = new DateTime(2025, 1, 1, 9, 0, 0, DateTimeKind.Utc);
        var atSeventeen = new DateTime(2025, 1, 1, 17, 0, 0, DateTimeKind.Utc);

        // Act
        var rateAtNine = _sut.GetRate(atNine);
        var rateAtSeventeen = _sut.GetRate(atSeventeen);

        // Assert
        rateAtNine.Should().Be(0.2635m);
        rateAtSeventeen.Should().Be(0.22m);
    }
}
