using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;

namespace MediatR.Extensions.Azure.Storage.Tests
{
    public static class ServiceCollectionExtensions
    {
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

        // options don't matter, commands are mocks
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

        // options are required, commands are real instances
        public static IServiceCollection AddBlobExtensions<TRequest, TResponse>(this IServiceCollection services, BlobOptions<TRequest> req, BlobOptions<TResponse> res) where TRequest : IRequest<TResponse>
        {
            return services

                .AddTransient<IOptions<BlobOptions<TRequest>>>(sp => Options.Create(req))
                .AddTransient<UploadBlobCommand<TRequest>>()
                .AddTransient<DownloadBlobCommand<TRequest>>()
                .AddTransient<DeleteBlobCommand<TRequest>>()

                .AddTransient<IOptions<BlobOptions<TResponse>>>(sp => Options.Create(res))
                .AddTransient<UploadBlobCommand<TResponse>>()
                .AddTransient<DownloadBlobCommand<TResponse>>()
                .AddTransient<DeleteBlobCommand<TResponse>>()

                .AddTransient<UploadBlobRequestBehavior<TRequest, TResponse>>()
                .AddTransient<DownloadBlobRequestBehavior<TRequest, TResponse>>()
                .AddTransient<DeleteBlobRequestBehavior<TRequest, TResponse>>()

                .AddTransient<UploadBlobResponseBehavior<TRequest, TResponse>>()
                .AddTransient<DownloadBlobResponseBehavior<TRequest, TResponse>>()
                .AddTransient<DeleteBlobResponseBehavior<TRequest, TResponse>>()

                .AddTransient<UploadBlobRequestProcessor<TRequest>>()
                .AddTransient<DownloadBlobRequestProcessor<TRequest>>()
                .AddTransient<DeleteBlobRequestProcessor<TRequest>>()

                .AddTransient<UploadBlobResponseProcessor<TRequest, TResponse>>()
                .AddTransient<DownloadBlobResponseProcessor<TRequest, TResponse>>()
                .AddTransient<DeleteBlobResponseProcessor<TRequest, TResponse>>()
 
                ;
        }
    }
}
