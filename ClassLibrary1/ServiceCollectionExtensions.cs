using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Queues;
using MediatR;
using MediatR.Extensions.Azure.Storage;
using MediatR.Pipeline;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Xml.Serialization;

namespace ClassLibrary1
{
    public static class ServiceCollectionExtensions
    {
        // 1. walk through the models, pipeline (commands/query and behaviors) and functions
        // 2. simple pipeline (without any storage behaviors/processors) and GET endpoint
        // 3. table, blob and queue tracking pipelines (default/custom)
        // 4. add storage processors to track GET response
        // 5. use storage behaviors for activity tracking (BAM)
        // 6. use storage behaviors for activity and message tracking (named options)
        // 7. TODO: claim check pipeline?

        // TODO: create simple diagrams?

        // TODO: TableEntity delegate returns null for queue (okay) and blobs (throws)?

        // TODO: add DevOps build + README
        // TODO: add request type to blob metadata?
        // TODO: honour cancellation tokens?
        // TODO: add projects for Service Bus (messaging and management?) and HttpClient?

        public static IServiceCollection AddCore(this IServiceCollection services)
        {
            // core set of dependencies - POST and GET work without any additional dependency
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
            //services.AddScoped<PipelineContext>();

            services.AddSingleton<QueueClient>(sp =>
            {
                var queueClient = new QueueClient("UseDevelopmentStorage=true", "customers");
                queueClient.CreateIfNotExists();

                return queueClient;
            });

            return services;
        }

        public static IServiceCollection AddSimplePipeline(this IServiceCollection services)
        {
            services.AddTransient<IPipelineBehavior<SourceCustomerCommand, Unit>, ValidateSourceCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<SourceCustomerCommand, Unit>, TransformSourceCustomerBehavior>();

            services.AddTransient<IPipelineBehavior<TargetCustomerCommand, Unit>, TransformTargetCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<TargetCustomerCommand, Unit>, EnrichTargetCustomerBehavior>();

            return services;
        }

        public static IServiceCollection AddTableTrackingPipeline(this IServiceCollection services)
        {
            var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            var cloudTable = storageAccount.CreateCloudTableClient().GetTableReference("Messages");
            cloudTable.CreateIfNotExists();

            services.AddOptions<InsertEntityOptions<SourceCustomerCommand>>().Configure<IConfiguration>((opt, cfg) =>
            {
                opt.IsEnabled = cfg.GetValue<bool>("TrackingEnabled");
                opt.CloudTable = cloudTable;
            });
            services.AddOptions<InsertEntityOptions<TargetCustomerCommand>>().Configure<IConfiguration>((opt, cfg) =>
            {
                opt.IsEnabled = cfg.GetValue<bool>("TrackingEnabled");
                opt.CloudTable = cloudTable;
                opt.TableEntity = (req, ctx) =>
                {
                    var pk = Guid.NewGuid().ToString();
                    var rk = Guid.NewGuid().ToString();

                    var tableEntity = new DynamicTableEntity(pk, rk);

                    tableEntity.Properties.Add("Message", EntityProperty.GeneratePropertyForString(req.CanonicalCustomer.GetType().FullName));
                    tableEntity.Properties.Add("Content", EntityProperty.GeneratePropertyForString(JsonConvert.SerializeObject(req.CanonicalCustomer)));

                    return tableEntity;
                };
            });

            services.AddTransient<IPipelineBehavior<SourceCustomerCommand, Unit>, InsertRequestBehavior<SourceCustomerCommand>>();
            services.AddTransient<IPipelineBehavior<SourceCustomerCommand, Unit>, ValidateSourceCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<SourceCustomerCommand, Unit>, TransformSourceCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<SourceCustomerCommand, Unit>, InsertRequestBehavior<SourceCustomerCommand>>();

            services.AddTransient<IPipelineBehavior<TargetCustomerCommand, Unit>, InsertRequestBehavior<TargetCustomerCommand>>();
            services.AddTransient<IPipelineBehavior<TargetCustomerCommand, Unit>, TransformTargetCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<TargetCustomerCommand, Unit>, EnrichTargetCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<TargetCustomerCommand, Unit>, InsertRequestBehavior<TargetCustomerCommand>>();

            return services;
        }

        public static IServiceCollection AddBlobTrackingPipeline(this IServiceCollection services)
        {
            var container = new BlobContainerClient("UseDevelopmentStorage=true", "messages");
            container.CreateIfNotExists();

            services.AddOptions<UploadBlobOptions<SourceCustomerCommand>>().Configure<IConfiguration>((opt, cfg) =>
            {
                opt.IsEnabled = cfg.GetValue<bool>("TrackingEnabled");
                opt.BlobClient = (req, ctx) => container.GetBlobClient($"customers/source/{Guid.NewGuid().ToString()}.json");
            });
            services.AddOptions<UploadBlobOptions<TargetCustomerCommand>>().Configure<IConfiguration>((opt, cfg) =>
            {
                opt.IsEnabled = cfg.GetValue<bool>("TrackingEnabled");
                opt.BlobClient = (req, ctx) => container.GetBlobClient($"customers/target/{Guid.NewGuid().ToString()}.xml");
                opt.BlobContent = (req, ctx) =>
                {
                    var xml = new XmlSerializer(req.CanonicalCustomer.GetType());

                    using var ms = new MemoryStream();

                    xml.Serialize(ms, req.CanonicalCustomer);

                    return BinaryData.FromBytes(ms.ToArray());
                };
                opt.BlobHeaders = (req, ctx) => new BlobHttpHeaders { ContentType = "application/xml" };
            });

            services.AddTransient<IPipelineBehavior<SourceCustomerCommand, Unit>, UploadRequestBehavior<SourceCustomerCommand>>();
            services.AddTransient<IPipelineBehavior<SourceCustomerCommand, Unit>, ValidateSourceCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<SourceCustomerCommand, Unit>, TransformSourceCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<SourceCustomerCommand, Unit>, UploadRequestBehavior<SourceCustomerCommand>>();

            services.AddTransient<IPipelineBehavior<TargetCustomerCommand, Unit>, UploadRequestBehavior<TargetCustomerCommand>>();
            services.AddTransient<IPipelineBehavior<TargetCustomerCommand, Unit>, TransformTargetCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<TargetCustomerCommand, Unit>, EnrichTargetCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<TargetCustomerCommand, Unit>, UploadRequestBehavior<TargetCustomerCommand>>();

            return services;
        }

        public static IServiceCollection AddQueueRoutingPipeline(this IServiceCollection services)
        {
            var queueClient = new QueueClient("UseDevelopmentStorage=true", "messages");
            queueClient.CreateIfNotExists();

            services.AddOptions<QueueMessageOptions<SourceCustomerCommand>>().Configure<IConfiguration>((opt, cfg) =>
            {
                opt.IsEnabled = cfg.GetValue<bool>("TrackingEnabled");
                opt.QueueClient = queueClient;
            });
            services.AddOptions<QueueMessageOptions<TargetCustomerCommand>>().Configure<IConfiguration>((opt, cfg) =>
            {
                opt.IsEnabled = cfg.GetValue<bool>("TrackingEnabled");
                opt.QueueClient = queueClient;
                opt.TimeToLive = TimeSpan.FromDays(1);
                opt.Visibility = TimeSpan.FromSeconds(30);
                opt.QueueMessage = (req, ctx) =>
                {
                    var xml = new XmlSerializer(req.CanonicalCustomer.GetType());

                    using var ms = new MemoryStream();

                    xml.Serialize(ms, req.CanonicalCustomer);

                    return BinaryData.FromBytes(ms.ToArray());
                };
            });

            services.AddTransient<IPipelineBehavior<SourceCustomerCommand, Unit>, QueueRequestBehavior<SourceCustomerCommand>>();
            services.AddTransient<IPipelineBehavior<SourceCustomerCommand, Unit>, ValidateSourceCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<SourceCustomerCommand, Unit>, TransformSourceCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<SourceCustomerCommand, Unit>, QueueRequestBehavior<SourceCustomerCommand>>();

            services.AddTransient<IPipelineBehavior<TargetCustomerCommand, Unit>, QueueRequestBehavior<TargetCustomerCommand>>();
            services.AddTransient<IPipelineBehavior<TargetCustomerCommand, Unit>, TransformTargetCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<TargetCustomerCommand, Unit>, EnrichTargetCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<TargetCustomerCommand, Unit>, QueueRequestBehavior<TargetCustomerCommand>>();

            return services;
        }

        public static IServiceCollection AddBlobTrackingProcessors(this IServiceCollection services)
        {
            var container = new BlobContainerClient("UseDevelopmentStorage=true", "messages");
            container.CreateIfNotExists();

            // register pre/post processors to track messages in blob storage
            services.AddTransient<IRequestPreProcessor<RetrieveCustomerQuery>, UploadRequestProcessor<RetrieveCustomerQuery>>();
            services.AddTransient<IRequestPostProcessor<RetrieveCustomerQuery, RetrieveCustomerResult>, UploadResponseProcessor<RetrieveCustomerQuery, RetrieveCustomerResult>>();

            services.AddOptions<UploadBlobOptions<RetrieveCustomerQuery>>().Configure<IConfiguration>((opt, cfg) =>
            {
                opt.IsEnabled = cfg.GetValue<bool>("TrackingEnabled");
                opt.BlobClient = (req, ctx) => container.GetBlobClient($"customers/query/{Guid.NewGuid().ToString()}.json");
            });
            services.AddOptions<UploadBlobOptions<RetrieveCustomerResult>>().Configure<IConfiguration>((opt, cfg) =>
            {
                opt.IsEnabled = cfg.GetValue<bool>("TrackingEnabled");
                opt.BlobClient = (req, ctx) => container.GetBlobClient($"customers/result/{Guid.NewGuid().ToString()}.json");
            });

            return services;
        }

        public static IServiceCollection AddActivityTrackingPipeline(this IServiceCollection services)
        {
            var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            var cloudTable = storageAccount.CreateCloudTableClient().GetTableReference("Activities");
            cloudTable.CreateIfNotExists();

            services.AddOptions<InsertEntityOptions<SourceCustomerCommand>>().Configure<IConfiguration>((opt, cfg) =>
            {
                opt.IsEnabled = cfg.GetValue<bool>("TrackingEnabled");
                opt.CloudTable = cloudTable;
                opt.TableEntity = (req, ctx) =>
                {
                    return new CustomerActivityEntity
                    {
                        PartitionKey = req.MessageId,
                        RowKey = Guid.NewGuid().ToString(),
                        IsValid = true,
                        CustomerReceivedOn = DateTime.Now,
                        Email = req.SourceCustomer.Email
                    };
                };
            });
            services.AddOptions<InsertEntityOptions<TargetCustomerCommand>>().Configure<IConfiguration>((opt, cfg) =>
            {
                opt.IsEnabled = cfg.GetValue<bool>("TrackingEnabled");
                opt.CloudTable = cloudTable;
                opt.TableEntity = (req, ctx) =>
                {
                    return new CustomerActivityEntity
                    {
                        PartitionKey = req.MessageId,
                        RowKey = Guid.NewGuid().ToString(),
                        DateOfBirth = req.TargetCustomer.DateOfBirth,
                        CustomerPublishedOn = DateTime.Now
                    };
                };
            });

            services.AddTransient<IPipelineBehavior<SourceCustomerCommand, Unit>, ValidateSourceCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<SourceCustomerCommand, Unit>, TransformSourceCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<SourceCustomerCommand, Unit>, InsertRequestBehavior<SourceCustomerCommand>>();

            services.AddTransient<IPipelineBehavior<TargetCustomerCommand, Unit>, TransformTargetCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<TargetCustomerCommand, Unit>, EnrichTargetCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<TargetCustomerCommand, Unit>, InsertRequestBehavior<TargetCustomerCommand>>();

            return services;
        }

        public static IServiceCollection AddMultiTrackingPipeline(this IServiceCollection services)
        {
            var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;

            var messagesTable = storageAccount.CreateCloudTableClient().GetTableReference("Messages");
            messagesTable.CreateIfNotExists();

            var activitiesTable = storageAccount.CreateCloudTableClient().GetTableReference("Activities");
            activitiesTable.CreateIfNotExists();

            services.AddOptions<InsertEntityOptions<SourceCustomerCommand>>("Messages").Configure<IConfiguration>((opt, cfg) =>
            {
                opt.IsEnabled = cfg.GetValue<bool>("TrackingEnabled");
                opt.CloudTable = messagesTable;
            });
            services.AddOptions<InsertEntityOptions<TargetCustomerCommand>>("Messages").Configure<IConfiguration>((opt, cfg) =>
            {
                opt.IsEnabled = cfg.GetValue<bool>("TrackingEnabled");
                opt.CloudTable = messagesTable;
                opt.TableEntity = (req, ctx) =>
                {
                    var pk = Guid.NewGuid().ToString();
                    var rk = Guid.NewGuid().ToString();

                    var tableEntity = new DynamicTableEntity(pk, rk);

                    tableEntity.Properties.Add("Message", EntityProperty.GeneratePropertyForString(req.CanonicalCustomer.GetType().FullName));
                    tableEntity.Properties.Add("Content", EntityProperty.GeneratePropertyForString(JsonConvert.SerializeObject(req.CanonicalCustomer)));

                    return tableEntity;
                };
            });
            services.AddOptions<InsertEntityOptions<SourceCustomerCommand>>("Source").Configure<IConfiguration>((opt, cfg) =>
            {
                opt.IsEnabled = cfg.GetValue<bool>("TrackingEnabled");
                opt.CloudTable = activitiesTable;
                opt.TableEntity = (req, ctx) =>
                {
                    return new CustomerActivityEntity
                    {
                        PartitionKey = req.MessageId,
                        RowKey = Guid.NewGuid().ToString(),
                        IsValid = true,
                        CustomerReceivedOn = DateTime.Now,
                        Email = req.SourceCustomer.Email
                    };
                };
            });
            services.AddOptions<InsertEntityOptions<TargetCustomerCommand>>("Target").Configure<IConfiguration>((opt, cfg) =>
            {
                opt.IsEnabled = cfg.GetValue<bool>("TrackingEnabled");
                opt.CloudTable = activitiesTable;
                opt.TableEntity = (req, ctx) =>
                {
                    return new CustomerActivityEntity
                    {
                        PartitionKey = req.MessageId,
                        RowKey = Guid.NewGuid().ToString(),
                        DateOfBirth = req.TargetCustomer.DateOfBirth,
                        CustomerPublishedOn = DateTime.Now
                    };
                };
            });

            services.AddTransient<IPipelineBehavior<SourceCustomerCommand, Unit>, InsertRequestBehavior<SourceCustomerCommand>>(sp =>
            {
                var opt = sp.GetRequiredService<IOptionsSnapshot<InsertEntityOptions<SourceCustomerCommand>>>().Get("Messages");

                return ActivatorUtilities.CreateInstance<InsertRequestBehavior<SourceCustomerCommand>>(sp, Options.Create(opt));
            });
            services.AddTransient<IPipelineBehavior<SourceCustomerCommand, Unit>, ValidateSourceCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<SourceCustomerCommand, Unit>, TransformSourceCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<SourceCustomerCommand, Unit>, InsertRequestBehavior<SourceCustomerCommand>>(sp =>
            {
                var opt = sp.GetRequiredService<IOptionsSnapshot<InsertEntityOptions<SourceCustomerCommand>>>().Get("Messages");

                return ActivatorUtilities.CreateInstance<InsertRequestBehavior<SourceCustomerCommand>>(sp, Options.Create(opt));
            });
            services.AddTransient<IPipelineBehavior<SourceCustomerCommand, Unit>, InsertRequestBehavior<SourceCustomerCommand>>(sp =>
            {
                var opt = sp.GetRequiredService<IOptionsSnapshot<InsertEntityOptions<SourceCustomerCommand>>>().Get("Source");

                return ActivatorUtilities.CreateInstance<InsertRequestBehavior<SourceCustomerCommand>>(sp, Options.Create(opt));
            });

            services.AddTransient<IPipelineBehavior<TargetCustomerCommand, Unit>, InsertRequestBehavior<TargetCustomerCommand>>(sp =>
            {
                var opt = sp.GetRequiredService<IOptionsSnapshot<InsertEntityOptions<TargetCustomerCommand>>>().Get("Messages");

                return ActivatorUtilities.CreateInstance<InsertRequestBehavior<TargetCustomerCommand>>(sp, Options.Create(opt));
            });
            services.AddTransient<IPipelineBehavior<TargetCustomerCommand, Unit>, TransformTargetCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<TargetCustomerCommand, Unit>, EnrichTargetCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<TargetCustomerCommand, Unit>, InsertRequestBehavior<TargetCustomerCommand>>(sp =>
            {
                var opt = sp.GetRequiredService<IOptionsSnapshot<InsertEntityOptions<TargetCustomerCommand>>>().Get("Messages");

                return ActivatorUtilities.CreateInstance<InsertRequestBehavior<TargetCustomerCommand>>(sp, Options.Create(opt));
            });
            services.AddTransient<IPipelineBehavior<TargetCustomerCommand, Unit>, InsertRequestBehavior<TargetCustomerCommand>>(sp =>
            {
                var opt = sp.GetRequiredService<IOptionsSnapshot<InsertEntityOptions<TargetCustomerCommand>>>().Get("Target");

                return ActivatorUtilities.CreateInstance<InsertRequestBehavior<TargetCustomerCommand>>(sp, Options.Create(opt));
            });

            return services;
        }
    }
}