using ClassLibrary1;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

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
            //builder.Services.AddCore();

            //builder.Services.AddSimplePipeline();

            //builder.Services.AddTableTrackingPipeline();
            //builder.Services.AddBlobTrackingPipeline();
            //builder.Services.AddQueueRoutingPipeline();

            //builder.Services.AddActivityTrackingPipeline();
            //builder.Services.AddMultiTrackingPipeline();

            //builder.Services.AddClaimCheckPipeline();

            //builder.Services.AddBlobTrackingProcessors();
        }
    }
}