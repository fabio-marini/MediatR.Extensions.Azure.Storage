using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace MediatR.Extensions.Azure.Storage.Tests
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBlobExtensions<TRequest, TResponse>(this IServiceCollection services, Func<IServiceProvider, IOptions<BlobOptions<TRequest>>> options) where TRequest : IRequest<TResponse>
        {
            return services

                .AddTransient<IOptions<BlobOptions<TRequest>>>(sp => options(sp))
                .AddTransient<UploadBlobCommand<TRequest>>()
                .AddTransient<DownloadBlobCommand<TRequest>>()
                .AddTransient<DeleteBlobCommand<TRequest>>()

                .AddTransient<UploadBlobRequestBehavior<TRequest, TResponse>>()
                .AddTransient<DownloadBlobRequestBehavior<TRequest, TResponse>>()
                .AddTransient<DeleteBlobRequestBehavior<TRequest, TResponse>>()

                .AddTransient<UploadBlobRequestProcessor<TRequest>>()
                .AddTransient<DownloadBlobRequestProcessor<TRequest>>()
                .AddTransient<DeleteBlobRequestProcessor<TRequest>>()

                ;
        }

        public static IServiceCollection AddBlobExtensions<TRequest, TResponse>(this IServiceCollection services, Func<IServiceProvider, IOptions<BlobOptions<TResponse>>> options) where TRequest : IRequest<TResponse>
        {
            return services

                .AddTransient<IOptions<BlobOptions<TResponse>>>(sp => options(sp))
                .AddTransient<UploadBlobCommand<TResponse>>()
                .AddTransient<DownloadBlobCommand<TResponse>>()
                .AddTransient<DeleteBlobCommand<TResponse>>()

                .AddTransient<UploadBlobResponseBehavior<TRequest, TResponse>>()
                .AddTransient<DownloadBlobResponseBehavior<TRequest, TResponse>>()
                .AddTransient<DeleteBlobResponseBehavior<TRequest, TResponse>>()

                .AddTransient<UploadBlobResponseProcessor<TRequest, TResponse>>()
                .AddTransient<DownloadBlobResponseProcessor<TRequest, TResponse>>()
                .AddTransient<DeleteBlobResponseProcessor<TRequest, TResponse>>()

                ;
        }

        public static IServiceCollection AddTableExtensions<TRequest, TResponse>(this IServiceCollection services, Func<IServiceProvider, IOptions<TableOptions<TRequest>>> options) where TRequest : IRequest<TResponse>
        {
            return services

                .AddTransient<IOptions<TableOptions<TRequest>>>(sp => options(sp))
                .AddTransient<InsertEntityCommand<TRequest>>()
                .AddTransient<RetrieveEntityCommand<TRequest>>()
                .AddTransient<DeleteEntityCommand<TRequest>>()

                .AddTransient<InsertEntityRequestBehavior<TRequest, TResponse>>()
                .AddTransient<RetrieveEntityRequestBehavior<TRequest, TResponse>>()
                .AddTransient<DeleteEntityRequestBehavior<TRequest, TResponse>>()

                .AddTransient<InsertEntityRequestProcessor<TRequest>>()
                .AddTransient<RetrieveEntityRequestProcessor<TRequest>>()
                .AddTransient<DeleteEntityRequestProcessor<TRequest>>()

                ;
        }

        public static IServiceCollection AddTableExtensions<TRequest, TResponse>(this IServiceCollection services, Func<IServiceProvider, IOptions<TableOptions<TResponse>>> options) where TRequest : IRequest<TResponse>
        {
            return services

                .AddTransient<IOptions<TableOptions<TResponse>>>(sp => options(sp))
                .AddTransient<InsertEntityCommand<TResponse>>()
                .AddTransient<RetrieveEntityCommand<TResponse>>()
                .AddTransient<DeleteEntityCommand<TResponse>>()

                .AddTransient<InsertEntityResponseBehavior<TRequest, TResponse>>()
                .AddTransient<RetrieveEntityResponseBehavior<TRequest, TResponse>>()
                .AddTransient<DeleteEntityResponseBehavior<TRequest, TResponse>>()

                .AddTransient<InsertEntityResponseProcessor<TRequest, TResponse>>()
                .AddTransient<RetrieveEntityResponseProcessor<TRequest, TResponse>>()
                .AddTransient<DeleteEntityResponseProcessor<TRequest, TResponse>>()

                ;
        }

        public static IServiceCollection AddQueueExtensions<TRequest, TResponse>(this IServiceCollection services, Func<IServiceProvider, IOptions<QueueOptions<TRequest>>> options) where TRequest : IRequest<TResponse>
        {
            return services

                .AddTransient<IOptions<QueueOptions<TRequest>>>(sp => options(sp))
                .AddTransient<SendMessageCommand<TRequest>>()
                .AddTransient<ReceiveMessageCommand<TRequest>>()
                .AddTransient<DeleteMessageCommand<TRequest>>()

                .AddTransient<SendMessageRequestBehavior<TRequest, TResponse>>()
                .AddTransient<ReceiveMessageRequestBehavior<TRequest, TResponse>>()
                .AddTransient<DeleteMessageRequestBehavior<TRequest, TResponse>>()

                .AddTransient<SendMessageRequestProcessor<TRequest>>()
                .AddTransient<ReceiveMessageRequestProcessor<TRequest>>()
                .AddTransient<DeleteMessageRequestProcessor<TRequest>>()

                ;
        }

        public static IServiceCollection AddQueueExtensions<TRequest, TResponse>(this IServiceCollection services, Func<IServiceProvider, IOptions<QueueOptions<TResponse>>> options) where TRequest : IRequest<TResponse>
        {
            return services

                .AddTransient<IOptions<QueueOptions<TResponse>>>(sp => options(sp))
                .AddTransient<SendMessageCommand<TResponse>>()
                .AddTransient<ReceiveMessageCommand<TResponse>>()
                .AddTransient<DeleteMessageCommand<TResponse>>()

                .AddTransient<SendMessageResponseBehavior<TRequest, TResponse>>()
                .AddTransient<ReceiveMessageResponseBehavior<TRequest, TResponse>>()
                .AddTransient<DeleteMessageResponseBehavior<TRequest, TResponse>>()

                .AddTransient<SendMessageResponseProcessor<TRequest, TResponse>>()
                .AddTransient<ReceiveMessageResponseProcessor<TRequest, TResponse>>()
                .AddTransient<DeleteMessageResponseProcessor<TRequest, TResponse>>()

                ;
        }
    }
}
