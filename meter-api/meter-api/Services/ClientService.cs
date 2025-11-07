
using meter_api.Datatypes.Messages.Client;
using meter_api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace meter_api.Services
{
    public class ClientService(ISnapshotService snapshotService, IHubContext<ClientHub> clientHub) : IClientService
    {
        public async Task MeterAgentUpdate(string meterId)
        {
            var meterSnapshot = await snapshotService.GetMeterSnapshot(meterId);
            var message = new ClientUpdateMessage
            {
                Body = meterSnapshot
            };

            await clientHub.Clients.Group($"meter:{meterId}").SendAsync(message.MessageName, message);
        }
    }
}
