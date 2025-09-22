using meter_api.Datatypes.Database;

namespace meter_api.Services
{
    public interface IBillingService
    {
        float CalculateCost(MeterAgentReading currentReading, MeterAgentReading? previousReading);
    }
}
