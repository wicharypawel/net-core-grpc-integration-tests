using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NetCoreGrpcIntegrationTests.AspNetCoreServerApp.Tests.App_Infrastructure.ClassFixture
{
    public sealed class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");
            base.ConfigureWebHost(builder);
            builder.UseContentRoot(Directory.GetCurrentDirectory());
        }

        /// <summary>
        ///     Do not use base.CreateWebHostBuilder(); unless you want to use IWebHostBuilder CreateWebHostBuilder(string[] args)
        ///     Do not use base.CreateHostBuilder(); unless you want to use IHostBuilder CreateHostBuilder(string[] args)
        ///     method via reflection from Program.cs
        /// </summary>
        protected override IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureAppConfiguration(builder => builder.AddJsonFile(new EmbeddedFileProvider(Assembly.GetExecutingAssembly()), "App_Infrastructure\\ClassFixture\\appsettings.Test.json", false, true));
                    webBuilder.UseStartup<TStartup>();
                    webBuilder.UseDefaultServiceProvider(options => options.ValidateScopes = false);
                    webBuilder.ConfigureLogging((loggingBuilder) => loggingBuilder.ClearProviders());
                });
        }
    }
}
