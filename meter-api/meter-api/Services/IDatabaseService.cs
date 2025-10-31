using meter_api.Datatypes;

namespace meter_api.Services
{
    public interface IDatabaseService
    {
        Task<FullMeterAgent> GetFullMeterAgentFromId(string id);
        Task<T> Create<T>(T entity);
        Task<T> Update<T>(string id, T entity);
        Task<T> Get<T>(Dictionary<string, string> paramValue);
        Task<List<T>> GetCollection<T>(Dictionary<string, string> paramValue);
    }
}