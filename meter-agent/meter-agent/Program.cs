using DotNetEnv;
using meter_agent.Datatypes;
using meter_agent.Datatypes.Requests;
using meter_agent.DataTypes.Exceptions;
using meter_agent.Services;
using System.Threading.Tasks;

namespace meter_agent
{
    public static class Program
    {
        private static readonly IUsageService usageService = new UsageService();
        private static IAuthenticationService authenticationService;
        private static readonly Random random = new();

        public static async Task Main(string[] args)
        {
            Env.Load();

            var credentials = new Credentials
            {
                MeterId = Environment.GetEnvironmentVariable("METER_ID") ?? throw new MissingCredentialException("METER_ID missing"),
                Username = Environment.GetEnvironmentVariable("USERNAME") ?? throw new MissingCredentialException("USERNAME missing"),
                Password = Environment.GetEnvironmentVariable("PASSWORD") ?? throw new MissingCredentialException("PASSWORD missing")
            };

            var baseUrl = Environment.GetEnvironmentVariable("BASE_URL") ?? throw new MissingCredentialException("BASE_URL missing");

            authenticationService = new AuthenticationService(baseUrl);

            var loginRequest = new AgentLoginRequest
            {
                MeterId = credentials.MeterId,
                Username = credentials.Username,
                Password = credentials.Password
            };

            var response = await authenticationService.Login(loginRequest);

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