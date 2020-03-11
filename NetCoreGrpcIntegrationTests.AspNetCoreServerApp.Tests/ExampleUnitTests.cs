using AutoFixture;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using NetCoreGrpcIntegrationTests.AspNetCoreServerApp.Services;
using NetCoreGrpcIntegrationTests.Proto;
using System.Threading.Tasks;
using Xunit;

namespace NetCoreGrpcIntegrationTests.AspNetCoreServerApp.Tests
{
    public sealed class ExampleUnitTests
    {
        private readonly Fixture _fixture;

        public ExampleUnitTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public async Task Given_RandomName_When_CallingSayHello_Return_GreetingMessage()
        {
            // Arrange
            var loggerMock = new Moq.Mock<ILogger<GreeterService>>();
            var contextMock = new Moq.Mock<ServerCallContext>();
            var service = new GreeterService(loggerMock.Object);
            var request = new HelloRequest() { Name = _fixture.Create<string>() };
            // Act
            var result = await service.SayHello(request, contextMock.Object);
            // Assert
            Assert.Equal($"Hello {request.Name}", result.Message);
        }

        [Fact]
        public async Task Given_EmptyName_When_CallingSayHello_Return_Error()
        {
            // Arrange
            var loggerMock = new Moq.Mock<ILogger<GreeterService>>();
            var contextMock = new Moq.Mock<ServerCallContext>();
            var service = new GreeterService(loggerMock.Object);
            var request = new HelloRequest() { Name = string.Empty };
            // Act
            var exception = await Assert.ThrowsAsync<RpcException>(async () =>
            {
                await service.SayHello(request, contextMock.Object);
            });
            // Assert
            Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
        }
    }
}
