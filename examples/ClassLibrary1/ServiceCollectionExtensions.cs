using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using MediatR;
using MediatR.Extensions.Abstractions;
using MediatR.Extensions.Azure.Storage;
using MediatR.Pipeline;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ClassLibrary1
{
    public static class ServiceCollectionExtensions
    {
        #region Examples

        // TODO: update fabrikam handler to have dir info injected + remove AddCore()?

        // TODO: refactor contoso request to return a canonical customer +
        //       refactor fabrikam request as a canonical request that returns a fabrikam response?
        //       (will prove whether mapping as a behavior is a viable solution)

        // TODO: 1. basic pipeline, 2. add blob message tracking (JSON and XML), 3. add table activity tracking (BAM)
        //       4. add message AND activity tracking to same pipeline, 5. claim check pipeline (upload/download/delete blob)

        //  1. walk through the models, pipeline (commands/query and behaviors) and functions
        //  2. simple pipeline (without any storage behaviors/processors)
        //  3. table, blob and queue tracking pipelines (default/JSON and custom/XML)
        //  4. add storage processors to track GET response
        //  5. use storage behaviors for activity tracking (BAM)
        //  6. use storage behaviors for activity and message tracking (named options)
        //  7. claim check pipeline (blob and table)
        // TODO: persistence points to enable edit and resubmit?
        // TODO: sign/verify and encrypt/decrypt using certs?
        // TODO: route error messages to an error pipeline?

        #endregion

        // TODO: update fixtures to use ILogger.. :(
        // TODO: rewrite multi tracking pipeline so that multiple activity records are tracked (instead of messages), e.g.
        //       track at the beginning of the pipeline and the end (and optionally in the middle)
        // TODO: create pipelines/tests to exercise all extensions, e.g. a pipeline to insert/retrieve/delete an entity?

        #region TODO: Obsolete, delete!

        public static IServiceCollection AddTableTrackingPipeline(this IServiceCollection services)
        {
            var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;

            var cloudTable = storageAccount.CreateCloudTableClient().GetTableReference("Messages");
            cloudTable.CreateIfNotExists();

            services.AddOptions<TableOptions<ContosoCustomerRequest>>().Configure<IConfiguration>((opt, cfg) =>
            {
                opt.IsEnabled = cfg.GetValue<bool>("TrackingEnabled");
                opt.CloudTable = cloudTable;
            });
            services.AddOptions<TableOptions<FabrikamCustomerRequest>>().Configure<IConfiguration>((opt, cfg) =>
            {
                // use custom TableEntity to serialize only the canonical customer
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

            services.AddTransient<InsertEntityCommand<ContosoCustomerRequest>>();
            services.AddTransient<InsertEntityCommand<FabrikamCustomerRequest>>();

            services.AddTransient<IPipelineBehavior<ContosoCustomerRequest, Unit>, InsertEntityRequestBehavior<ContosoCustomerRequest>>();
            services.AddTransient<IPipelineBehavior<ContosoCustomerRequest, Unit>, ValidateContosoCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<ContosoCustomerRequest, Unit>, TransformContosoCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<ContosoCustomerRequest, Unit>, InsertEntityRequestBehavior<ContosoCustomerRequest>>();

            services.AddTransient<IPipelineBehavior<FabrikamCustomerRequest, Unit>, InsertEntityRequestBehavior<FabrikamCustomerRequest>>();
            services.AddTransient<IPipelineBehavior<FabrikamCustomerRequest, Unit>, TransformFabrikamCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<FabrikamCustomerRequest, Unit>, EnrichFabrikamCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<FabrikamCustomerRequest, Unit>, InsertEntityRequestBehavior<FabrikamCustomerRequest>>();

            return services;
        }

        public static IServiceCollection AddBlobTrackingPipeline(this IServiceCollection services)
        {
            return services

                .AddContosoMessageTrackingPipeline()
                .AddFabrikamMessageTrackingPipeline();
        }

        public static IServiceCollection AddQueueRoutingPipeline(this IServiceCollection services)
        {
            var queueClient = new QueueClient("UseDevelopmentStorage=true", "messages");
            queueClient.CreateIfNotExists();

            services.AddOptions<QueueOptions<ContosoCustomerRequest>>().Configure<IConfiguration>((opt, cfg) =>
            {
                opt.IsEnabled = cfg.GetValue<bool>("TrackingEnabled");
                opt.QueueClient = queueClient;
            });
            services.AddOptions<QueueOptions<FabrikamCustomerRequest>>().Configure<IConfiguration>((opt, cfg) =>
            {
                // use custom QueueMessage to serialize only the canonical customer (as XML)
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

            services.AddTransient<SendMessageCommand<ContosoCustomerRequest>>();
            services.AddTransient<SendMessageCommand<FabrikamCustomerRequest>>();

            services.AddTransient<IPipelineBehavior<ContosoCustomerRequest, Unit>, SendMessageRequestBehavior<ContosoCustomerRequest>>();
            services.AddTransient<IPipelineBehavior<ContosoCustomerRequest, Unit>, ValidateContosoCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<ContosoCustomerRequest, Unit>, TransformContosoCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<ContosoCustomerRequest, Unit>, SendMessageRequestBehavior<ContosoCustomerRequest>>();

            services.AddTransient<IPipelineBehavior<FabrikamCustomerRequest, Unit>, SendMessageRequestBehavior<FabrikamCustomerRequest>>();
            services.AddTransient<IPipelineBehavior<FabrikamCustomerRequest, Unit>, TransformFabrikamCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<FabrikamCustomerRequest, Unit>, EnrichFabrikamCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<FabrikamCustomerRequest, Unit>, SendMessageRequestBehavior<FabrikamCustomerRequest>>();

            return services;
        }

        public static IServiceCollection AddQueueDeletingPipeline(this IServiceCollection services)
        {
            var queueClient = new QueueClient("UseDevelopmentStorage=true", "messages");
            queueClient.CreateIfNotExists();

            var memoryQueue = new Queue<QueueMessage>();

            services.AddOptions<QueueOptions<ContosoCustomerRequest>>().Configure<IConfiguration>((opt, cfg) =>
            {
                opt.IsEnabled = cfg.GetValue<bool>("TrackingEnabled");
                opt.QueueClient = queueClient;
            });
            services.AddOptions<QueueOptions<FabrikamCustomerRequest>>().Configure<IConfiguration>((opt, cfg) =>
            {
                opt.IsEnabled = cfg.GetValue<bool>("TrackingEnabled");
                opt.QueueClient = queueClient;
                opt.Received = (msg, ctx, req) =>
                {
                    memoryQueue.Enqueue(msg);

                    return Task.CompletedTask;
                };
                opt.Delete = (ctx, req) =>
                {
                    QueueMessage msg;

                    _ = memoryQueue.TryDequeue(out msg);

                    return Task.FromResult(msg);
                };
            });

            services.AddTransient<SendMessageCommand<ContosoCustomerRequest>>();
            services.AddTransient<ReceiveMessageCommand<FabrikamCustomerRequest>>();
            services.AddTransient<DeleteMessageCommand<FabrikamCustomerRequest>>();

            services.AddTransient<IPipelineBehavior<ContosoCustomerRequest, Unit>, SendMessageRequestBehavior<ContosoCustomerRequest>>();
            services.AddTransient<IPipelineBehavior<ContosoCustomerRequest, Unit>, ValidateContosoCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<ContosoCustomerRequest, Unit>, TransformContosoCustomerBehavior>();

            services.AddTransient<IPipelineBehavior<FabrikamCustomerRequest, Unit>, ReceiveMessageRequestBehavior<FabrikamCustomerRequest>>();
            services.AddTransient<IPipelineBehavior<FabrikamCustomerRequest, Unit>, TransformFabrikamCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<FabrikamCustomerRequest, Unit>, EnrichFabrikamCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<FabrikamCustomerRequest, Unit>, DeleteMessageRequestBehavior<FabrikamCustomerRequest>>();

            return services;
        }

        public static IServiceCollection AddBlobTrackingProcessors(this IServiceCollection services)
        {
            var container = new BlobContainerClient("UseDevelopmentStorage=true", "messages");
            container.CreateIfNotExists();

            services.AddTransient<UploadBlobCommand<RetrieveCustomerRequest>>();
            services.AddTransient<UploadBlobCommand<RetrieveCustomerResponse>>();

            // register pre/post processors to track messages in blob storage
            services.AddTransient<IRequestPreProcessor<RetrieveCustomerRequest>, UploadBlobRequestProcessor<RetrieveCustomerRequest>>();
            services.AddTransient<IRequestPostProcessor<RetrieveCustomerRequest, RetrieveCustomerResponse>, UploadBlobResponseProcessor<RetrieveCustomerRequest, RetrieveCustomerResponse>>();

            services.AddOptions<BlobOptions<RetrieveCustomerRequest>>().Configure<IConfiguration>((opt, cfg) =>
            {
                opt.IsEnabled = cfg.GetValue<bool>("TrackingEnabled");
                opt.BlobClient = (req, ctx) => container.GetBlobClient($"customers/query/{Guid.NewGuid().ToString()}.json");
            });
            services.AddOptions<BlobOptions<RetrieveCustomerResponse>>().Configure<IConfiguration>((opt, cfg) =>
            {
                opt.IsEnabled = cfg.GetValue<bool>("TrackingEnabled");
                opt.BlobClient = (req, ctx) => container.GetBlobClient($"customers/result/{Guid.NewGuid().ToString()}.json");
            });

            return services;
        }

        public static IServiceCollection AddBlobTrackingBehaviors(this IServiceCollection services)
        {
            // TODO: query example - delete or add commands?
            var container = new BlobContainerClient("UseDevelopmentStorage=true", "messages");
            container.CreateIfNotExists();

            services.AddOptions<BlobOptions<RetrieveCustomerRequest>>().Configure<IConfiguration>((opt, cfg) =>
            {
                opt.IsEnabled = cfg.GetValue<bool>("TrackingEnabled");
                opt.BlobClient = (req, ctx) => container.GetBlobClient($"customers/query/{Guid.NewGuid().ToString()}.json");
            });
            services.AddOptions<BlobOptions<RetrieveCustomerResponse>>().Configure<IConfiguration>((opt, cfg) =>
            {
                opt.IsEnabled = cfg.GetValue<bool>("TrackingEnabled");
                opt.BlobClient = (req, ctx) => container.GetBlobClient($"customers/result/{Guid.NewGuid().ToString()}.json");
            });

            services.AddTransient<IPipelineBehavior<RetrieveCustomerRequest, RetrieveCustomerResponse>, UploadBlobRequestBehavior<RetrieveCustomerRequest, RetrieveCustomerResponse>>();

            return services;
        }

        public static IServiceCollection AddTableClaimCheckPipeline(this IServiceCollection services)
        {
            var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            var cloudTable = storageAccount.CreateCloudTableClient().GetTableReference("ClaimChecks");
            cloudTable.CreateIfNotExists();

            services.AddScoped<PipelineContext>();

            services.AddTransient<InsertEntityCommand<ContosoCustomerRequest>>();
            services.AddTransient<RetrieveEntityCommand<FabrikamCustomerRequest>>();
            services.AddTransient<DeleteEntityCommand<FabrikamCustomerRequest>>();

            services.AddOptions<TableOptions<ContosoCustomerRequest>>().Configure<IConfiguration>((opt, cfg) =>
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
            services.AddOptions<TableOptions<FabrikamCustomerRequest>>().Configure<IConfiguration>((opt, cfg) =>
            {
                opt.IsEnabled = cfg.GetValue<bool>("TrackingEnabled");
                opt.CloudTable = cloudTable;
                opt.TableEntity = (req, ctx) => new DynamicTableEntity("SourceCustomerCommand", req.MessageId) { ETag = "*" };
                opt.Retrieved = (res, ctx, req) =>
                {
                    var dte = res.Result as DynamicTableEntity;

                    if (dte != null)
                    {
                        var canonicalCustomer = dte["CanonicalCustomer"].StringValue;

                        req.CanonicalCustomer = JsonConvert.DeserializeObject<CanonicalCustomer>(canonicalCustomer);
                    }

                    return Task.CompletedTask;
                };
            });

            services.AddTransient<IPipelineBehavior<ContosoCustomerRequest, Unit>, ValidateContosoCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<ContosoCustomerRequest, Unit>, TransformContosoCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<ContosoCustomerRequest, Unit>, InsertEntityRequestBehavior<ContosoCustomerRequest>>();

            services.AddTransient<IPipelineBehavior<FabrikamCustomerRequest, Unit>, RetrieveEntityRequestBehavior<FabrikamCustomerRequest>>();
            services.AddTransient<IPipelineBehavior<FabrikamCustomerRequest, Unit>, DeleteEntityRequestBehavior<FabrikamCustomerRequest>>();
            services.AddTransient<IPipelineBehavior<FabrikamCustomerRequest, Unit>, TransformFabrikamCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<FabrikamCustomerRequest, Unit>, EnrichFabrikamCustomerBehavior>();

            return services;
        }

        #endregion

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

            // used by ContosoCustomer handler
            services.AddSingleton<QueueClient>(sp =>
            {
                var queueClient = new QueueClient("UseDevelopmentStorage=true", "customers");
                queueClient.CreateIfNotExists();

                return queueClient;
            });

            // used by FabrikamCustomer handler
            services.AddTransient<DirectoryInfo>(sp => new DirectoryInfo($"C:\\Repos\\Customers"));

            return services;
        }

        public static IServiceCollection AddSimplePipeline(this IServiceCollection services)
        {
            services.AddTransient<IPipelineBehavior<ContosoCustomerRequest, Unit>, ValidateContosoCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<ContosoCustomerRequest, Unit>, TransformContosoCustomerBehavior>();

            services.AddTransient<IPipelineBehavior<FabrikamCustomerRequest, Unit>, TransformFabrikamCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<FabrikamCustomerRequest, Unit>, EnrichFabrikamCustomerBehavior>();

            return services;
        }

        public static IServiceCollection AddContosoMessageTrackingPipeline(this IServiceCollection services)
        {
            services.AddOptions<BlobOptions<ContosoCustomerRequest>>().Configure<IServiceProvider>((opt, svc) =>
            {
                var cfg = svc.GetRequiredService<IConfiguration>();
                var blb = svc.GetRequiredService<BlobContainerClient>();

                opt.IsEnabled = cfg.GetValue<bool>("TrackingEnabled");
                opt.BlobClient = (req, ctx) => blb.GetBlobClient($"customers/source/{Guid.NewGuid().ToString()}.json");
            });

            services.AddTransient<UploadBlobCommand<ContosoCustomerRequest>>();

            services.AddTransient<IPipelineBehavior<ContosoCustomerRequest, Unit>, UploadBlobRequestBehavior<ContosoCustomerRequest>>();
            services.AddTransient<IPipelineBehavior<ContosoCustomerRequest, Unit>, ValidateContosoCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<ContosoCustomerRequest, Unit>, TransformContosoCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<ContosoCustomerRequest, Unit>, UploadBlobRequestBehavior<ContosoCustomerRequest>>();

            return services;
        }

        public static IServiceCollection AddFabrikamMessageTrackingPipeline(this IServiceCollection services)
        {
            services.AddOptions<BlobOptions<FabrikamCustomerRequest>>().Configure<IServiceProvider>((opt, svc) =>
            {
                var cfg = svc.GetRequiredService<IConfiguration>();
                var blb = svc.GetRequiredService<BlobContainerClient>();

                // use custom BlobContent to serialize only the canonical customer (as XML)
                opt.IsEnabled = cfg.GetValue<bool>("TrackingEnabled");
                opt.BlobClient = (req, ctx) => blb.GetBlobClient($"customers/target/{Guid.NewGuid().ToString()}.xml");
                opt.BlobContent = (req, ctx) =>
                {
                    var xml = new XmlSerializer(req.CanonicalCustomer.GetType());

                    using var ms = new MemoryStream();

                    xml.Serialize(ms, req.CanonicalCustomer);

                    return BinaryData.FromBytes(ms.ToArray());
                };
                opt.BlobHeaders = (req, ctx) => new BlobHttpHeaders { ContentType = "application/xml" };
            });

            services.AddTransient<UploadBlobCommand<FabrikamCustomerRequest>>();

            services.AddTransient<IPipelineBehavior<FabrikamCustomerRequest, Unit>, UploadBlobRequestBehavior<FabrikamCustomerRequest>>();
            services.AddTransient<IPipelineBehavior<FabrikamCustomerRequest, Unit>, TransformFabrikamCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<FabrikamCustomerRequest, Unit>, EnrichFabrikamCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<FabrikamCustomerRequest, Unit>, UploadBlobRequestBehavior<FabrikamCustomerRequest>>();

            return services;
        }

        public static IServiceCollection AddContosoActivityTrackingPipeline(this IServiceCollection services)
        {
            services.AddOptions<TableOptions<ContosoCustomerRequest>>().Configure<IServiceProvider>((opt, svc) =>
            {
                var cfg = svc.GetRequiredService<IConfiguration>();
                var tbl = svc.GetRequiredService<CloudTable>();

                opt.IsEnabled = cfg.GetValue<bool>("TrackingEnabled");
                opt.CloudTable = tbl;
                opt.TableEntity = (req, ctx) =>
                {
                    return new CustomerActivityEntity
                    {
                        PartitionKey = req.MessageId,
                        RowKey = Guid.NewGuid().ToString(),
                        IsValid = true,
                        CustomerReceivedOn = DateTime.Now,
                        Email = req.ContosoCustomer.Email
                    };
                };
            });

            services.AddTransient<InsertEntityCommand<ContosoCustomerRequest>>();

            services.AddTransient<IPipelineBehavior<ContosoCustomerRequest, Unit>, ValidateContosoCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<ContosoCustomerRequest, Unit>, TransformContosoCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<ContosoCustomerRequest, Unit>, InsertEntityRequestBehavior<ContosoCustomerRequest>>();

            return services;
        }

        public static IServiceCollection AddFabrikamActivityTrackingPipeline(this IServiceCollection services)
        {
            services.AddOptions<TableOptions<FabrikamCustomerRequest>>().Configure<IServiceProvider>((opt, svc) =>
            {
                var cfg = svc.GetRequiredService<IConfiguration>();
                var tbl = svc.GetRequiredService<CloudTable>();

                opt.IsEnabled = cfg.GetValue<bool>("TrackingEnabled");
                opt.CloudTable = tbl;
                opt.TableEntity = (req, ctx) =>
                {
                    return new CustomerActivityEntity
                    {
                        PartitionKey = req.MessageId,
                        RowKey = Guid.NewGuid().ToString(),
                        DateOfBirth = req.FabrikamCustomer.DateOfBirth,
                        CustomerPublishedOn = DateTime.Now
                    };
                };
            });

            services.AddTransient<InsertEntityCommand<FabrikamCustomerRequest>>();

            services.AddTransient<IPipelineBehavior<FabrikamCustomerRequest, Unit>, TransformFabrikamCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<FabrikamCustomerRequest, Unit>, EnrichFabrikamCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<FabrikamCustomerRequest, Unit>, InsertEntityRequestBehavior<FabrikamCustomerRequest>>();

            return services;
        }

        public static IServiceCollection AddMultiTrackingPipeline(this IServiceCollection services)
        {
            var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;

            var messagesTable = storageAccount.CreateCloudTableClient().GetTableReference("Messages");
            messagesTable.CreateIfNotExists();

            var activitiesTable = storageAccount.CreateCloudTableClient().GetTableReference("Activities");
            activitiesTable.CreateIfNotExists();

            services.AddTransient<InsertEntityCommand<ContosoCustomerRequest>>();
            services.AddTransient<InsertEntityCommand<FabrikamCustomerRequest>>();

            services.AddOptions<TableOptions<ContosoCustomerRequest>>("Messages").Configure<IConfiguration>((opt, cfg) =>
            {
                opt.IsEnabled = cfg.GetValue<bool>("TrackingEnabled");
                opt.CloudTable = messagesTable;
            });
            services.AddOptions<TableOptions<FabrikamCustomerRequest>>("Messages").Configure<IConfiguration>((opt, cfg) =>
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
            services.AddOptions<TableOptions<ContosoCustomerRequest>>("Source").Configure<IConfiguration>((opt, cfg) =>
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
                        Email = req.ContosoCustomer.Email
                    };
                };
            });
            services.AddOptions<TableOptions<FabrikamCustomerRequest>>("Target").Configure<IConfiguration>((opt, cfg) =>
            {
                opt.IsEnabled = cfg.GetValue<bool>("TrackingEnabled");
                opt.CloudTable = activitiesTable;
                opt.TableEntity = (req, ctx) =>
                {
                    return new CustomerActivityEntity
                    {
                        PartitionKey = req.MessageId,
                        RowKey = Guid.NewGuid().ToString(),
                        DateOfBirth = req.FabrikamCustomer.DateOfBirth,
                        CustomerPublishedOn = DateTime.Now
                    };
                };
            });

            services.AddTransient<IPipelineBehavior<ContosoCustomerRequest, Unit>, InsertEntityRequestBehavior<ContosoCustomerRequest>>(sp =>
            {
                var opt = sp.GetRequiredService<IOptionsSnapshot<TableOptions<ContosoCustomerRequest>>>().Get("Messages");

                return ActivatorUtilities.CreateInstance<InsertEntityRequestBehavior<ContosoCustomerRequest>>(sp, Options.Create(opt));
            });
            services.AddTransient<IPipelineBehavior<ContosoCustomerRequest, Unit>, ValidateContosoCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<ContosoCustomerRequest, Unit>, TransformContosoCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<ContosoCustomerRequest, Unit>, InsertEntityRequestBehavior<ContosoCustomerRequest>>(sp =>
            {
                var opt = sp.GetRequiredService<IOptionsSnapshot<TableOptions<ContosoCustomerRequest>>>().Get("Messages");

                return ActivatorUtilities.CreateInstance<InsertEntityRequestBehavior<ContosoCustomerRequest>>(sp, Options.Create(opt));
            });
            services.AddTransient<IPipelineBehavior<ContosoCustomerRequest, Unit>, InsertEntityRequestBehavior<ContosoCustomerRequest>>(sp =>
            {
                var opt = sp.GetRequiredService<IOptionsSnapshot<TableOptions<ContosoCustomerRequest>>>().Get("Source");

                return ActivatorUtilities.CreateInstance<InsertEntityRequestBehavior<ContosoCustomerRequest>>(sp, Options.Create(opt));
            });

            services.AddTransient<IPipelineBehavior<FabrikamCustomerRequest, Unit>, InsertEntityRequestBehavior<FabrikamCustomerRequest>>(sp =>
            {
                var opt = sp.GetRequiredService<IOptionsSnapshot<TableOptions<FabrikamCustomerRequest>>>().Get("Messages");

                return ActivatorUtilities.CreateInstance<InsertEntityRequestBehavior<FabrikamCustomerRequest>>(sp, Options.Create(opt));
            });
            services.AddTransient<IPipelineBehavior<FabrikamCustomerRequest, Unit>, TransformFabrikamCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<FabrikamCustomerRequest, Unit>, EnrichFabrikamCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<FabrikamCustomerRequest, Unit>, InsertEntityRequestBehavior<FabrikamCustomerRequest>>(sp =>
            {
                var opt = sp.GetRequiredService<IOptionsSnapshot<TableOptions<FabrikamCustomerRequest>>>().Get("Messages");

                return ActivatorUtilities.CreateInstance<InsertEntityRequestBehavior<FabrikamCustomerRequest>>(sp, Options.Create(opt));
            });
            services.AddTransient<IPipelineBehavior<FabrikamCustomerRequest, Unit>, InsertEntityRequestBehavior<FabrikamCustomerRequest>>(sp =>
            {
                var opt = sp.GetRequiredService<IOptionsSnapshot<TableOptions<FabrikamCustomerRequest>>>().Get("Target");

                return ActivatorUtilities.CreateInstance<InsertEntityRequestBehavior<FabrikamCustomerRequest>>(sp, Options.Create(opt));
            });

            return services;
        }

        public static IServiceCollection AddBlobClaimCheckPipeline(this IServiceCollection services)
        {
            var container = new BlobContainerClient("UseDevelopmentStorage=true", "claim-checks");
            container.CreateIfNotExists();

            services.AddScoped<PipelineContext>();

            services.AddTransient<UploadBlobCommand<ContosoCustomerRequest>>();
            services.AddTransient<DownloadBlobCommand<FabrikamCustomerRequest>>();
            services.AddTransient<DeleteBlobCommand<FabrikamCustomerRequest>>();

            services.AddOptions<BlobOptions<ContosoCustomerRequest>>().Configure<IConfiguration>((opt, cfg) =>
            {
                opt.IsEnabled = cfg.GetValue<bool>("TrackingEnabled");
                opt.BlobClient = (req, ctx) => container.GetBlobClient($"customers/canonical/{req.MessageId}.json");
                opt.BlobContent = (req, ctx) =>
                {
                    var canonicalCustomer = JsonConvert.SerializeObject(req.CanonicalCustomer);

                    req.CanonicalCustomer = null;
                    req.ContosoCustomer = null;

                    return BinaryData.FromString(canonicalCustomer);
                };
            });
            services.AddOptions<BlobOptions<FabrikamCustomerRequest>>().Configure<IConfiguration>((opt, cfg) =>
            {
                opt.IsEnabled = cfg.GetValue<bool>("TrackingEnabled");
                opt.BlobClient = (req, ctx) => container.GetBlobClient($"customers/canonical/{req.MessageId}.json");
                opt.Downloaded = (res, ctx, req) =>
                {
                    var canonicalCustomer = res.Content.ToString();

                    req.CanonicalCustomer = JsonConvert.DeserializeObject<CanonicalCustomer>(canonicalCustomer);

                    return Task.CompletedTask;
                };
            });

            services.AddTransient<IPipelineBehavior<ContosoCustomerRequest, Unit>, ValidateContosoCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<ContosoCustomerRequest, Unit>, TransformContosoCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<ContosoCustomerRequest, Unit>, UploadBlobRequestBehavior<ContosoCustomerRequest>>();

            services.AddTransient<IPipelineBehavior<FabrikamCustomerRequest, Unit>, DownloadBlobRequestBehavior<FabrikamCustomerRequest>>();
            services.AddTransient<IPipelineBehavior<FabrikamCustomerRequest, Unit>, DeleteBlobRequestBehavior<FabrikamCustomerRequest>>();
            services.AddTransient<IPipelineBehavior<FabrikamCustomerRequest, Unit>, TransformFabrikamCustomerBehavior>();
            services.AddTransient<IPipelineBehavior<FabrikamCustomerRequest, Unit>, EnrichFabrikamCustomerBehavior>();

            return services;
        }
    }
}