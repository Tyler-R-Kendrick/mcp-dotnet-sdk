using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abstractions.Models;

namespace Server
{
    public class Server
    {
        private ClientCapabilities _clientCapabilities;
        private Implementation _clientVersion;
        private ServerCapabilities _capabilities;
        private string _instructions;

        public event Action OnInitialized;

        public Server(Implementation serverInfo, ServerOptions options = null)
        {
            _capabilities = options?.Capabilities ?? new ServerCapabilities();
            _instructions = options?.Instructions;
            SetRequestHandler(InitializeRequestSchema, OnInitialize);
            SetNotificationHandler(InitializedNotificationSchema, () => OnInitialized?.Invoke());
        }

        public void RegisterCapabilities(ServerCapabilities capabilities)
        {
            if (Transport != null)
            {
                throw new InvalidOperationException("Cannot register capabilities after connecting to transport");
            }

            _capabilities = MergeCapabilities(_capabilities, capabilities);
        }

        private async Task<InitializeResult> OnInitialize(InitializeRequest request)
        {
            var requestedVersion = request.Params.ProtocolVersion;

            _clientCapabilities = request.Params.Capabilities;
            _clientVersion = request.Params.ClientInfo;

            return new InitializeResult
            {
                ProtocolVersion = SupportedProtocolVersions.Contains(requestedVersion) ? requestedVersion : LatestProtocolVersion,
                Capabilities = GetCapabilities(),
                ServerInfo = _serverInfo,
                Instructions = _instructions
            };
        }

        public ClientCapabilities GetClientCapabilities() => _clientCapabilities;

        public Implementation GetClientVersion() => _clientVersion;

        private ServerCapabilities GetCapabilities() => _capabilities;

        public async Task Ping() => await Request(new { Method = "ping" }, EmptyResultSchema);

        public async Task CreateMessage(CreateMessageRequest.Params @params, RequestOptions options = null) => await Request(new { Method = "sampling/createMessage", Params = @params }, CreateMessageResultSchema, options);

        public async Task ListRoots(ListRootsRequest.Params @params = null, RequestOptions options = null) => await Request(new { Method = "roots/list", Params = @params }, ListRootsResultSchema, options);

        public async Task SendLoggingMessage(LoggingMessageNotification.Params @params) => await Notification(new { Method = "notifications/message", Params = @params });

        public async Task SendResourceUpdated(ResourceUpdatedNotification.Params @params) => await Notification(new { Method = "notifications/resources/updated", Params = @params });

        public async Task SendResourceListChanged() => await Notification(new { Method = "notifications/resources/list_changed" });

        public async Task SendToolListChanged() => await Notification(new { Method = "notifications/tools/list_changed" });

        public async Task SendPromptListChanged() => await Notification(new { Method = "notifications/prompts/list_changed" });
    }
}
