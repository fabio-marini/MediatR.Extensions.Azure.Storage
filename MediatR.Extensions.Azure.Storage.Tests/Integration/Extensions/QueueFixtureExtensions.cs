using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using FluentAssertions;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediatR.Extensions.Azure.Storage.Tests.Integration
{
    public static class QueueFixtureExtensions
    {
        public static IServiceCollection AddQueueOptions<TRequest, TResponse>(this IServiceCollection services) where TRequest : IRequest<TResponse>
        {
            return services

                .AddOptions<QueueOptions<TRequest>>("Processors")
                .Configure<IServiceProvider>((opt, svc) =>
                {
                    var que = svc.GetRequiredService<Queue<QueueMessage>>();

                    opt.IsEnabled = true;
                    opt.QueueClient = svc.GetRequiredService<QueueClient>();
                    opt.Received = (msg, ctx, req) =>
                    {
                        var obj = JsonConvert.DeserializeObject<TRequest>(msg.Body.ToString());

                        obj.Should().NotBeNull();

                        que.Enqueue(msg);

                        return Task.CompletedTask;
                    };
                    opt.Delete = (ctx, req) => Task.FromResult(que.Dequeue());
                })
                .Services

                .AddOptions<QueueOptions<TResponse>>("Processors")
                .Configure<IServiceProvider>((opt, svc) =>
                {
                    var que = svc.GetRequiredService<Queue<QueueMessage>>();

                    opt.IsEnabled = true;
                    opt.QueueClient = svc.GetRequiredService<QueueClient>();
                    opt.Received = (msg, ctx, req) =>
                    {
                        var obj = JsonConvert.DeserializeObject<TResponse>(msg.Body.ToString());

                        obj.Should().NotBeNull();

                        que.Enqueue(msg);

                        return Task.CompletedTask;
                    };
                    opt.Delete = (ctx, req) => Task.FromResult(que.Dequeue());
                })
                .Services

                .AddOptions<QueueOptions<TRequest>>("Behaviors")
                .Configure<IServiceProvider>((opt, svc) =>
                {
                    var que = svc.GetRequiredService<Queue<QueueMessage>>();

                    opt.IsEnabled = true;
                    opt.QueueClient = svc.GetRequiredService<QueueClient>();
                    opt.Received = (msg, ctx, req) =>
                    {
                        var obj = JsonConvert.DeserializeObject<TRequest>(msg.Body.ToString());

                        obj.Should().NotBeNull();

                        que.Enqueue(msg);

                        return Task.CompletedTask;
                    };
                    opt.Delete = (ctx, req) => Task.FromResult(que.Dequeue());
                })
                .Services

                .AddOptions<QueueOptions<TResponse>>("Behaviors")
                .Configure<IServiceProvider>((opt, svc) =>
                {
                    var que = svc.GetRequiredService<Queue<QueueMessage>>();

                    opt.IsEnabled = true;
                    opt.QueueClient = svc.GetRequiredService<QueueClient>();
                    opt.Received = (msg, ctx, req) =>
                    {
                        var obj = JsonConvert.DeserializeObject<TResponse>(msg.Body.ToString());

                        obj.Should().NotBeNull();

                        que.Enqueue(msg);

                        return Task.CompletedTask;
                    };
                    opt.Delete = (ctx, req) => Task.FromResult(que.Dequeue());
                })
                .Services

                ;
        }

        public static IServiceCollection AddSendMessageExtensions<TRequest, TResponse>(this IServiceCollection services) where TRequest : IRequest<TResponse>
        {
            return services

                .AddTransient<IRequestPreProcessor<TRequest>, SendMessageRequestProcessor<TRequest>>(sp =>
                {
                    var opt = sp.GetRequiredService<IOptionsSnapshot<QueueOptions<TRequest>>>().Get("Processors");

                    var cmd = ActivatorUtilities.CreateInstance<SendMessageCommand<TRequest>>(sp, Options.Create(opt));

                    return ActivatorUtilities.CreateInstance<SendMessageRequestProcessor<TRequest>>(sp, cmd);
                })
                .AddTransient<IRequestPostProcessor<TRequest, TResponse>, SendMessageResponseProcessor<TRequest, TResponse>>(sp =>
                {
                    var opt = sp.GetRequiredService<IOptionsSnapshot<QueueOptions<TResponse>>>().Get("Processors");

                    var cmd = ActivatorUtilities.CreateInstance<SendMessageCommand<TResponse>>(sp, Options.Create(opt));

                    return ActivatorUtilities.CreateInstance<SendMessageResponseProcessor<TRequest, TResponse>>(sp, cmd);
                })
                .AddTransient<IPipelineBehavior<TRequest, TResponse>, SendMessageRequestBehavior<TRequest, TResponse>>(sp =>
                {
                    var opt = sp.GetRequiredService<IOptionsSnapshot<QueueOptions<TRequest>>>().Get("Behaviors");

                    var cmd = ActivatorUtilities.CreateInstance<SendMessageCommand<TRequest>>(sp, Options.Create(opt));

                    return ActivatorUtilities.CreateInstance<SendMessageRequestBehavior<TRequest, TResponse>>(sp, cmd);
                })
                .AddTransient<IPipelineBehavior<TRequest, TResponse>, SendMessageResponseBehavior<TRequest, TResponse>>(sp =>
                {
                    var opt = sp.GetRequiredService<IOptionsSnapshot<QueueOptions<TResponse>>>().Get("Behaviors");

                    var cmd = ActivatorUtilities.CreateInstance<SendMessageCommand<TResponse>>(sp, Options.Create(opt));

                    return ActivatorUtilities.CreateInstance<SendMessageResponseBehavior<TRequest, TResponse>>(sp, cmd);
                })

                ;
        }

        public static IServiceCollection AddReceiveMessageExtensions<TRequest, TResponse>(this IServiceCollection services) where TRequest : IRequest<TResponse>
        {
            return services

                .AddTransient<IRequestPreProcessor<TRequest>, ReceiveMessageRequestProcessor<TRequest>>(sp =>
                {
                    var opt = sp.GetRequiredService<IOptionsSnapshot<QueueOptions<TRequest>>>().Get("Processors");

                    var cmd = ActivatorUtilities.CreateInstance<ReceiveMessageCommand<TRequest>>(sp, Options.Create(opt));

                    return ActivatorUtilities.CreateInstance<ReceiveMessageRequestProcessor<TRequest>>(sp, cmd);
                })
                .AddTransient<IRequestPostProcessor<TRequest, TResponse>, ReceiveMessageResponseProcessor<TRequest, TResponse>>(sp =>
                {
                    var opt = sp.GetRequiredService<IOptionsSnapshot<QueueOptions<TResponse>>>().Get("Processors");

                    var cmd = ActivatorUtilities.CreateInstance<ReceiveMessageCommand<TResponse>>(sp, Options.Create(opt));

                    return ActivatorUtilities.CreateInstance<ReceiveMessageResponseProcessor<TRequest, TResponse>>(sp, cmd);
                })
                .AddTransient<IPipelineBehavior<TRequest, TResponse>, ReceiveMessageRequestBehavior<TRequest, TResponse>>(sp =>
                {
                    var opt = sp.GetRequiredService<IOptionsSnapshot<QueueOptions<TRequest>>>().Get("Behaviors");

                    var cmd = ActivatorUtilities.CreateInstance<ReceiveMessageCommand<TRequest>>(sp, Options.Create(opt));

                    return ActivatorUtilities.CreateInstance<ReceiveMessageRequestBehavior<TRequest, TResponse>>(sp, cmd);
                })
                .AddTransient<IPipelineBehavior<TRequest, TResponse>, ReceiveMessageResponseBehavior<TRequest, TResponse>>(sp =>
                {
                    var opt = sp.GetRequiredService<IOptionsSnapshot<QueueOptions<TResponse>>>().Get("Behaviors");

                    var cmd = ActivatorUtilities.CreateInstance<ReceiveMessageCommand<TResponse>>(sp, Options.Create(opt));

                    return ActivatorUtilities.CreateInstance<ReceiveMessageResponseBehavior<TRequest, TResponse>>(sp, cmd);
                })

                ;
        }

        public static IServiceCollection AddDeleteMessageExtensions<TRequest, TResponse>(this IServiceCollection services) where TRequest : IRequest<TResponse>
        {
            return services

                .AddTransient<IRequestPreProcessor<TRequest>, DeleteMessageRequestProcessor<TRequest>>(sp =>
                {
                    var opt = sp.GetRequiredService<IOptionsSnapshot<QueueOptions<TRequest>>>().Get("Processors");

                    var cmd = ActivatorUtilities.CreateInstance<DeleteMessageCommand<TRequest>>(sp, Options.Create(opt));

                    return ActivatorUtilities.CreateInstance<DeleteMessageRequestProcessor<TRequest>>(sp, cmd);
                })
                .AddTransient<IRequestPostProcessor<TRequest, TResponse>, DeleteMessageResponseProcessor<TRequest, TResponse>>(sp =>
                {
                    var opt = sp.GetRequiredService<IOptionsSnapshot<QueueOptions<TResponse>>>().Get("Processors");

                    var cmd = ActivatorUtilities.CreateInstance<DeleteMessageCommand<TResponse>>(sp, Options.Create(opt));

                    return ActivatorUtilities.CreateInstance<DeleteMessageResponseProcessor<TRequest, TResponse>>(sp, cmd);
                })
                .AddTransient<IPipelineBehavior<TRequest, TResponse>, DeleteMessageRequestBehavior<TRequest, TResponse>>(sp =>
                {
                    var opt = sp.GetRequiredService<IOptionsSnapshot<QueueOptions<TRequest>>>().Get("Behaviors");

                    var cmd = ActivatorUtilities.CreateInstance<DeleteMessageCommand<TRequest>>(sp, Options.Create(opt));

                    return ActivatorUtilities.CreateInstance<DeleteMessageRequestBehavior<TRequest, TResponse>>(sp, cmd);
                })
                .AddTransient<IPipelineBehavior<TRequest, TResponse>, DeleteMessageResponseBehavior<TRequest, TResponse>>(sp =>
                {
                    var opt = sp.GetRequiredService<IOptionsSnapshot<QueueOptions<TResponse>>>().Get("Behaviors");

                    var cmd = ActivatorUtilities.CreateInstance<DeleteMessageCommand<TResponse>>(sp, Options.Create(opt));

                    return ActivatorUtilities.CreateInstance<DeleteMessageResponseBehavior<TRequest, TResponse>>(sp, cmd);
                })

                ;
        }
    }
}
