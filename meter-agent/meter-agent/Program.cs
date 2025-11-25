using System.Globalization;
using DotNetEnv;
using meter_agent.Datatypes;
using meter_agent.Datatypes.Requests;
using meter_agent.DataTypes;
using meter_agent.DataTypes.Exceptions;
using meter_agent.DataTypes.Messages;
using meter_agent.Hubs;
using meter_agent.Services;

namespace meter_agent;

public static class Program
{
    private static readonly UsageService usageService = new();
    private static readonly Random random = new();

    public static async Task Main()
    {
        Env.Load();

        var credentials = new Credentials
        {
            MeterId = Environment.GetEnvironmentVariable("METER_ID") ?? throw new MissingCredentialException("METER_ID missing"),
            Username = Environment.GetEnvironmentVariable("USERNAME") ?? throw new MissingCredentialException("USERNAME missing"),
            Password = Environment.GetEnvironmentVariable("PASSWORD") ?? throw new MissingCredentialException("PASSWORD missing")
        };

        var baseUrl = Environment.GetEnvironmentVariable("BASE_URL") ?? throw new MissingArgumentException("BASE_URL missing");

        var errorChanceString = Environment.GetEnvironmentVariable("ERROR_CHANCE") ?? throw new MissingArgumentException("ERROR_CHANCE missing");

        if (!double.TryParse(errorChanceString, NumberStyles.Float, CultureInfo.InvariantCulture, out var errorChance))
        {
            throw new FormatException("ERROR_CHANCE must be a valid double");
        }

        if (errorChance < 0.0 || errorChance > 1.0)
        {
            throw new FormatException("ERROR_CHANCE must be between 0.0 and 1.0");
        }

        if (!baseUrl.EndsWith('/'))
        {
            baseUrl += "/";
        }

        var handler = new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(2),
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(5),
            EnableMultipleHttp2Connections = true
        };

        var sharedClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl),
            Timeout = TimeSpan.FromSeconds(100)
        };

        var healthCheckService = new HealthCheckService(sharedClient);
        var healthy = await healthCheckService.WaitForHealthyStatus(10, 3);

        if (!healthy)
        {
            Environment.Exit(1);
        }

        var authenticationService = new AuthenticationService(sharedClient);

        var loginRequest = new AgentLoginRequest
        {
            MeterId = credentials.MeterId,
            Username = credentials.Username,
            Password = credentials.Password
        };

        var hubUrl = $"{baseUrl}hub/agents";

        var agentHubClient = new AgentHubClient(authenticationService, loginRequest, hubUrl);

        try
        {
            await agentHubClient.ConnectAsync();

            while (true)
            {
                var shouldSendError = random.NextDouble() < errorChance;

                if (shouldSendError)
                {
                    var errorMessage = "Meter Agent failed to get usage";
                    Console.WriteLine($"Error: {errorMessage}");

                    await agentHubClient.SendMessageAsync(new AgentErrorUpdateMessage
                    {
                        Body = new AgentError
                        {
                            ErrorMessage = errorMessage
                        }
                    });
                }
                else
                {
                    var usage = usageService.GetUsage();
                    Console.WriteLine($"Usage: {usage} kWh");

                    await agentHubClient.SendMessageAsync(new AgentUsageUpdateMessage
                    {
                        Body = new AgentUsage
                        {
                            EnergyUsedKWh = usage
                        }
                    });
                }

                var delaySeconds = random.Next(15, 61);
                Console.WriteLine($"Waiting {delaySeconds} seconds");
                await Task.Delay(delaySeconds * 1000);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fatal error: {ex.Message}");
            try
            {
                await agentHubClient.SendMessageAsync(new AgentErrorUpdateMessage
                {
                    Body = new AgentError
                    {
                        ErrorMessage = ex.Message
                    }
                });
            }
            catch (Exception newEx) 
            {
                Console.WriteLine(newEx.Message);
            }
        }
        finally
        {
            await agentHubClient.DisconnectAsync();
        }
    }
}
