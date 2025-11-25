using meter_api.Datatypes.Database;

namespace meter_api.Services;

public interface IBillingService
{
    decimal CalculateCost(MeterAgentReading currentReading, MeterAgentReading? previousReading);
}
