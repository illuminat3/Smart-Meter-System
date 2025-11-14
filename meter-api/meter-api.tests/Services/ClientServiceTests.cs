using meter_api.Datatypes.Messages.Client;
using meter_api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace meter_api.tests.Services
{
    public class ClientServiceTests
    {
        private static ClientService CreateSut(
            out ISnapshotService snapshotService,
            out IHubContext<ClientHub> hubContext,
            out IHubClients clients,
            out IClientProxy clientProxy)
        {
            snapshotService = Substitute.For<ISnapshotService>();
            hubContext = Substitute.For<IHubContext<ClientHub>>();
            clients = Substitute.For<IHubClients>();
            clientProxy = Substitute.For<IClientProxy>();

            hubContext.Clients.Returns(clients);

            return new ClientService(snapshotService, hubContext);
        }

        private static bool MatchesUpdateArgs(object?[] args, string meterId, string displayName)
        {
            if (args == null || args.Length == 0) return false;

            if (args[0] is not ClientUpdateMessage message) return false;

            if (message.Body is not MeterSnapshot snapshot) return false;

            return snapshot.MeterId == meterId &&
                   snapshot.DisplayName == displayName;
        }

        private static bool MatchesErrorUpdateArgs(object?[] args, string meterId, string errorMessage)
        {
            if (args == null || args.Length == 0) return false;

            if (args[0] is not ClientUpdateMessage message) return false;

            if (message.Body is not MeterSnapshot snapshot) return false;

            return snapshot.MeterId == meterId &&
                   snapshot.ErrorMessage == errorMessage;
        }

        [Fact]
        public async Task MeterAgentUpdate_SendsClientUpdateMessageWithSnapshot()
        {
            // Arrange
            var sut = CreateSut(out var snapshotService, out var hubContext, out var clients, out var clientProxy);
            var meterId = "meter-1";

            var snapshot = new MeterSnapshot
            {
                MeterId = meterId,
                DisplayName = "Test Meter",
                CurrentUsage = 1m,
                TotalUsage = 10m,
                TotalCost = 2m
            };

            snapshotService.GetMeterSnapshot(meterId).Returns(snapshot);
            clients.Group($"meter:{meterId}").Returns(clientProxy);

            // Act
            await sut.MeterAgentUpdate(meterId);

            // Assert
            await clientProxy.Received(1).SendCoreAsync(
                Arg.Any<string>(),
                Arg.Is<object?[]>(args => MatchesUpdateArgs(args, meterId, "Test Meter")),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task MeterAgentErrorUpdate_SendsClientUpdateMessageWithErrorInSnapshot()
        {
            // Arrange
            var sut = CreateSut(out var snapshotService, out var hubContext, out var clients, out var clientProxy);
            var meterId = "meter-2";

            var snapshot = new MeterSnapshot
            {
                MeterId = meterId,
                DisplayName = "Meter With Error",
                CurrentUsage = 0m,
                TotalUsage = 5m,
                TotalCost = 1m,
                ErrorMessage = string.Empty
            };

            var error = new AgentError
            {
                ErrorMessage = "Something went wrong"
            };

            snapshotService.GetMeterSnapshot(meterId).Returns(snapshot);
            clients.Group($"meter:{meterId}").Returns(clientProxy);

            // Act
            await sut.MeterAgentErrorUpdate(meterId, error);

            // Assert
            await clientProxy.Received(1).SendCoreAsync(
                Arg.Any<string>(),
                Arg.Is<object?[]>(args => MatchesErrorUpdateArgs(args, meterId, "Something went wrong")),
                Arg.Any<CancellationToken>());
        }
    }
}
