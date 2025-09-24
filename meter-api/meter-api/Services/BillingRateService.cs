
namespace meter_api.Services
{
    public class BillingRateService : IBillingRateService
    {
        public decimal GetRate(DateTime timestampUtc)
        {
            var hour = timestampUtc.Hour;

            if (hour >= 9 && hour < 17)
            {
                return 0.2635m;
            }

            return 0.22m;
        }
    }
}
