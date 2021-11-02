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
            builder.Services.AddCore();

            //builder.Services.AddSimplePipeline();

            //builder.Services.AddTableTrackingPipeline();
            //builder.Services.AddBlobTrackingPipeline();
            //builder.Services.AddQueueRoutingPipeline();

            //builder.Services.AddBlobTrackingProcessors();

            //builder.Services.AddActivityTrackingPipeline();
            builder.Services.AddMultiTrackingPipeline();

            // the first one causes a runtime exception if the service depends on a delegate,
            // i.e. Action or Func, however, the one below works - but they should be the same!
            //builder.Services.AddSingleton<MyService>();
            //builder.Services.AddSingleton(new MyService());
        }
    }
}