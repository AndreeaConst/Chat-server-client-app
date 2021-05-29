﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using Grpc.Net.Client;

namespace ChatClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // The port number(5001) must match the port of the gRPC server.

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            using var channel = GrpcChannel.ForAddress("http://localhost:5001");
            var client = new Chat.ChatClient(channel);

            /*var reply = await client.SayHelloAsync(
                              new HelloRequest { Name = "GreeterClient" });

            Console.WriteLine("Greeting: " + reply.Message);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();*/
        }
    }
}