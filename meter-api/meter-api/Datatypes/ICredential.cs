namespace meter_api.Datatypes
{
    public interface ICredential
    {
        string Username { get; set; }
        string HashedPassword { get; set; }
    }
}
