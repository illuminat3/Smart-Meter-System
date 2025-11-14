namespace meter_api.Services
{
    public interface IDatabaseHttpClient
    {
        Task<List<T>?> GetListAsync<T>(string url);
    }
}