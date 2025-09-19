namespace meter_api.Services
{
    public interface IJwtService
    {
        string GetJwt(string key); // Change to user object
    }
}
