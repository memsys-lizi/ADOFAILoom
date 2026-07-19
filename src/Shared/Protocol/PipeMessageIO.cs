using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ADOFAILoom.Protocol
{
    public static class PipeMessageIO
    {
        public static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        public static async Task<T> ReadAsync<T>(
            Stream stream,
            CancellationToken cancellationToken)
        {
            using (var buffer = new MemoryStream())
            {
                var singleByte = new byte[1];
                while (true)
                {
                    int count = await stream
                        .ReadAsync(singleByte, 0, 1, cancellationToken)
                        .ConfigureAwait(false);
                    if (count == 0)
                    {
                        throw new EndOfStreamException("The pipe closed before a complete message was received.");
                    }

                    if (singleByte[0] == (byte)'\n')
                    {
                        break;
                    }

                    if (buffer.Length >= BridgeProtocol.MaxMessageBytes)
                    {
                        throw new InvalidDataException("The pipe message exceeds the 64 KiB limit.");
                    }

                    buffer.WriteByte(singleByte[0]);
                }

                if (buffer.Length == 0)
                {
                    throw new InvalidDataException("The pipe message is empty.");
                }

                string json = Encoding.UTF8.GetString(buffer.ToArray());
                T? message = JsonSerializer.Deserialize<T>(json, JsonOptions);
                return message ?? throw new InvalidDataException("The pipe message contains JSON null.");
            }
        }

        public static async Task WriteAsync<T>(
            Stream stream,
            T message,
            CancellationToken cancellationToken)
        {
            byte[] json = JsonSerializer.SerializeToUtf8Bytes(message, JsonOptions);
            if (json.Length > BridgeProtocol.MaxMessageBytes)
            {
                throw new InvalidDataException("The pipe message exceeds the 64 KiB limit.");
            }

            await stream.WriteAsync(json, 0, json.Length, cancellationToken)
                .ConfigureAwait(false);
            await stream.WriteAsync(new[] { (byte)'\n' }, 0, 1, cancellationToken)
                .ConfigureAwait(false);
            await stream.FlushAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
