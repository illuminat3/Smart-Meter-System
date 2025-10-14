using meter_api.Datatypes.Messages;
using meter_api.Datatypes.Messages.Agent;
using meter_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;
using System.Text.Json;

namespace meter_api.Hubs
{
    [Authorize]
    [SignalRHub]
    public class AgentHub(IMeterAgentService meterAgentService) : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var meterIds = Context.User?.FindAll("meterId").Select(c => c.Value) ?? [];

            if (!meterIds.Any())
            {
                Context.Abort();
                return;
            }

            foreach (var meterId in meterIds)
            {
                meterAgentService.AgentConnected(meterId, Context.ConnectionId);
                await Groups.AddToGroupAsync(Context.ConnectionId, $"meter:{meterId}");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var meterIds = Context.User?.FindAll("meterId").Select(c => c.Value) ?? [];
            foreach (var meterId in meterIds)
            {
                meterAgentService.AgentDisconnected(meterId, Context.ConnectionId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"meter:{meterId}");
            }

            await base.OnDisconnectedAsync(exception);
        }

        [HubMethodName("ReceiveMessage")]
        public async Task ReceiveMessage(string rawMessage)
        {
            using var doc = JsonDocument.Parse(rawMessage);
            var messageName = doc.RootElement.GetProperty("MessageName").GetString();

            IMessage<object>? message = null;

            switch (messageName)
            {
                case "AgentErrorUpdate":
                    message = JsonSerializer.Deserialize<AgentErrorUpdateMessage>(rawMessage) as IMessage<object>;
                    if (message is AgentErrorUpdateMessage errorUpdate)
                    {
                        var meterIds = Context.User?.FindAll("meterId").Select(c => c.Value) ?? [];
                        foreach (var meterId in meterIds)
                            meterAgentService.HandleErrorUpdate(meterId, errorUpdate.Body);
                    }
                    break;

                case "AgentUsageUpdate":
                    message = JsonSerializer.Deserialize<AgentUsageUpdateMessage>(rawMessage) as IMessage<object>;
                    if (message is AgentUsageUpdateMessage usageUpdate)
                    {
                        var meterIds = Context.User?.FindAll("meterId").Select(c => c.Value) ?? [];
                        foreach (var meterId in meterIds)
                            meterAgentService.HandleUsageUpdate(meterId, usageUpdate.Body);
                    }
                    break;

                default:
                    break;
            }
        }

    }
}
