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
    [SignalRHub("hub/agents")]
    public class AgentHub(IMeterAgentService meterAgentService) : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var meterIds = Context.User?.FindAll("agent_id").Select(c => c.Value) ?? [];

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
            var meterIds = Context.User?.FindAll("agent_id").Select(c => c.Value) ?? [];
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

            switch (messageName)
            {
                case "AgentErrorUpdate":
                    var errorUpdate = JsonSerializer.Deserialize<AgentErrorUpdateMessage>(rawMessage);
                    if (errorUpdate is not null)
                    {
                        var meterIds = Context.User?.FindAll("agent_id").Select(c => c.Value) ?? [];
                        foreach (var meterId in meterIds)
                            await meterAgentService.HandleErrorUpdate(meterId, errorUpdate.Body);
                    }
                    break;

                case "AgentUsageUpdate":
                    var usageUpdate = JsonSerializer.Deserialize<AgentUsageUpdateMessage>(rawMessage);
                    if (usageUpdate is not null)
                    {
                        var meterIds = Context.User?.FindAll("agent_id").Select(c => c.Value) ?? [];
                        foreach (var meterId in meterIds)
                            await meterAgentService.HandleUsageUpdate(meterId, usageUpdate.Body);
                    }
                    break;

                default:
                    break;
            }
        }
    }
}
