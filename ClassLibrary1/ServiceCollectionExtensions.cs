using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using MediatR;
using MediatR.Extensions.Azure.Storage;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Xml.Serialization;

namespace ClassLibrary1
{
    public static class ServiceCollectionExtensions
    {
        // TODO: add DevOps build...

        // TODO: add request type to blob metadata?
        // TODO: add pre (request) and post (response) processors for table/blob/queue storage
        // TODO: refactor test fixtures to test commands directly, not behaviors/processors?

        // TODO: review commands logging - some still using "behavior"
        // TODO: review logging - debug only in commands + info/error in processors/behaviors?
        // TODO: review exception handling - bubble exception up in commands + try/catch in processors/behaviors?

        // TODO: extract insert/queue/upload logic (incl. defaults)
        // TODO: rename Insert/Queue/UploadRequestBehavior and Insert/Queue/UploadResponseProcessor and options?

        public static IServiceCollection AddPipelines(this IServiceCollection services)
        {
            services.AddSingleton<ILogger>(sp =>
            {
                // https://blog.stephencleary.com/2018/06/microsoft-extensions-logging-part-2-types.html
                var loggerFactory = LoggerFactory.Create(cfg =>
                {
                    cfg.AddConsole();
                    cfg.SetMinimumLevel(LogLevel.Information);
                });

                return loggerFactory.CreateLogger("ClassLibrary1");
            });

            services.AddMediatR(typeof(ServiceCollectionExtensions));

            // a transient lifetime will cause state to be lost between behaviors
            // a singleton lifetime will cause duplicate errors when adding items to the dictionary
            services.AddScoped<PipelineContext>();

            // this is required by the SourceCustomerCommand... :)
            services.AddSingleton<QueueClient>(sp =>
            {
                var queueClient = new QueueClient("UseDevelopmentStorage=true", "customers");
                queueClient.CreateIfNotExists();

                return queueClient;
            });

            services.AddOptions<UploadBlobOptions<SourceCustomerCommand>>().Configure<IConfiguration>((opt, cfg) =>
            {
                opt.IsEnabled = cfg.GetValue<bool>("TrackingEnabled");

                opt.BlobClient = (req, ctx) =>
                {
                    var container = new BlobContainerClient("UseDevelopmentStorage=true", "messages");
                    container.CreateIfNotExists();

                    return container.GetBlobClient(Guid.NewGuid().ToString() + ".xml");
                };
            });
            services.AddOptions<InsertEntityOptions<SourceCustomerCommand>>().Configure<IConfiguration>((opt, cfg) =>
            {
                opt.IsEnabled = cfg.GetValue<bool>("TrackingEnabled");

                var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;

                opt.CloudTable = storageAccount.CreateCloudTableClient().GetTableReference("Messages");
                opt.CloudTable.CreateIfNotExists();
            });
            services.AddOptions<QueueMessageOptions<SourceCustomerCommand>>().Configure<IConfiguration>((opt, cfg) =>
            {
                opt.IsEnabled = cfg.GetValue<bool>("TrackingEnabled");

                opt.QueueClient = new QueueClient("UseDevelopmentStorage=true", "messages");
                opt.QueueClient.CreateIfNotExists();

                opt.TimeToLive = TimeSpan.FromDays(1);
                opt.Visibility = TimeSpan.FromSeconds(15);
            });

            services.AddOptions<UploadBlobOptions<TargetCustomerCommand>>().Configure<IConfiguration>((opt, cfg) =>
            {
                opt.IsEnabled = cfg.GetValue<bool>("TrackingEnabled");

                opt.BlobClient = (req, ctx) =>
                {
                    var container = new BlobContainerClient("UseDevelopmentStorage=true", "messages2");
                    container.CreateIfNotExists();

                    return container.GetBlobClient(Guid.NewGuid().ToString() + ".xml");
                };

                opt.BlobContent = (req, ctx) =>
                {
                    var xml = new XmlSerializer(typeof(TargetCustomerCommand));

                    using var ms = new MemoryStream();

                    xml.Serialize(ms, req);

                    return BinaryData.FromBytes(ms.ToArray());
                };
            });

            services.AddOptions<InsertEntityOptions<SourceCustomerCommand>>("BAM").Configure<IConfiguration>((opt, cfg) =>
            {
                opt.IsEnabled = cfg.GetValue<bool>("TrackingEnabled");

                var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;

                opt.CloudTable = storageAccount.CreateCloudTableClient().GetTableReference("Activities");
                opt.CloudTable.CreateIfNotExists();

                opt.TableEntity = (cmd, ctx) =>
                {
                    if (ctx.ContainsKey("CustomerActivity") == false)
                    {
                        throw new ArgumentException("No customer activity found in pipeline context");
                    }

                    return (CustomerActivityEntity)ctx["CustomerActivity"];
                };
            });
            services.AddOptions<InsertEntityOptions<TargetCustomerCommand>>("BAM").Configure<IConfiguration>((opt, cfg) =>
            {
                opt.IsEnabled = cfg.GetValue<bool>("TrackingEnabled");

                var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;

                opt.CloudTable = storageAccount.CreateCloudTableClient().GetTableReference("Activities");
                opt.CloudTable.CreateIfNotExists();

                opt.TableEntity = (cmd, ctx) =>
                {
                    if (ctx.ContainsKey("CustomerActivity") == false)
                    {
                        throw new ArgumentException("No customer activity found in pipeline context");
                    }

                    return (CustomerActivityEntity)ctx["CustomerActivity"];
                };
            });

            // source customer pipeline - validate, transform and send to queue
            services.AddTransient<IPipelineBehavior<SourceCustomerCommand, Unit>, InsertRequestBehavior<SourceCustomerCommand>>();
            services.AddTransient<IPipelineBehavior<SourceCustomerCommand, Unit>, ValidateSourceCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<SourceCustomerCommand, Unit>, TransformSourceCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<SourceCustomerCommand, Unit>, UploadRequestBehavior<SourceCustomerCommand>>();
            services.AddTransient<IPipelineBehavior<SourceCustomerCommand, Unit>, InsertRequestBehavior<SourceCustomerCommand>>(sp =>
            {
                var opt = sp.GetRequiredService<IOptionsSnapshot<InsertEntityOptions<SourceCustomerCommand>>>().Get("BAM");

                return ActivatorUtilities.CreateInstance<InsertRequestBehavior<SourceCustomerCommand>>(sp, Options.Create(opt));
            });

            // target customer pipeline - transform, enrich and write to file
            services.AddTransient<IPipelineBehavior<TargetCustomerCommand, Unit>, TransformTargetCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<TargetCustomerCommand, Unit>, EnrichTargetCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<TargetCustomerCommand, Unit>, InsertRequestBehavior<TargetCustomerCommand>>(sp =>
            {
                var opt = sp.GetRequiredService<IOptionsSnapshot<InsertEntityOptions<TargetCustomerCommand>>>().Get("BAM");

                return ActivatorUtilities.CreateInstance<InsertRequestBehavior<TargetCustomerCommand>>(sp, Options.Create(opt));
            });

            // retrieve pipeline
            services.AddOptions<InsertEntityOptions<RetrieveCustomerQuery>>().Configure<IConfiguration>((opt, cfg) =>
            {
                opt.IsEnabled = cfg.GetValue<bool>("TrackingEnabled");

                var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;

                opt.CloudTable = storageAccount.CreateCloudTableClient().GetTableReference("Messages");
                opt.CloudTable.CreateIfNotExists();
            });

            services.AddTransient<IPipelineBehavior<RetrieveCustomerQuery, RetrieveCustomerResult>, InsertRequestBehavior<RetrieveCustomerQuery, RetrieveCustomerResult>>();

            return services;
        }
    }
}