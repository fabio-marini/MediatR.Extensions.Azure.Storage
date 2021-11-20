using FluentAssertions;
using MediatR.Pipeline;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace MediatR.Extensions.Azure.Storage.Tests.Integration
{
    public static class TableFixtureExtensions
    {
        public static IServiceCollection AddTableOptions<TRequest, TResponse>(this IServiceCollection services) where TRequest : IRequest<TResponse>
        {
            return services

                .AddOptions<TableOptions<TRequest>>("Processors")
                .Configure<IServiceProvider>((opt, svc) =>
                {
                    opt.IsEnabled = true;
                    opt.CloudTable = svc.GetRequiredService<CloudTable>();
                    opt.TableEntity = (req, ctx) =>
                    {
                        var tableEntity = new DynamicTableEntity("Processors", "Request") { ETag = "*" };

                        tableEntity.Properties.Add("Content", EntityProperty.GeneratePropertyForString(JsonConvert.SerializeObject(req)));

                        return tableEntity;
                    };
                    opt.Retrieved = (res, ctx, req) =>
                    {
                        var tableEntity = res.Result as DynamicTableEntity;

                        tableEntity.Should().NotBeNull();

                        var obj = JsonConvert.DeserializeObject<TRequest>(tableEntity.Properties["Content"].StringValue);

                        obj.Should().NotBeNull();

                        ctx.Add("ProcessorRequest", obj);

                        return Task.CompletedTask;
                    };
                })
                .Services

                .AddOptions<TableOptions<TResponse>>("Processors")
                .Configure<IServiceProvider>((opt, svc) =>
                {
                    opt.IsEnabled = true;
                    opt.CloudTable = svc.GetRequiredService<CloudTable>();
                    opt.TableEntity = (req, ctx) =>
                    {
                        var tableEntity = new DynamicTableEntity("Processors", "Response") { ETag = "*" };

                        tableEntity.Properties.Add("Content", EntityProperty.GeneratePropertyForString(JsonConvert.SerializeObject(req)));

                        return tableEntity;
                    };
                    opt.Retrieved = (res, ctx, req) =>
                    {
                        var tableEntity = res.Result as DynamicTableEntity;

                        tableEntity.Should().NotBeNull();

                        var obj = JsonConvert.DeserializeObject<TResponse>(tableEntity.Properties["Content"].StringValue);

                        obj.Should().NotBeNull();

                        ctx.Add("ProcessorResponse", obj);

                        return Task.CompletedTask;
                    };
                })
                .Services

                .AddOptions<TableOptions<TRequest>>("Behaviors")
                .Configure<IServiceProvider>((opt, svc) =>
                {
                    opt.IsEnabled = true;
                    opt.CloudTable = svc.GetRequiredService<CloudTable>();
                    opt.TableEntity = (req, ctx) =>
                    {
                        var tableEntity = new DynamicTableEntity("Behaviors", "Request") { ETag = "*" };

                        tableEntity.Properties.Add("Content", EntityProperty.GeneratePropertyForString(JsonConvert.SerializeObject(req)));

                        return tableEntity;
                    };
                    opt.Retrieved = (res, ctx, req) =>
                    {
                        var tableEntity = res.Result as DynamicTableEntity;

                        tableEntity.Should().NotBeNull();

                        var obj = JsonConvert.DeserializeObject<TRequest>(tableEntity.Properties["Content"].StringValue);

                        obj.Should().NotBeNull();

                        ctx.Add("BehaviorRequest", obj);

                        return Task.CompletedTask;
                    };
                })
                .Services

                .AddOptions<TableOptions<TResponse>>("Behaviors")
                .Configure<IServiceProvider>((opt, svc) =>
                {
                    opt.IsEnabled = true;
                    opt.CloudTable = svc.GetRequiredService<CloudTable>();
                    opt.TableEntity = (req, ctx) =>
                    {
                        var tableEntity = new DynamicTableEntity("Behaviors", "Response") { ETag = "*" };

                        tableEntity.Properties.Add("Content", EntityProperty.GeneratePropertyForString(JsonConvert.SerializeObject(req)));

                        return tableEntity;
                    };
                    opt.Retrieved = (res, ctx, req) =>
                    {
                        var tableEntity = res.Result as DynamicTableEntity;

                        tableEntity.Should().NotBeNull();

                        var obj = JsonConvert.DeserializeObject<TResponse>(tableEntity.Properties["Content"].StringValue);

                        obj.Should().NotBeNull();

                        ctx.Add("BehaviorResponse", obj);

                        return Task.CompletedTask;
                    };
                })
                .Services

                ;
        }

        public static IServiceCollection AddInsertEntityExtensions<TRequest, TResponse>(this IServiceCollection services) where TRequest : IRequest<TResponse>
        {
            return services

                .AddTransient<IRequestPreProcessor<TRequest>, InsertEntityRequestProcessor<TRequest>>(sp =>
                {
                    var opt = sp.GetRequiredService<IOptionsSnapshot<TableOptions<TRequest>>>().Get("Processors");

                    var cmd = ActivatorUtilities.CreateInstance<InsertEntityCommand<TRequest>>(sp, Options.Create(opt));

                    return ActivatorUtilities.CreateInstance<InsertEntityRequestProcessor<TRequest>>(sp, cmd);
                })
                .AddTransient<IRequestPostProcessor<TRequest, TResponse>, InsertEntityResponseProcessor<TRequest, TResponse>>(sp =>
                {
                    var opt = sp.GetRequiredService<IOptionsSnapshot<TableOptions<TResponse>>>().Get("Processors");

                    var cmd = ActivatorUtilities.CreateInstance<InsertEntityCommand<TResponse>>(sp, Options.Create(opt));

                    return ActivatorUtilities.CreateInstance<InsertEntityResponseProcessor<TRequest, TResponse>>(sp, cmd);
                })
                .AddTransient<IPipelineBehavior<TRequest, TResponse>, InsertEntityRequestBehavior<TRequest, TResponse>>(sp =>
                {
                    var opt = sp.GetRequiredService<IOptionsSnapshot<TableOptions<TRequest>>>().Get("Behaviors");

                    var cmd = ActivatorUtilities.CreateInstance<InsertEntityCommand<TRequest>>(sp, Options.Create(opt));

                    return ActivatorUtilities.CreateInstance<InsertEntityRequestBehavior<TRequest, TResponse>>(sp, cmd);
                })
                .AddTransient<IPipelineBehavior<TRequest, TResponse>, InsertEntityResponseBehavior<TRequest, TResponse>>(sp =>
                {
                    var opt = sp.GetRequiredService<IOptionsSnapshot<TableOptions<TResponse>>>().Get("Behaviors");

                    var cmd = ActivatorUtilities.CreateInstance<InsertEntityCommand<TResponse>>(sp, Options.Create(opt));

                    return ActivatorUtilities.CreateInstance<InsertEntityResponseBehavior<TRequest, TResponse>>(sp, cmd);
                })

                ;
        }

        public static IServiceCollection AddRetrieveEntityExtensions<TRequest, TResponse>(this IServiceCollection services) where TRequest : IRequest<TResponse>
        {
            return services

                .AddTransient<IRequestPreProcessor<TRequest>, RetrieveEntityRequestProcessor<TRequest>>(sp =>
                {
                    var opt = sp.GetRequiredService<IOptionsSnapshot<TableOptions<TRequest>>>().Get("Processors");

                    var cmd = ActivatorUtilities.CreateInstance<RetrieveEntityCommand<TRequest>>(sp, Options.Create(opt));

                    return ActivatorUtilities.CreateInstance<RetrieveEntityRequestProcessor<TRequest>>(sp, cmd);
                })
                .AddTransient<IRequestPostProcessor<TRequest, TResponse>, RetrieveEntityResponseProcessor<TRequest, TResponse>>(sp =>
                {
                    var opt = sp.GetRequiredService<IOptionsSnapshot<TableOptions<TResponse>>>().Get("Processors");

                    var cmd = ActivatorUtilities.CreateInstance<RetrieveEntityCommand<TResponse>>(sp, Options.Create(opt));

                    return ActivatorUtilities.CreateInstance<RetrieveEntityResponseProcessor<TRequest, TResponse>>(sp, cmd);
                })
                .AddTransient<IPipelineBehavior<TRequest, TResponse>, RetrieveEntityRequestBehavior<TRequest, TResponse>>(sp =>
                {
                    var opt = sp.GetRequiredService<IOptionsSnapshot<TableOptions<TRequest>>>().Get("Behaviors");

                    var cmd = ActivatorUtilities.CreateInstance<RetrieveEntityCommand<TRequest>>(sp, Options.Create(opt));

                    return ActivatorUtilities.CreateInstance<RetrieveEntityRequestBehavior<TRequest, TResponse>>(sp, cmd);
                })
                .AddTransient<IPipelineBehavior<TRequest, TResponse>, RetrieveEntityResponseBehavior<TRequest, TResponse>>(sp =>
                {
                    var opt = sp.GetRequiredService<IOptionsSnapshot<TableOptions<TResponse>>>().Get("Behaviors");

                    var cmd = ActivatorUtilities.CreateInstance<RetrieveEntityCommand<TResponse>>(sp, Options.Create(opt));

                    return ActivatorUtilities.CreateInstance<RetrieveEntityResponseBehavior<TRequest, TResponse>>(sp, cmd);
                })

                ;
        }

        public static IServiceCollection AddDeleteEntityExtensions<TRequest, TResponse>(this IServiceCollection services) where TRequest : IRequest<TResponse>
        {
            return services

                .AddTransient<IRequestPreProcessor<TRequest>, DeleteEntityRequestProcessor<TRequest>>(sp =>
                {
                    var opt = sp.GetRequiredService<IOptionsSnapshot<TableOptions<TRequest>>>().Get("Processors");

                    var cmd = ActivatorUtilities.CreateInstance<DeleteEntityCommand<TRequest>>(sp, Options.Create(opt));

                    return ActivatorUtilities.CreateInstance<DeleteEntityRequestProcessor<TRequest>>(sp, cmd);
                })
                .AddTransient<IRequestPostProcessor<TRequest, TResponse>, DeleteEntityResponseProcessor<TRequest, TResponse>>(sp =>
                {
                    var opt = sp.GetRequiredService<IOptionsSnapshot<TableOptions<TResponse>>>().Get("Processors");

                    var cmd = ActivatorUtilities.CreateInstance<DeleteEntityCommand<TResponse>>(sp, Options.Create(opt));

                    return ActivatorUtilities.CreateInstance<DeleteEntityResponseProcessor<TRequest, TResponse>>(sp, cmd);
                })
                .AddTransient<IPipelineBehavior<TRequest, TResponse>, DeleteEntityRequestBehavior<TRequest, TResponse>>(sp =>
                {
                    var opt = sp.GetRequiredService<IOptionsSnapshot<TableOptions<TRequest>>>().Get("Behaviors");

                    var cmd = ActivatorUtilities.CreateInstance<DeleteEntityCommand<TRequest>>(sp, Options.Create(opt));

                    return ActivatorUtilities.CreateInstance<DeleteEntityRequestBehavior<TRequest, TResponse>>(sp, cmd);
                })
                .AddTransient<IPipelineBehavior<TRequest, TResponse>, DeleteEntityResponseBehavior<TRequest, TResponse>>(sp =>
                {
                    var opt = sp.GetRequiredService<IOptionsSnapshot<TableOptions<TResponse>>>().Get("Behaviors");

                    var cmd = ActivatorUtilities.CreateInstance<DeleteEntityCommand<TResponse>>(sp, Options.Create(opt));

                    return ActivatorUtilities.CreateInstance<DeleteEntityResponseBehavior<TRequest, TResponse>>(sp, cmd);
                })

                ;
        }
    }
}
