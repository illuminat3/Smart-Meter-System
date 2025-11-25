namespace meter_api.Services;

public class BillingRateService : IBillingRateService
{
    public decimal GetRate(DateTime timestampUtc)
    {
        var hour = timestampUtc.Hour;

        return hour >= 9 && hour < 17 ? 0.2635m : 0.22m;
    }
}
