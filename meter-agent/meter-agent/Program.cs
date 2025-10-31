using DotNetEnv;
using meter_agent.Datatypes;
using meter_agent.DataTypes.Exceptions;
using meter_agent.Services;

namespace meter_agent
{
    public static class Program
    {
        private static readonly IUsageService usageService = new UsageService();
        private static readonly Random random = new();

        public static void Main(string[] args)
        {
            Env.Load();

            var credentials = new Credentials
            {
                MeterId = Environment.GetEnvironmentVariable("METER_ID") ?? throw new MissingCredentialException("METER_ID missing"),
                Username = Environment.GetEnvironmentVariable("USERNAME") ?? throw new MissingCredentialException("USERNAME missing"),
                Password = Environment.GetEnvironmentVariable("PASSWORD") ?? throw new MissingCredentialException("PASSWORD missing")
            };
            };

            while (true)
            {
                var usage = usageService.GetUsage();
                Console.WriteLine($"Usage: {usage} kWh");

                int delaySeconds = random.Next(15, 61);
                Console.WriteLine($"Waiting {delaySeconds} seconds before next reading...\n");
                Thread.Sleep(delaySeconds * 1000);
            }
        }
    }
}