using meter_api.Datatypes;
using meter_api.Datatypes.Database;
using meter_api.Utils;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Text;

namespace meter_api.Services
{
    public class DatabaseService(DatabaseHttpClient databaseClient, Database database, IOptions<DatabaseOptions> options) : IDatabaseService
    {
        private readonly DatabaseOptions _databaseOptions = options.Value;
        private readonly SemaphoreSlim semaphoreSlim = new(1, 1);


        public async Task<FullMeterAgent> GetFullMeterAgentFromId(string id)
        {
            var meterAgent = await Get<MeterAgent>(new Dictionary<string, string> { { "id", id } });
            var readings = await GetCollection<MeterAgentReading>(new Dictionary<string, string> { { "meterId", id } });
            var credentials = await Get<MeterAgentCredentials>(new Dictionary<string, string> { { "meterId", id } });

            var fullMeterAgent = new FullMeterAgent
            {
                Id = meterAgent.Id,
                DisplayName = meterAgent.DisplayName,
                Credentials = credentials,
                Readings = readings,
                TotalUsage = meterAgent.TotalUsage,
                TotalBilling = meterAgent.TotalBilling
            };

            return fullMeterAgent;
        }

        public async Task InitialiseDatabase()
        {
            if (database.IsInitialised) return;

            await InitialiseTable<Client>();
            await InitialiseTable<ClientCredentials>();
            await InitialiseTable<MeterAgent>();
            await InitialiseTable<MeterAgentCredentials>();
            await InitialiseTable<MeterAgentReading>();

            database.IsInitialised = true;
        }

        #region Generic Methods

        public async Task<T> Create<T>(T entity) where T : IDatabaseObject
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                var table = GetTable<T>();

                if (string.IsNullOrWhiteSpace(entity.Id))
                {
                    var nextNumeric = table
                        .Select(x => int.TryParse(x.Id, out var n) ? n : 0)
                        .DefaultIfEmpty(0)
                        .Max() + 1;

                    entity.Id = nextNumeric.ToString();
                }

                return Update(entity.Id, entity);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public T Update<T>(string id, T entity) where T : IDatabaseObject
        {
            var table = GetTable<T>();

            var existing = table.FirstOrDefault(item => item.Id == id);

            if (existing != null)
            {
                var index = table.IndexOf(existing);
                table[index] = entity;
            }
            else
            {
                table.Add(entity);
            }

            return entity;
        }

        public async Task<T> Get<T>(Dictionary<string, string> paramValue) where T : IDatabaseObject
        {
            var table = GetTable<T>();

            var entity = await Task.Run(() =>
                table.FirstOrDefault(item => MatchesProperties(item, paramValue))
            );
            return entity ?? throw new KeyNotFoundException($"{typeof(T).Name} collection with specified parameters not found.");
        }

        public async Task<List<T>> GetCollection<T>(Dictionary<string, string> paramValue) where T : IDatabaseObject
        {
            var table = GetTable<T>();

            var entities = await Task.Run(() =>
                table.Where(item => MatchesProperties(item, paramValue)).ToList()
            );

            if (entities.Count == 0)
                throw new KeyNotFoundException($"{typeof(T).Name} collection with specified parameters not found.");

            return entities;
        }

        private async Task InitialiseTable<T>() where T : IDatabaseObject
        {
            var property = typeof(Database).GetProperties()
                .FirstOrDefault(p => p.PropertyType == typeof(List<T>));

            if (property == null)
                throw new NotSupportedException($"No property found for type {typeof(T).Name} in database.");

            var url = $"{_databaseOptions.ConnectionUrl}{typeof(T).Name.ToCamelCase()}";
            var list = await databaseClient.GetListAsync<T>(url) ?? new List<T>();

            property.SetValue(database, list);
        }

        public List<T> GetTable<T>() where T : IDatabaseObject => typeof(T).Name switch
        {
            nameof(Client) => database.Clients as List<T>,
            nameof(ClientCredentials) => database.ClientCredentials as List<T>,
            nameof(MeterAgent) => database.MeterAgents as List<T>,
            nameof(MeterAgentCredentials) => database.MeterAgentCredentials as List<T>,
            nameof(MeterAgentReading) => database.MeterAgentReadings as List<T>,
            _ => throw new NotSupportedException($"No resource path configured for type {typeof(T).Name}.")
        } ?? throw new InvalidOperationException($"Failed to resolve table for type {typeof(T).Name}.");

        private static bool MatchesProperties<T>(T item, Dictionary<string, string> parameters)
        {
            foreach (var kv in parameters)
            {
                var prop = typeof(T).GetProperty(
                    kv.Key,
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance
                );

                if (prop == null)
                    return false;

                var value = prop.GetValue(item);
                if (value == null)
                    return false;

                if (!string.Equals(value.ToString(), kv.Value, StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            return true;
        }
        #endregion 
    }
}
