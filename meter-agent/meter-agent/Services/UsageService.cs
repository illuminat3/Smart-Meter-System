namespace meter_agent.Services;

public class UsageService : IUsageService
{
    private readonly Random _random = new();

    public decimal GetUsage()
    {
        var min = 0.001m;
        var max = 0.06m;

        var usage = (decimal)_random.NextDouble();
        usage = min + (usage * (max - min));

        return usage;
    }
}