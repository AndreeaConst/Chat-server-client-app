
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ChatClientConsole;
using System.Threading.Tasks;
using System.Threading.Channels;

namespace ChatClientConsole
{
    public class ChatServiceClient
    {
        private readonly Chat.ChatClient m_client;

        public ChatServiceClient()
        {
            var secure = false;

            if (secure)
            {
                var serverCACert = File.ReadAllText(@"C:\localhost_server.crt");
                var clientCert = File.ReadAllText(@"C:\localhost_client.crt");
                var clientKey = File.ReadAllText(@"C:\localhost_clientkey.pem");
                var keyPair = new KeyCertificatePair(clientCert, clientKey);
                var credentials = new SslCredentials(serverCACert, keyPair);

                m_client = new Chat.ChatClient(
                    new Grpc.Core.Channel("localhost", 50052, credentials));
            }
            else
            {
                m_client = new Chat.ChatClient(
                    new Grpc.Core.Channel("localhost", 50052, ChannelCredentials.Insecure));
            }
        }

        public async Task Write(ChatLog chatLog)
        {
            await m_client.WriteAsync(chatLog);
        }

        public IAsyncEnumerable<ChatLog> ChatLogs(Username username)
        {
            var callUserName = m_client.Subscribe(username);
            return callUserName.ResponseStream
                .ToAsyncEnumerable()
                .Finally(() => callUserName.Dispose());
        }

        public IAsyncEnumerable<Username> Users()
        {
            var callUsers = m_client.GetUsers(new Empty());

            return callUsers.ResponseStream
                .ToAsyncEnumerable()
                .Finally(() => callUsers.Dispose());
        }
    }
}
