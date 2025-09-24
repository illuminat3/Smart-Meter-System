namespace meter_api.Services
{
    public interface IBillingRateService
    {
        decimal GetRate(DateTime timestampUtc);
    }
}
