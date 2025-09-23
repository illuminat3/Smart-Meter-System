using meter_api.Datatypes.Database;

namespace meter_api.Services
{
    public interface IJwtService
    {
        string GetClientJwt(Client client);
        bool IsValidJwt(string jwt);
    }
}
