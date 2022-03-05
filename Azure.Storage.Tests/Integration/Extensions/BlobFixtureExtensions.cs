using Azure.Storage.Blobs;
using FluentAssertions;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace MediatR.Extensions.Azure.Storage.Tests.Integration
{
    public static class BlobFixtureExtensions
    {
        public static IServiceCollection AddBlobOptions(this IServiceCollection services) => services.AddBlobOptions<EchoRequest, EchoResponse>();

        public static IServiceCollection AddUploadBlobExtensions(this IServiceCollection services) => services.AddUploadBlobExtensions<EchoRequest, EchoResponse>();

        public static IServiceCollection AddDownloadBlobExtensions(this IServiceCollection services) => services.AddDownloadBlobExtensions<EchoRequest, EchoResponse>();

        public static IServiceCollection AddDeleteBlobExtensions(this IServiceCollection services) => services.AddDeleteBlobExtensions<EchoRequest, EchoResponse>();

        private static IServiceCollection AddBlobOptions<TRequest, TResponse>(this IServiceCollection services) where TRequest : IRequest<TResponse>
        {
            return services

                .AddOptions<BlobOptions<TRequest>>("Processors")
                .Configure<IServiceProvider>((opt, svc) =>
                {
                    var container = svc.GetRequiredService<BlobContainerClient>();

                    opt.IsEnabled = true;
                    opt.BlobClient = (req, ctx) => container.GetBlobClient($"processors/test.request.json");
                    opt.Downloaded = (res, ctx, req) =>
                    {
                        var obj = JsonConvert.DeserializeObject<TRequest>(res.Content.ToString());

                        obj.Should().NotBeNull();

                        ctx.Add("ProcessorRequest", obj);

                        return Task.CompletedTask;
                    };
                })
                .Services

                .AddOptions<BlobOptions<TResponse>>("Processors")
                .Configure<IServiceProvider>((opt, svc) =>
                {
                    var container = svc.GetRequiredService<BlobContainerClient>();

                    opt.IsEnabled = true;
                    opt.BlobClient = (req, ctx) => container.GetBlobClient($"processors/test.response.json");
                    opt.Downloaded = (res, ctx, req) =>
                    {
                        var obj = JsonConvert.DeserializeObject<TResponse>(res.Content.ToString());

                        obj.Should().NotBeNull();

                        ctx.Add("ProcessorResponse", obj);

                        return Task.CompletedTask;
                    };
                })
                .Services

                .AddOptions<BlobOptions<TRequest>>("Behaviors")
                .Configure<IServiceProvider>((opt, svc) =>
                {
                    var container = svc.GetRequiredService<BlobContainerClient>();

                    opt.IsEnabled = true;
                    opt.BlobClient = (req, ctx) => container.GetBlobClient($"behaviors/test.request.json");
                    opt.Downloaded = (res, ctx, req) =>
                    {
                        var obj = JsonConvert.DeserializeObject<TRequest>(res.Content.ToString());

                        obj.Should().NotBeNull();

                        ctx.Add("BehaviorRequest", obj);

                        return Task.CompletedTask;
                    };
                })
                .Services

                .AddOptions<BlobOptions<TResponse>>("Behaviors")
                .Configure<IServiceProvider>((opt, svc) =>
                {
                    var container = svc.GetRequiredService<BlobContainerClient>();

                    opt.IsEnabled = true;
                    opt.BlobClient = (req, ctx) => container.GetBlobClient($"behaviors/test.response.json");
                    opt.Downloaded = (res, ctx, req) =>
                    {
                        var obj = JsonConvert.DeserializeObject<TResponse>(res.Content.ToString());

                        obj.Should().NotBeNull();

                        ctx.Add("BehaviorResponse", obj);

                        return Task.CompletedTask;
                    };
                })
                .Services

                ;
        }

        private static IServiceCollection AddUploadBlobExtensions<TRequest, TResponse>(this IServiceCollection services) where TRequest : IRequest<TResponse>
        {
            return services

                .AddTransient<IRequestPreProcessor<TRequest>, UploadBlobRequestProcessor<TRequest>>(sp =>
                {
                    var opt = sp.GetRequiredService<IOptionsSnapshot<BlobOptions<TRequest>>>().Get("Processors");

                    var cmd = ActivatorUtilities.CreateInstance<UploadBlobCommand<TRequest>>(sp, Options.Create(opt));

                    return ActivatorUtilities.CreateInstance<UploadBlobRequestProcessor<TRequest>>(sp, cmd);
                })
                .AddTransient<IRequestPostProcessor<TRequest, TResponse>, UploadBlobResponseProcessor<TRequest, TResponse>>(sp =>
                {
                    var opt = sp.GetRequiredService<IOptionsSnapshot<BlobOptions<TResponse>>>().Get("Processors");

                    var cmd = ActivatorUtilities.CreateInstance<UploadBlobCommand<TResponse>>(sp, Options.Create(opt));

                    return ActivatorUtilities.CreateInstance<UploadBlobResponseProcessor<TRequest, TResponse>>(sp, cmd);
                })
                .AddTransient<IPipelineBehavior<TRequest, TResponse>, UploadBlobRequestBehavior<TRequest, TResponse>>(sp =>
                {
                    var opt = sp.GetRequiredService<IOptionsSnapshot<BlobOptions<TRequest>>>().Get("Behaviors");

                    var cmd = ActivatorUtilities.CreateInstance<UploadBlobCommand<TRequest>>(sp, Options.Create(opt));

                    return ActivatorUtilities.CreateInstance<UploadBlobRequestBehavior<TRequest, TResponse>>(sp, cmd);
                })
                .AddTransient<IPipelineBehavior<TRequest, TResponse>, UploadBlobResponseBehavior<TRequest, TResponse>>(sp =>
                {
                    var opt = sp.GetRequiredService<IOptionsSnapshot<BlobOptions<TResponse>>>().Get("Behaviors");

                    var cmd = ActivatorUtilities.CreateInstance<UploadBlobCommand<TResponse>>(sp, Options.Create(opt));

                    return ActivatorUtilities.CreateInstance<UploadBlobResponseBehavior<TRequest, TResponse>>(sp, cmd);
                })

                ;
        }

        private static IServiceCollection AddDownloadBlobExtensions<TRequest, TResponse>(this IServiceCollection services) where TRequest : IRequest<TResponse>
        {
            return services

                .AddTransient<IRequestPreProcessor<TRequest>, DownloadBlobRequestProcessor<TRequest>>(sp =>
                {
                    var opt = sp.GetRequiredService<IOptionsSnapshot<BlobOptions<TRequest>>>().Get("Processors");

                    var cmd = ActivatorUtilities.CreateInstance<DownloadBlobCommand<TRequest>>(sp, Options.Create(opt));

                    return ActivatorUtilities.CreateInstance<DownloadBlobRequestProcessor<TRequest>>(sp, cmd);
                })
                .AddTransient<IRequestPostProcessor<TRequest, TResponse>, DownloadBlobResponseProcessor<TRequest, TResponse>>(sp =>
                {
                    var opt = sp.GetRequiredService<IOptionsSnapshot<BlobOptions<TResponse>>>().Get("Processors");

                    var cmd = ActivatorUtilities.CreateInstance<DownloadBlobCommand<TResponse>>(sp, Options.Create(opt));

                    return ActivatorUtilities.CreateInstance<DownloadBlobResponseProcessor<TRequest, TResponse>>(sp, cmd);
                })
                .AddTransient<IPipelineBehavior<TRequest, TResponse>, DownloadBlobRequestBehavior<TRequest, TResponse>>(sp =>
                {
                    var opt = sp.GetRequiredService<IOptionsSnapshot<BlobOptions<TRequest>>>().Get("Behaviors");

                    var cmd = ActivatorUtilities.CreateInstance<DownloadBlobCommand<TRequest>>(sp, Options.Create(opt));

                    return ActivatorUtilities.CreateInstance<DownloadBlobRequestBehavior<TRequest, TResponse>>(sp, cmd);
                })
                .AddTransient<IPipelineBehavior<TRequest, TResponse>, DownloadBlobResponseBehavior<TRequest, TResponse>>(sp =>
                {
                    var opt = sp.GetRequiredService<IOptionsSnapshot<BlobOptions<TResponse>>>().Get("Behaviors");

                    var cmd = ActivatorUtilities.CreateInstance<DownloadBlobCommand<TResponse>>(sp, Options.Create(opt));

                    return ActivatorUtilities.CreateInstance<DownloadBlobResponseBehavior<TRequest, TResponse>>(sp, cmd);
                })

                ;
        }

        private static IServiceCollection AddDeleteBlobExtensions<TRequest, TResponse>(this IServiceCollection services) where TRequest : IRequest<TResponse>
        {
            return services

                .AddTransient<IRequestPreProcessor<TRequest>, DeleteBlobRequestProcessor<TRequest>>(sp =>
                {
                    var opt = sp.GetRequiredService<IOptionsSnapshot<BlobOptions<TRequest>>>().Get("Processors");

                    var cmd = ActivatorUtilities.CreateInstance<DeleteBlobCommand<TRequest>>(sp, Options.Create(opt));

                    return ActivatorUtilities.CreateInstance<DeleteBlobRequestProcessor<TRequest>>(sp, cmd);
                })
                .AddTransient<IRequestPostProcessor<TRequest, TResponse>, DeleteBlobResponseProcessor<TRequest, TResponse>>(sp =>
                {
                    var opt = sp.GetRequiredService<IOptionsSnapshot<BlobOptions<TResponse>>>().Get("Processors");

                    var cmd = ActivatorUtilities.CreateInstance<DeleteBlobCommand<TResponse>>(sp, Options.Create(opt));

                    return ActivatorUtilities.CreateInstance<DeleteBlobResponseProcessor<TRequest, TResponse>>(sp, cmd);
                })
                .AddTransient<IPipelineBehavior<TRequest, TResponse>, DeleteBlobRequestBehavior<TRequest, TResponse>>(sp =>
                {
                    var opt = sp.GetRequiredService<IOptionsSnapshot<BlobOptions<TRequest>>>().Get("Behaviors");

                    var cmd = ActivatorUtilities.CreateInstance<DeleteBlobCommand<TRequest>>(sp, Options.Create(opt));

                    return ActivatorUtilities.CreateInstance<DeleteBlobRequestBehavior<TRequest, TResponse>>(sp, cmd);
                })
                .AddTransient<IPipelineBehavior<TRequest, TResponse>, DeleteBlobResponseBehavior<TRequest, TResponse>>(sp =>
                {
                    var opt = sp.GetRequiredService<IOptionsSnapshot<BlobOptions<TResponse>>>().Get("Behaviors");

                    var cmd = ActivatorUtilities.CreateInstance<DeleteBlobCommand<TResponse>>(sp, Options.Create(opt));

                    return ActivatorUtilities.CreateInstance<DeleteBlobResponseBehavior<TRequest, TResponse>>(sp, cmd);
                })

                ;
        }
    }
}
