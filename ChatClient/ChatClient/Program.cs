using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using GrpcWpfSample.Client;

namespace ChatClient
{
    class Program
    {
        static void Main(string[] args)
        {
            // The port number(5001) must match the port of the gRPC server.

            //AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            //using var channel = GrpcChannel.ForAddress("http://localhost:5001");
            //var client = new Chat.ChatClient(channel);

            Console.Write("Enter your name> ");
            var name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name))
            {
                name = "anonymous";
            }

            Console.WriteLine($"Joined as {name}");

            var chatServiceClient = new ChatServiceClient();
            var consoleLock = new object();

            // subscribe (asynchronous)
            _ = chatServiceClient.ChatLogs()
                .ForEachAsync((x) =>
                {
                    // if the user is writing something, wait until it finishes.
                    lock (consoleLock)
                    {
                        Console.WriteLine($"{x.Time.ToDateTime().ToString("HH:mm:ss")} {x.Name}: {x.Content}");
                    }
                });

            // write
            while (true)
            {
                var key = Console.ReadKey();

                // A key input starts writing mode
                lock (consoleLock)
                {
                    var content = key.KeyChar + Console.ReadLine();

                    chatServiceClient.Write(new ChatLog
                    {
                        Name = name,
                        Content = content,
                        Time = Timestamp.FromDateTime(DateTime.Now.ToUniversalTime()),
                    }).Wait();
                }
            }

        }
    }


    }
