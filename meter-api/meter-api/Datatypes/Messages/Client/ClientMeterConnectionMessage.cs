namespace meter_api.Datatypes.Messages.Client
{
    public class ClientMeterConnectionMessage : IMessage<ConnectionMessage>
    {
        public string MessageName { get; } = "ClientMeterConnection";

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public required ConnectionMessage Body { get; set; }
    }
}
