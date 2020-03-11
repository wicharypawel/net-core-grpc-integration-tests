using AutoFixture;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.TestHost;
using NetCoreGrpcIntegrationTests.AspNetCoreServerApp.Tests.App_Infrastructure.ClassFixture;
using NetCoreGrpcIntegrationTests.AspNetCoreServerApp.Tests.App_Infrastructure.Extensions;
using NetCoreGrpcIntegrationTests.Proto;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace NetCoreGrpcIntegrationTests.AspNetCoreServerApp.Tests
{
    public sealed class ExampleIntegrationTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly Fixture _fixture;
        private readonly GrpcChannel _channel;
        public ExampleIntegrationTests(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    // ConfigureServices of DI
                });
            }).CreateClientForGrpc();
            _channel = GrpcChannel.ForAddress("http://randomAddress:1234", new GrpcChannelOptions()
            {
                HttpClient = _client
            });
            _fixture = new Fixture();
        }

        [Trait("Category", "Integration")]
        [Fact]
        public async Task Given_RandomName_When_CallingSayHelloEndpoint_Return_GreetingMessage()
        {
            // Arrange
            var greeterClient = new Greeter.GreeterClient(_channel);
            var userName = _fixture.Create<string>();
            // Act
            var responseMessage = await greeterClient.SayHelloAsync(new HelloRequest() { Name = userName });
            // Assert
            Assert.Equal($"Hello {userName}", responseMessage.Message);
        }

        [Trait("Category", "Integration")]
        [Fact]
        public async Task Given_RandomName_When_CallingSayHelloServerStreamEndpoint_Return_GreetingInChunks()
        {
            // Arrange
            var greeterClient = new Greeter.GreeterClient(_channel);
            var userName = _fixture.Create<string>();
            // Act
            var responseMessage = greeterClient.SayHelloServerStream(new HelloRequest() { Name = userName });
            // Assert
            var expectedResult = $"Hello {userName}";
            var i = 0;
            while (await responseMessage.ResponseStream.MoveNext())
            {
                var value = responseMessage.ResponseStream.Current;
                Assert.Equal(expectedResult[i++].ToString(), value.Message);
            }
        }

        [Trait("Category", "Integration")]
        [Fact]
        public async Task Given_RandomNameInChunks_When_CallingSayHelloClientStreamEndpoint_Return_GreetingMessage()
        {
            // Arrange
            var greeterClient = new Greeter.GreeterClient(_channel);
            var userName = _fixture.Create<string>();
            // Act
            var clientStreamingCall = greeterClient.SayHelloClientStream();
            foreach (var nameChunk in userName)
            {
                await clientStreamingCall.RequestStream.WriteAsync(new HelloRequest() { Name = nameChunk.ToString() });
            }
            await clientStreamingCall.RequestStream.CompleteAsync();
            // Assert
            var responseMessage = await clientStreamingCall.ResponseAsync;
            Assert.Equal($"Hello {userName}", responseMessage.Message);
        }

        [Trait("Category", "Integration")]
        [Fact]
        public async Task Given_RandomNameInChunks_When_CallingSayHelloBiStreamEndpoint_Return_GreetingMessageInChunks()
        {
            // Arrange
            var greeterClient = new Greeter.GreeterClient(_channel);
            var userName = _fixture.Create<string>();
            // Act
            var call = greeterClient.SayHelloBiStream();
            foreach (var nameChunk in userName)
            {
                await call.RequestStream.WriteAsync(new HelloRequest() { Name = nameChunk.ToString() });
            }
            await call.RequestStream.CompleteAsync();
            // Assert
            var expectedResult = userName;
            var i = 0;
            while (await call.ResponseStream.MoveNext())
            {
                var value = call.ResponseStream.Current;
                Assert.Equal(expectedResult[i++].ToString(), value.Message);
            }
        }
    }
}
