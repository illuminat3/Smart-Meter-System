namespace meter_agent.Services
{
    public class UsageService : IUsageService
    {
        private readonly Random _random = new();

        public decimal GetUsage()
        {
            decimal min = 0.001m;
            decimal max = 0.06m;

            decimal usage = (decimal)_random.NextDouble();
            usage = min + (usage * (max - min));

            return usage;
        }
    }
}