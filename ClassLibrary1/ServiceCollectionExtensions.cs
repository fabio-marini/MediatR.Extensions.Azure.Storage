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
using System.Linq;
using System.Xml.Serialization;

namespace ClassLibrary1
{
    public static class ServiceCollectionExtensions
    {
        // 1. walk through the models, pipeline (commands/query and behaviors) and functions
        // 2. simple pipeline (without any storage behaviors/processors)
        // 3. table, blob and queue tracking pipelines (default/custom)
        // 4. add storage processors to track GET response
        // 5. use storage behaviors for activity tracking (BAM)
        // 6. use storage behaviors for activity and message tracking (named options)
        // 7. claim check pipeline

        // TODO: review ServiceCollectionExtensions extension methods - behaviors are added explicitly, processors automatically?
        // TODO: add commands to all DEMO ServiceCollectionExtensions extension methods so they can be injected

        // TODO: refactor extension tests so each base class has a corresponding test class...
        // TODO: add tests for send/receive queue commands

        // TODO: add example for queue behaviors/processors
        // TODO: add implementation of remaining blob/queue commands
        // TODO: add implementation of remaining behaviors/processors

        // TODO: rename behaviors (delete request applies to blob/table/queue)
        // TODO: add src and examples folders + add code examples to README...
        // TODO: log operation results (see table commands) + wrap command operations in try/catch and rethrow consistent exception
        // TODO: rename/document options (they are use for insert/delete/retrieve) + update README

        // TODO: create simple diagrams?
        // TODO: add projects for Service Bus (messaging and management?) and HttpClient?

        public static IServiceCollection AddCore(this IServiceCollection services)
        {
            // core set of dependencies
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

            services.AddTransient<IPipelineBehavior<SourceCustomerCommand, Unit>, SendRequestBehavior<SourceCustomerCommand>>();
            services.AddTransient<IPipelineBehavior<SourceCustomerCommand, Unit>, ValidateSourceCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<SourceCustomerCommand, Unit>, TransformSourceCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<SourceCustomerCommand, Unit>, SendRequestBehavior<SourceCustomerCommand>>();

            services.AddTransient<IPipelineBehavior<TargetCustomerCommand, Unit>, SendRequestBehavior<TargetCustomerCommand>>();
            services.AddTransient<IPipelineBehavior<TargetCustomerCommand, Unit>, TransformTargetCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<TargetCustomerCommand, Unit>, EnrichTargetCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<TargetCustomerCommand, Unit>, SendRequestBehavior<TargetCustomerCommand>>();

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

        public static IServiceCollection AddBlobTrackingBehaviors(this IServiceCollection services)
        {
            var container = new BlobContainerClient("UseDevelopmentStorage=true", "messages");
            container.CreateIfNotExists();

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

            services.AddTransient<IPipelineBehavior<RetrieveCustomerQuery, RetrieveCustomerResult>, UploadRequestBehavior<RetrieveCustomerQuery, RetrieveCustomerResult>>();

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

        public static IServiceCollection AddClaimCheckPipeline(this IServiceCollection services)
        {
            // FIXME: claim check - canonical customer is populated on delete (retrieve only stores it in the context)
            //        could use a delegate to map ITableEntity to TRequest/TResponse?

            var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            var cloudTable = storageAccount.CreateCloudTableClient().GetTableReference("ClaimChecks");
            cloudTable.CreateIfNotExists();

            services.AddScoped<PipelineContext>();

            services.AddTransient<InsertEntityCommand<SourceCustomerCommand>>();
            services.AddTransient<RetrieveEntityCommand<SourceCustomerCommand>>();
            services.AddTransient<DeleteEntityCommand<SourceCustomerCommand>>();
            services.AddTransient<InsertEntityCommand<TargetCustomerCommand>>();
            services.AddTransient<RetrieveEntityCommand<TargetCustomerCommand>>();
            services.AddTransient<DeleteEntityCommand<TargetCustomerCommand>>();

            services.AddOptions<InsertEntityOptions<SourceCustomerCommand>>().Configure<IConfiguration>((opt, cfg) =>
            {
                opt.IsEnabled = cfg.GetValue<bool>("TrackingEnabled");
                opt.CloudTable = cloudTable;
                opt.TableEntity = (req, ctx) =>
                {
                    var tableEntity = new DynamicTableEntity("SourceCustomerCommand", req.MessageId);

                    var canonicalCustomer = JsonConvert.SerializeObject(req.CanonicalCustomer);

                    tableEntity.Properties.Add("CanonicalCustomer", EntityProperty.GeneratePropertyForString(canonicalCustomer));

                    return tableEntity;
                };
            });
            services.AddOptions<InsertEntityOptions<TargetCustomerCommand>>().Configure<IConfiguration>((opt, cfg) =>
            {
                opt.IsEnabled = cfg.GetValue<bool>("TrackingEnabled");
                opt.CloudTable = cloudTable;
                opt.TableEntity = (req, ctx) =>
                {
                    var tableEntity = new DynamicTableEntity("SourceCustomerCommand", req.MessageId) { ETag = "*" };

                    if (ctx?.Entities != null)
                    {
                        // this delegate will be used by both retrieve and delete...
                        // when used by delete, the context will have the canonical customer
                        var retrievedEntity = ctx.Entities
                            .FirstOrDefault(e => e.PartitionKey == tableEntity.PartitionKey && e.RowKey == tableEntity.RowKey);

                        if (retrievedEntity != null)
                        {
                            var canonicalCustomer = retrievedEntity["CanonicalCustomer"].StringValue;

                            req.CanonicalCustomer = JsonConvert.DeserializeObject<CanonicalCustomer>(canonicalCustomer);
                        }
                    }

                    return tableEntity;
                };
            });

            services.AddTransient<IPipelineBehavior<SourceCustomerCommand, Unit>, ValidateSourceCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<SourceCustomerCommand, Unit>, TransformSourceCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<SourceCustomerCommand, Unit>, InsertRequestBehavior<SourceCustomerCommand>>();

            services.AddTransient<IPipelineBehavior<TargetCustomerCommand, Unit>, RetrieveRequestBehavior<TargetCustomerCommand>>();
            services.AddTransient<IPipelineBehavior<TargetCustomerCommand, Unit>, DeleteRequestBehavior<TargetCustomerCommand>>();
            services.AddTransient<IPipelineBehavior<TargetCustomerCommand, Unit>, TransformTargetCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<TargetCustomerCommand, Unit>, EnrichTargetCustomerBehavior>();

            return services;
        }

        public static IServiceCollection AddQueueClaimCheckPipeline(this IServiceCollection services)
        {
            var queueClient = new QueueClient("UseDevelopmentStorage=true", "claim-checks");
            queueClient.CreateIfNotExists();

            services.AddScoped<PipelineContext>();

            services.AddTransient<SendMessageCommand<SourceCustomerCommand>>();
            services.AddTransient<ReceiveMessageCommand<TargetCustomerCommand>>();

            services.AddOptions<QueueMessageOptions<SourceCustomerCommand>>().Configure<IConfiguration>((opt, cfg) =>
            {
                opt.IsEnabled = cfg.GetValue<bool>("TrackingEnabled");
                opt.QueueClient = queueClient;
                opt.QueueMessage = (req, ctx) =>
                {
                    var canonicalCustomer = JsonConvert.SerializeObject(new { Id = req.MessageId, req.CanonicalCustomer });

                    return BinaryData.FromString(canonicalCustomer);
                };
            });
            services.AddOptions<QueueMessageOptions<TargetCustomerCommand>>().Configure<IConfiguration>((opt, cfg) =>
            {
                opt.IsEnabled = cfg.GetValue<bool>("TrackingEnabled");
                opt.QueueClient = queueClient;
                opt.QueueMessage = (req, ctx) =>
                {
                    var receivedMessage = ctx?.Messages.Dequeue();

                    if (receivedMessage != null)
                    {
                        var canonicalCustomer = receivedMessage.Body.ToString();

                        req.CanonicalCustomer = JsonConvert.DeserializeObject<CanonicalCustomer>(canonicalCustomer);
                    }

                    return receivedMessage?.Body;
                };
            });

            services.AddTransient<IPipelineBehavior<SourceCustomerCommand, Unit>, ValidateSourceCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<SourceCustomerCommand, Unit>, TransformSourceCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<SourceCustomerCommand, Unit>, SendRequestBehavior<SourceCustomerCommand>>();

            services.AddTransient<IPipelineBehavior<TargetCustomerCommand, Unit>, ReceiveRequestBehavior<TargetCustomerCommand>>();
            services.AddTransient<IPipelineBehavior<TargetCustomerCommand, Unit>, TransformTargetCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<TargetCustomerCommand, Unit>, EnrichTargetCustomerBehavior>();

            return services;
        }
    }
}