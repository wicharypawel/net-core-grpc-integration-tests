using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using NetCoreGrpcIntegrationTests.Proto;

namespace NetCoreGrpcIntegrationTests.AspNetCoreServerApp.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            if(string.IsNullOrWhiteSpace(request.Name))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, nameof(request.Name)));
            }
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }

        public override async Task SayHelloServerStream(HelloRequest request, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
        {
            foreach (var item in $"Hello {request.Name}")
            {
                await responseStream.WriteAsync(new HelloReply()
                {
                    Message = item.ToString()
                });
            }
        }

        public override async Task<HelloReply> SayHelloClientStream(IAsyncStreamReader<HelloRequest> requestStream, ServerCallContext context)
        {
            var input = string.Empty;
            while (await requestStream.MoveNext())
            {
                input += requestStream.Current.Name;
            }
            return new HelloReply
            {
                Message = "Hello " + input
            };
        }


        public override async Task SayHelloBiStream(IAsyncStreamReader<HelloRequest> requestStream, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                var elem = requestStream.Current;
                await responseStream.WriteAsync(new HelloReply() { Message = elem.Name });
            }
        }
    }
}
