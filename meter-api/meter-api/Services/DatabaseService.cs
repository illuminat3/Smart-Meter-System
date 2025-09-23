using meter_api.Datatypes;
using meter_api.Datatypes.Database;

namespace meter_api.Services
{
    public class DatabaseService : IDatabaseService
    {
        public Task<MeterAgent> GetAgentFromUsername(string username)
        {
            throw new NotImplementedException();
        }

        public Task<Client> GetClientFromUsername(string username)
        {
            throw new NotImplementedException();
        }

        public Task<ICredential> GetCredentialsFromUsername(string username)
        {
            throw new NotImplementedException();
        }
    }
}
