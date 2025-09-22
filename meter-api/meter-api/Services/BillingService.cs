using meter_api.Datatypes.Database;

namespace meter_api.Services
{
    public class BillingService : IBillingService
    {
        public float CalculateCost(MeterAgentReading currentReading, MeterAgentReading? previousReading)
        {
            throw new NotImplementedException();
        }
    }
}
