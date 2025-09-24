using meter_api.Datatypes.Database;

namespace meter_api.Services
{
    public class BillingService(IBillingRateService billingRateService) : IBillingService
    {
        public decimal CalculateCost(MeterAgentReading currentReading, MeterAgentReading? previousReading)
        {
            if (previousReading == null || currentReading.TimestampUtc <= previousReading.TimestampUtc)
            {
                return 0.0m;
            }

            var usageTime = currentReading.TimestampUtc - previousReading.TimestampUtc;
            var usageSeconds = usageTime.TotalSeconds;

            if (usageSeconds <= 0)
            {
                return 0.0m;
            }

            var usageRate = currentReading.Usage / usageSeconds;

            var totalCost = billingRateService.GetRate(currentReading.TimestampUtc) * (decimal)usageRate;

            return totalCost;
        }
    }
}
