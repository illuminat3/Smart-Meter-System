namespace meter_api.Services
{
    public interface IClientService
    {
        Task MeterAgentUpdate(string meterId);
    }
}
