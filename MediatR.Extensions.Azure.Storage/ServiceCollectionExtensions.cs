using Microsoft.Extensions.DependencyInjection;

namespace MediatR.Extensions.Azure.Storage
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTableExtensions<TRequest>(this IServiceCollection services) where TRequest : IRequest<Unit>
        {
            return services.AddTableExtensions<TRequest, Unit>();
        }

        public static IServiceCollection AddTableExtensions<TRequest, TResponse>(this IServiceCollection services) where TRequest : IRequest<TResponse>
        {
            return services

                .AddTransient<InsertEntityCommand<TRequest>>()
                .AddTransient<RetrieveEntityCommand<TRequest>>()
                .AddTransient<DeleteEntityCommand<TRequest>>()

                .AddTransient<InsertEntityCommand<TResponse>>()
                .AddTransient<RetrieveEntityCommand<TResponse>>()
                .AddTransient<DeleteEntityCommand<TResponse>>()

                .AddTransient<InsertRequestBehavior<TRequest, TResponse>>()
                .AddTransient<InsertResponseBehavior<TRequest, TResponse>>()
                .AddTransient<InsertRequestProcessor<TRequest>>()
                .AddTransient<InsertResponseProcessor<TRequest, TResponse>>()

                ;
        }

        public static IServiceCollection AddBlobExtensions<TRequest>(this IServiceCollection services) where TRequest : IRequest<Unit>
        {
            return services.AddBlobExtensions<TRequest, Unit>();
        }

        public static IServiceCollection AddBlobExtensions<TRequest, TResponse>(this IServiceCollection services) where TRequest : IRequest<TResponse>
        {
            return services

                .AddTransient<UploadBlobCommand<TRequest>>()
                .AddTransient<DownloadBlobCommand<TRequest>>()
                .AddTransient<DeleteBlobCommand<TRequest>>()

                .AddTransient<UploadBlobCommand<TResponse>>()
                .AddTransient<DownloadBlobCommand<TResponse>>()
                .AddTransient<DeleteBlobCommand<TResponse>>()

                .AddTransient<UploadRequestBehavior<TRequest, TResponse>>()
                .AddTransient<UploadResponseBehavior<TRequest, TResponse>>()
                .AddTransient<UploadRequestProcessor<TRequest>>()
                .AddTransient<UploadResponseProcessor<TRequest, TResponse>>()

                ;
        }

        public static IServiceCollection AddQueueExtensions<TRequest>(this IServiceCollection services) where TRequest : IRequest<Unit>
        {
            return services.AddQueueExtensions<TRequest, Unit>();
        }

        public static IServiceCollection AddQueueExtensions<TRequest, TResponse>(this IServiceCollection services) where TRequest : IRequest<TResponse>
        {
            return services

                .AddTransient<SendMessageCommand<TRequest>>()
                .AddTransient<ReceiveMessageCommand<TRequest>>()
                .AddTransient<DeleteMessageCommand<TRequest>>()

                .AddTransient<SendMessageCommand<TResponse>>()
                .AddTransient<ReceiveMessageCommand<TRequest>>()
                .AddTransient<DeleteMessageCommand<TRequest>>()

                .AddTransient<SendRequestBehavior<TRequest, TResponse>>()
                .AddTransient<SendResponseBehavior<TRequest, TResponse>>()
                .AddTransient<SendRequestProcessor<TRequest>>()
                .AddTransient<SendResponseProcessor<TRequest, TResponse>>()

                ;
        }
    }
}
