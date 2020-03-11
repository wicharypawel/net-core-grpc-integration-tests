using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Mvc.Testing.Handlers;
using NetCoreGrpcIntegrationTests.AspNetCoreServerApp.Tests.App_Infrastructure.DelegatingHandlers;
using System.Net;
using System.Net.Http;

namespace NetCoreGrpcIntegrationTests.AspNetCoreServerApp.Tests.App_Infrastructure.Extensions
{
    public static class WebApplicationFactoryExtensions
    {
        public static HttpClient CreateClientForGrpc<TEntryPoint>(this WebApplicationFactory<TEntryPoint> webApplicationFactory) where TEntryPoint : class
        {
            return webApplicationFactory.CreateDefaultClient(new DelegatingHandler[]
            {
                new CookieContainerHandler(),
                new OverrideResponseHttpVersionHandler(HttpVersion.Version20)
            });
        }
    }
}
