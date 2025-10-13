using meter_api.Datatypes.Messages;
using meter_api.Datatypes.Messages.Client;
using meter_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace meter_api.Hubs
{
    [Authorize]
    [SignalRHub]
    public class ClientHub(ISnapshotService snapshotService) : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var clientId = GetClientId();

            if (clientId == null)
            {
                Context.Abort();
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, $"client:{clientId}");

            var meterIds = Context.User?.FindAll("meterId").Select(c => c.Value) ?? [];
            foreach (var meterId in meterIds)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"meter:{meterId}");
            }

            var message = new ClientInitialStateMessage
            {
                Body = await snapshotService.GetMeterSnapshotsForClient(clientId)
            };

            await Clients.Caller.SendAsync(message.MessageName, message); 

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var clientId = GetClientId();

            if (clientId == null)
            {
                Context.Abort();
                return;
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"client:{clientId}");

            await base.OnDisconnectedAsync(exception);
        }

        private string? GetClientId()
        {
            return Context.User?.FindFirst("id")?.Value;
        }
    }
}
