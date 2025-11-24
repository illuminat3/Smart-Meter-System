namespace meter_agent.Services
{
    public class HealthCheckService(HttpClient httpClient) : IHealthCheckService
    {
        public async Task<bool> WaitForHealthyStatus(int maxRetries, int delaySeconds)
        {
            for (var attempt = 1; attempt <= maxRetries; attempt++)
            {
                var response = await httpClient.GetAsync("health");

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                if (attempt < maxRetries)
                {
                    await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
                }
            }

            return false;
        }
    }
}
