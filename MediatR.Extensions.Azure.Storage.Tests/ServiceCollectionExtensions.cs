using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using System;

namespace MediatR.Extensions.Azure.Storage.Tests
{
    public static class ServiceCollectionExtensions
    {
        #region Unit tests

        // for unit tests - options don't matter, commands are mocked
        public static IServiceCollection AddTableExtensions<TRequest, TResponse>(this IServiceCollection services) where TRequest : IRequest<TResponse>
        {
            return services

                .AddOptions<TableOptions<TRequest>>().Services
                .AddOptions<TableOptions<TResponse>>().Services

                .AddSingleton<Mock<InsertEntityCommand<TRequest>>>(sp =>
                {
                    var opt = sp.GetRequiredService<IOptions<TableOptions<TRequest>>>();

                    return new Mock<InsertEntityCommand<TRequest>>(opt, null, null);
                })
                .AddSingleton<Mock<InsertEntityCommand<TResponse>>>(sp =>
                {
                    var opt = sp.GetRequiredService<IOptions<TableOptions<TResponse>>>();

                    return new Mock<InsertEntityCommand<TResponse>>(opt, null, null);
                })

                .AddTransient<InsertEntityCommand<TRequest>>(sp => sp.GetRequiredService<Mock<InsertEntityCommand<TRequest>>>().Object)
                .AddTransient<InsertEntityCommand<TResponse>>(sp => sp.GetRequiredService<Mock<InsertEntityCommand<TResponse>>>().Object)

                .AddTransient<InsertEntityRequestBehavior<TRequest, TResponse>>()
                .AddTransient<InsertEntityResponseBehavior<TRequest, TResponse>>()
                .AddTransient<InsertEntityRequestProcessor<TRequest>>()
                .AddTransient<InsertEntityResponseProcessor<TRequest, TResponse>>()

                ;
        }

        public static IServiceCollection AddQueueExtensions<TRequest, TResponse>(this IServiceCollection services) where TRequest : IRequest<TResponse>
        {
            return services

                .AddOptions<QueueOptions<TRequest>>().Services
                .AddOptions<QueueOptions<TResponse>>().Services

                .AddSingleton<Mock<SendMessageCommand<TRequest>>>(sp =>
                {
                    var opt = sp.GetRequiredService<IOptions<QueueOptions<TRequest>>>();

                    return new Mock<SendMessageCommand<TRequest>>(opt, null, null);
                })
                .AddSingleton<Mock<SendMessageCommand<TResponse>>>(sp =>
                {
                    var opt = sp.GetRequiredService<IOptions<QueueOptions<TResponse>>>();

                    return new Mock<SendMessageCommand<TResponse>>(opt, null, null);
                })

                .AddTransient<SendMessageCommand<TRequest>>(sp => sp.GetRequiredService<Mock<SendMessageCommand<TRequest>>>().Object)
                .AddTransient<SendMessageCommand<TResponse>>(sp => sp.GetRequiredService<Mock<SendMessageCommand<TResponse>>>().Object)

                .AddTransient<SendMessageRequestBehavior<TRequest, TResponse>>()
                .AddTransient<SendMessageResponseBehavior<TRequest, TResponse>>()
                .AddTransient<SendMessageRequestProcessor<TRequest>>()
                .AddTransient<SendMessageResponseProcessor<TRequest, TResponse>>()

                ;
        }

        public static IServiceCollection AddBlobExtensions<TRequest, TResponse>(this IServiceCollection services) where TRequest : IRequest<TResponse>
        {
            return services

                .AddOptions<BlobOptions<TRequest>>().Services
                .AddOptions<BlobOptions<TResponse>>().Services

                .AddSingleton<Mock<UploadBlobCommand<TRequest>>>(sp =>
                {
                    var opt = sp.GetRequiredService<IOptions<BlobOptions<TRequest>>>();

                    return new Mock<UploadBlobCommand<TRequest>>(opt, null, null);
                })
                .AddSingleton<Mock<UploadBlobCommand<TResponse>>>(sp =>
                {
                    var opt = sp.GetRequiredService<IOptions<BlobOptions<TResponse>>>();

                    return new Mock<UploadBlobCommand<TResponse>>(opt, null, null);
                })

                .AddTransient<UploadBlobCommand<TRequest>>(sp => sp.GetRequiredService<Mock<UploadBlobCommand<TRequest>>>().Object)
                .AddTransient<UploadBlobCommand<TResponse>>(sp => sp.GetRequiredService<Mock<UploadBlobCommand<TResponse>>>().Object)

                .AddTransient<UploadBlobRequestBehavior<TRequest, TResponse>>()
                .AddTransient<UploadBlobResponseBehavior<TRequest, TResponse>>()
                .AddTransient<UploadBlobRequestProcessor<TRequest>>()
                .AddTransient<UploadBlobResponseProcessor<TRequest, TResponse>>()

                ;
        }

        #endregion

        #region Integration tests

        // for integration tests - real options and commands are used
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

        #endregion
    }
}
