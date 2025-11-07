using meter_api.Datatypes;
using meter_api.Datatypes.Database;

namespace meter_api.Services
{
    public interface IDatabaseService
    {
        Task InitialiseDatabase();
        Task<FullMeterAgent> GetFullMeterAgentFromId(string id);
        Task<T> Create<T>(T entity) where T : IDatabaseObject; 
        Task<T> Update<T>(string id, T entity) where T : IDatabaseObject;
        Task<T> Get<T>(Dictionary<string, string> paramValue) where T : IDatabaseObject;
        Task<List<T>> GetCollection<T>(Dictionary<string, string> paramValue) where T : IDatabaseObject;
    }
}