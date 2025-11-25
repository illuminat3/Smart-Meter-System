namespace meter_agent.tests.Services;

public class UsageServiceTests
{
    [Fact]
    public void GetUsage_ShouldReturnValueWithinExpectedRange()
    {
        // Arrange
        var service = new UsageService();
        const decimal min = 0.001m;
        const decimal max = 0.06m;

        // Act
        var usages = Enumerable.Range(0, 1000)
            .Select(_ => service.GetUsage())
            .ToList();

        // Assert
        usages.Should().AllSatisfy(value =>
        {
            value.Should().BeGreaterThanOrEqualTo(min);
            value.Should().BeLessThanOrEqualTo(max);
        });
    }
}