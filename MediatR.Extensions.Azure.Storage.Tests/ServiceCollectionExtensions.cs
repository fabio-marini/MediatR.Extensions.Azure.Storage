using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;

namespace MediatR.Extensions.Azure.Storage.Tests
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInsertEntityExtensions(this IServiceCollection services)
        {
            return services

                .AddTransient<InsertRequestBehavior<TestCommand, Unit>>()
                .AddTransient<InsertRequestBehavior<TestQuery, TestResult>>()
                .AddTransient<InsertResponseBehavior<TestCommand, Unit>>()
                .AddTransient<InsertResponseBehavior<TestQuery, TestResult>>()
                .AddTransient<InsertRequestProcessor<TestCommand>>()
                .AddTransient<InsertRequestProcessor<TestQuery>>()
                .AddTransient<InsertResponseProcessor<TestCommand, Unit>>()
                .AddTransient<InsertResponseProcessor<TestQuery, TestResult>>()

                .AddSingleton<Mock<InsertEntityOptions<TestCommand>>>()
                .AddSingleton<Mock<InsertEntityOptions<Unit>>>()
                .AddSingleton<Mock<InsertEntityOptions<TestQuery>>>()
                .AddSingleton<Mock<InsertEntityOptions<TestResult>>>()

                .AddTransient<IOptions<InsertEntityOptions<TestCommand>>>(sp =>
                {
                    var optionsMock = sp.GetRequiredService<Mock<InsertEntityOptions<TestCommand>>>();

                    return Options.Create(optionsMock.Object);
                })
                .AddTransient<IOptions<InsertEntityOptions<Unit>>>(sp =>
                {
                    var optionsMock = sp.GetRequiredService<Mock<InsertEntityOptions<Unit>>>();

                    return Options.Create(optionsMock.Object);
                })
                .AddTransient<IOptions<InsertEntityOptions<TestQuery>>>(sp =>
                {
                    var optionsMock = sp.GetRequiredService<Mock<InsertEntityOptions<TestQuery>>>();

                    return Options.Create(optionsMock.Object);
                })
                .AddTransient<IOptions<InsertEntityOptions<TestResult>>>(sp =>
                {
                    var optionsMock = sp.GetRequiredService<Mock<InsertEntityOptions<TestResult>>>();

                    return Options.Create(optionsMock.Object);
                });
        }

        public static IServiceCollection AddUploadBlobExtensions(this IServiceCollection services)
        {
            return services

                .AddTransient<UploadRequestBehavior<TestCommand, Unit>>()
                .AddTransient<UploadRequestBehavior<TestQuery, TestResult>>()
                .AddTransient<UploadResponseBehavior<TestCommand, Unit>>()
                .AddTransient<UploadResponseBehavior<TestQuery, TestResult>>()
                .AddTransient<UploadRequestProcessor<TestCommand>>()
                .AddTransient<UploadRequestProcessor<TestQuery>>()
                .AddTransient<UploadResponseProcessor<TestCommand, Unit>>()
                .AddTransient<UploadResponseProcessor<TestQuery, TestResult>>()

                .AddSingleton<Mock<UploadBlobOptions<TestCommand>>>()
                .AddSingleton<Mock<UploadBlobOptions<Unit>>>()
                .AddSingleton<Mock<UploadBlobOptions<TestQuery>>>()
                .AddSingleton<Mock<UploadBlobOptions<TestResult>>>()

                .AddTransient<IOptions<UploadBlobOptions<TestCommand>>>(sp =>
                {
                    var optionsMock = sp.GetRequiredService<Mock<UploadBlobOptions<TestCommand>>>();

                    return Options.Create(optionsMock.Object);
                })
                .AddTransient<IOptions<UploadBlobOptions<Unit>>>(sp =>
                {
                    var optionsMock = sp.GetRequiredService<Mock<UploadBlobOptions<Unit>>>();

                    return Options.Create(optionsMock.Object);
                })
                .AddTransient<IOptions<UploadBlobOptions<TestQuery>>>(sp =>
                {
                    var optionsMock = sp.GetRequiredService<Mock<UploadBlobOptions<TestQuery>>>();

                    return Options.Create(optionsMock.Object);
                })
                .AddTransient<IOptions<UploadBlobOptions<TestResult>>>(sp =>
                {
                    var optionsMock = sp.GetRequiredService<Mock<UploadBlobOptions<TestResult>>>();

                    return Options.Create(optionsMock.Object);
                });
        }

        public static IServiceCollection AddQueueMessageExtensions(this IServiceCollection services)
        {
            return services

                .AddTransient<QueueRequestBehavior<TestCommand, Unit>>()
                .AddTransient<QueueRequestBehavior<TestQuery, TestResult>>()
                .AddTransient<QueueResponseBehavior<TestCommand, Unit>>()
                .AddTransient<QueueResponseBehavior<TestQuery, TestResult>>()
                .AddTransient<QueueRequestProcessor<TestCommand>>()
                .AddTransient<QueueRequestProcessor<TestQuery>>()
                .AddTransient<QueueResponseProcessor<TestCommand, Unit>>()
                .AddTransient<QueueResponseProcessor<TestQuery, TestResult>>()

                .AddSingleton<Mock<QueueMessageOptions<TestCommand>>>()
                .AddSingleton<Mock<QueueMessageOptions<Unit>>>()
                .AddSingleton<Mock<QueueMessageOptions<TestQuery>>>()
                .AddSingleton<Mock<QueueMessageOptions<TestResult>>>()

                .AddTransient<IOptions<QueueMessageOptions<TestCommand>>>(sp =>
                {
                    var optionsMock = sp.GetRequiredService<Mock<QueueMessageOptions<TestCommand>>>();

                    return Options.Create(optionsMock.Object);
                })
                .AddTransient<IOptions<QueueMessageOptions<Unit>>>(sp =>
                {
                    var optionsMock = sp.GetRequiredService<Mock<QueueMessageOptions<Unit>>>();

                    return Options.Create(optionsMock.Object);
                })
                .AddTransient<IOptions<QueueMessageOptions<TestQuery>>>(sp =>
                {
                    var optionsMock = sp.GetRequiredService<Mock<QueueMessageOptions<TestQuery>>>();

                    return Options.Create(optionsMock.Object);
                })
                .AddTransient<IOptions<QueueMessageOptions<TestResult>>>(sp =>
                {
                    var optionsMock = sp.GetRequiredService<Mock<QueueMessageOptions<TestResult>>>();

                    return Options.Create(optionsMock.Object);
                });
        }
    }
}
