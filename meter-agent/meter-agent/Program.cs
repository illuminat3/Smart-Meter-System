using DotNetEnv;
using meter_agent.Datatypes;
using meter_agent.Services;

namespace meter_agent
{
    public static class Program
    {
        private static IUsageService usageService = new UsageService();

        public static void Main(string[] args)
        {
            Env.Load();

            var credentials = new Credentials
            {
                MeterId = Environment.GetEnvironmentVariable("METER_ID") ?? throw new Exception("METER_ID missing"),
                Username = Environment.GetEnvironmentVariable("USERNAME") ?? throw new Exception("USERNAME missing"),
                Password = Environment.GetEnvironmentVariable("PASSWORD") ?? throw new Exception("PASSWORD missing")
            };
        }
    }
}