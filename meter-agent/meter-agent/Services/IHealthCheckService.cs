namespace meter_agent.Services;

public interface IHealthCheckService
{
    Task<bool> WaitForHealthyStatus(int maxRetries, int delaySeconds);
}
