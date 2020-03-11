using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreGrpcIntegrationTests.AspNetCoreServerApp.Tests.App_Infrastructure.DelegatingHandlers
{
    public class OverrideResponseHttpVersionHandler : DelegatingHandler
    {
        private readonly Version _httpVersion;

        public OverrideResponseHttpVersionHandler(Version httpVersion) : this(httpVersion, null)
        {
        }

        public OverrideResponseHttpVersionHandler(Version httpVersion, HttpMessageHandler innerHandler)
        {
            _httpVersion = httpVersion;
            InnerHandler = innerHandler ?? new HttpClientHandler();
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);
            response.Version = _httpVersion;
            return response;
        }
    }
}
