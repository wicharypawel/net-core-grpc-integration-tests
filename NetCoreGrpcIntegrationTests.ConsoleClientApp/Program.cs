using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using NetCoreGrpcIntegrationTests.Proto;

namespace NetCoreGrpcIntegrationTests.ConsoleClientApp
{
    public class Program
    {
        public static async Task Main()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);
            Console.WriteLine("Type your name, confirm with enter...");
            var user = Console.ReadLine();
            var response = await client.SayHelloAsync(new HelloRequest { Name = user });
            Console.WriteLine(response.Message);
            await channel.ShutdownAsync();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
