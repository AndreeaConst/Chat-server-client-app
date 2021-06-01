using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using ChatClientConsole;
using ChatClient;


namespace ChatClient
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.Write("Enter your name> ");
            var name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name))
            {
                name = "anonymous";
            }

            Console.WriteLine($"Joined as {name}");

            var chatServiceClient = new ChatServiceClient();
            var consoleLock = new object();

            _ = chatServiceClient.ChatLogs(new Username
            {
                Name = name,
            })
                .ForEachAsync((x) =>
                {
                    lock (consoleLock)
                    {
                        Console.WriteLine($"{x.Time.ToDateTime().ToString("HH:mm:ss")} {x.Name}: {x.Content}");
                    }
                });
            while (true)
            {
                var key = Console.ReadKey();

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
