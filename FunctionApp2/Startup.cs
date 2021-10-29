using Azure.Storage.Blobs;
using ClassLibrary1;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Xml.Serialization;

[assembly: FunctionsStartup(typeof(FunctionApp2.Startup))]

namespace FunctionApp2
{
    public class Startup : FunctionsStartup
    {
        // https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-dependency-injection
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
        }


        public override void Configure(IFunctionsHostBuilder builder)
        {
            // https://blog.stephencleary.com/2018/06/microsoft-extensions-logging-part-2-types.html
            // FIXME: without Configure, it will NOT load the default options, i.e. enabled = false
            builder.Services.AddPipelines();

            // the first one causes a runtime exception if the service depends on a delegate,
            // i.e. Action or Func, however, the one below works - but they should be the same!
            //builder.Services.AddSingleton<MyService>();
            //builder.Services.AddSingleton(new MyService());
        }
    }
}