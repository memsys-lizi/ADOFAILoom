namespace ADOFAILoom.Protocol
{
    public sealed class BridgeRequest
    {
        public int ProtocolVersion { get; set; } = BridgeProtocol.Version;

        public string? RequestId { get; set; }

        public string? Method { get; set; }
    }

    public sealed class BridgeResponse
    {
        public int ProtocolVersion { get; set; } = BridgeProtocol.Version;

        public string? RequestId { get; set; }

        public GameState? Result { get; set; }

        public BridgeError? Error { get; set; }

        public static BridgeResponse Success(string requestId, GameState result)
        {
            return new BridgeResponse
            {
                RequestId = requestId,
                Result = result
            };
        }

        public static BridgeResponse Failure(string? requestId, string code, string message)
        {
            return new BridgeResponse
            {
                RequestId = requestId,
                Error = new BridgeError
                {
                    Code = code,
                    Message = message
                }
            };
        }
    }

    public sealed class BridgeError
    {
        public string? Code { get; set; }

        public string? Message { get; set; }
    }
}
