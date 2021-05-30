using ChatServer.Server.Model;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServer.Server.Services
{
    public class ChatService : Chat.ChatBase
    {
        [Export(typeof(IService))]
        public class ChatServiceGrpcServer : Chat.ChatBase, IService
        {
            [Import]
            private Logger m_logger = null;

            [Import]
            private ChatServiceModel m_chatService = null;
            private readonly Empty m_empty = new Empty();
            [Import]
            private UsersServiceModel m_usersService=null;

            private const int Port = 50052;
            private readonly Grpc.Core.Server m_server;

            public ChatServiceGrpcServer()
            {

                // Locate required files and set true to enable SSL
                var secure = false;

                if (secure)
                {
                    // secure
                    var clientCACert = File.ReadAllText(@"C:\localhost_client.crt");
                    var serverCert = File.ReadAllText(@"C:\localhost_server.crt");
                    var serverKey = File.ReadAllText(@"C:\localhost_serverkey.pem");
                    var keyPair = new KeyCertificatePair(serverCert, serverKey);
                    var credentials = new SslServerCredentials(new[] { keyPair }, clientCACert, SslClientCertificateRequestType.RequestAndRequireAndVerify);

                    // Client authentication is an option. You can remove it as follows if you only need SSL.
                    //var credentials = new SslServerCredentials(new[] { keyPair });

                    m_server = new Grpc.Core.Server
                    {
                        Services =
                    {
                        Chat.BindService(this)
                            .Intercept(new IpAddressAuthenticator())
                    },
                        Ports =
                    {
                        new ServerPort("localhost", Port, credentials)
                    }
                    };
                }
                else
                {
                    // insecure
                    m_server = new Grpc.Core.Server
                    {
                        Services =
                    {
                        Chat.BindService(this)
                            .Intercept(new IpAddressAuthenticator())
                    },
                        Ports =
                    {
                        new ServerPort("localhost", Port, ServerCredentials.Insecure)
                    }
                    };
                }
            }

            public void Start()
            {
                m_server.Start();

                m_logger.Info("Started.");
            }

            public override async Task Subscribe(Username request, IServerStreamWriter<ChatLog> responseStream, ServerCallContext context) //
            {
                var peer = context.Peer; 
                m_logger.Info($"{request} subscribes."); //modif
                m_usersService.Add(request);

                context.CancellationToken.Register(() => m_logger.Info($"{request} cancels subscription.")); //modif
                context.CancellationToken.Register(() => m_usersService.Delete(request));

                // Completing the method means disconnecting the stream by server side.
                // If subscribing IObservable, you have to block this method after the subscription.
                // I prefer converting IObservable to IAsyncEnumerable to consume the sequense here
                // because gRPC interface is in IAsyncEnumerable world.
                // Note that the chat service model itself is in IObservable world
                // because chat is naturally recognized as an event sequence.

                try
                {
                    await m_chatService.GetChatLogsAsObservable()
                        .ToAsyncEnumerable()
                        .ForEachAwaitAsync(async (x) => await responseStream.WriteAsync(x), context.CancellationToken)
                        .ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                    m_logger.Info($"{peer} unsubscribed.");
                }
            }


            public override async Task GetUsers(Empty request, IServerStreamWriter<Username> responseStream, ServerCallContext context) //
            {
               
                if(m_usersService!=null)
                try
                {
                    await m_usersService.GetUsersAsObservable()
                        .ToAsyncEnumerable()
                        .ForEachAwaitAsync(async (x) => await responseStream.WriteAsync(x))
                        .ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                   
                }
            }

            public override Task<Empty> Write(ChatLog request, ServerCallContext context)
            {
                m_logger.Info($"{context.Peer} {request}");

                m_chatService.Add(request);

                return Task.FromResult(m_empty);
            }
        }
    }
}
