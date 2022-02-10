using Azure.Storage.Blobs;
using MediatR.Extensions.Abstractions;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Xunit.Abstractions;

namespace MediatR.Extensions.Azure.Storage.Examples
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreDependencies(this IServiceCollection services, ITestOutputHelper log)
        {
            return services

                .AddMediatR(typeof(CanonicalCustomer))

                .AddSingleton<IConfiguration>(sp =>
                {
                    var appSettings = new Dictionary<string, string>
                    {
                        { "TrackingEnabled", "true" }
                    };

                    return new ConfigurationBuilder()

                        .AddInMemoryCollection(appSettings)
                        .Build();
                })

                .AddOptions<TestOutputLoggerOptions>().Configure(opt => opt.MinimumLogLevel = LogLevel.Information)
                .Services

                .AddTransient<ITestOutputHelper>(sp => log)
                .AddTransient<ILogger, TestOutputLogger>()

                .AddScoped<PipelineContext>()

                // used for message tracking and claim checks
                .AddTransient<BlobContainerClient>(sp =>
                {
                    var blobclient = new BlobContainerClient("UseDevelopmentStorage=true", "integration-tests");
                    blobclient.CreateIfNotExists();

                    return blobclient;
                })
                .AddTransient<BlobFixture>()

                // used for activity tracking
                .AddTransient<CloudTable>(sp =>
                {
                    var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;

                    var cloudTable = storageAccount.CreateCloudTableClient().GetTableReference("IntegrationTests");
                    cloudTable.CreateIfNotExists();

                    return cloudTable;
                })
                .AddTransient<TableFixture>()

                ;
        }
    }
}
