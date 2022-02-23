using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Xunit;

namespace MediatR.Extensions.Azure.Storage.Tests.Commands
{
    public class DependencyInjectionTests
    {
        public static IEnumerable<object[]> TestData()
        {
            yield return new object[] { EchoRequest.Default, EchoResponse.Default };
        }

        #region Blob Tests

        [Theory(DisplayName = "DeleteBlobCommand is injected in all DeleteBlob extensions"), MemberData(nameof(TestData))]
        public void Test1<TRequest, TResponse>(TRequest req, TResponse res) where TRequest : IRequest<TResponse>
        {
            var svc = new ServiceCollection()

                .AddTransient<IOptions<BlobOptions<TRequest>>>(sp => Options.Create(new BlobOptions<TRequest>()))
                .AddTransient<IOptions<BlobOptions<TResponse>>>(sp => Options.Create(new BlobOptions<TResponse>()))
                .AddTransient<DeleteBlobCommand<TRequest>>()
                .AddTransient<DeleteBlobCommand<TResponse>>()

                .AddTransient<DeleteBlobRequestBehavior<TRequest, TResponse>>()
                .AddTransient<DeleteBlobResponseBehavior<TRequest, TResponse>>()
                .AddTransient<DeleteBlobRequestProcessor<TRequest>>()
                .AddTransient<DeleteBlobResponseProcessor<TRequest, TResponse>>()

                .BuildServiceProvider();

            _ = svc.GetRequiredService<DeleteBlobRequestBehavior<TRequest, TResponse>>();
            _ = svc.GetRequiredService<DeleteBlobResponseBehavior<TRequest, TResponse>>();
            _ = svc.GetRequiredService<DeleteBlobRequestProcessor<TRequest>>();
            _ = svc.GetRequiredService<DeleteBlobResponseProcessor<TRequest, TResponse>>();
        }

        [Theory(DisplayName = "UploadBlobCommand is injected in all UploadBlob extensions"), MemberData(nameof(TestData))]
        public void Test2<TRequest, TResponse>(TRequest req, TResponse res) where TRequest : IRequest<TResponse>
        {
            var svc = new ServiceCollection()

                .AddTransient<IOptions<BlobOptions<TRequest>>>(sp => Options.Create(new BlobOptions<TRequest>()))
                .AddTransient<IOptions<BlobOptions<TResponse>>>(sp => Options.Create(new BlobOptions<TResponse>()))
                .AddTransient<UploadBlobCommand<TRequest>>()
                .AddTransient<UploadBlobCommand<TResponse>>()

                .AddTransient<UploadBlobRequestBehavior<TRequest, TResponse>>()
                .AddTransient<UploadBlobResponseBehavior<TRequest, TResponse>>()
                .AddTransient<UploadBlobRequestProcessor<TRequest>>()
                .AddTransient<UploadBlobResponseProcessor<TRequest, TResponse>>()

                .BuildServiceProvider();

            _ = svc.GetRequiredService<UploadBlobRequestBehavior<TRequest, TResponse>>();
            _ = svc.GetRequiredService<UploadBlobResponseBehavior<TRequest, TResponse>>();
            _ = svc.GetRequiredService<UploadBlobRequestProcessor<TRequest>>();
            _ = svc.GetRequiredService<UploadBlobResponseProcessor<TRequest, TResponse>>();
        }

        [Theory(DisplayName = "DownloadBlobCommand is injected in all DownloadBlob extensions"), MemberData(nameof(TestData))]
        public void Test3<TRequest, TResponse>(TRequest req, TResponse res) where TRequest : IRequest<TResponse>
        {
            var svc = new ServiceCollection()

                .AddTransient<IOptions<BlobOptions<TRequest>>>(sp => Options.Create(new BlobOptions<TRequest>()))
                .AddTransient<IOptions<BlobOptions<TResponse>>>(sp => Options.Create(new BlobOptions<TResponse>()))
                .AddTransient<DownloadBlobCommand<TRequest>>()
                .AddTransient<DownloadBlobCommand<TResponse>>()

                .AddTransient<DownloadBlobRequestBehavior<TRequest, TResponse>>()
                .AddTransient<DownloadBlobResponseBehavior<TRequest, TResponse>>()
                .AddTransient<DownloadBlobRequestProcessor<TRequest>>()
                .AddTransient<DownloadBlobResponseProcessor<TRequest, TResponse>>()

                .BuildServiceProvider();

            _ = svc.GetRequiredService<DownloadBlobRequestBehavior<TRequest, TResponse>>();
            _ = svc.GetRequiredService<DownloadBlobResponseBehavior<TRequest, TResponse>>();
            _ = svc.GetRequiredService<DownloadBlobRequestProcessor<TRequest>>();
            _ = svc.GetRequiredService<DownloadBlobResponseProcessor<TRequest, TResponse>>();
        }

        #endregion

        #region Table Tests

        [Theory(DisplayName = "DeleteEntityCommand is injected in all DeleteEntity extensions"), MemberData(nameof(TestData))]
        public void Test4<TRequest, TResponse>(TRequest req, TResponse res) where TRequest : IRequest<TResponse>
        {
            var svc = new ServiceCollection()

                .AddTransient<IOptions<TableOptions<TRequest>>>(sp => Options.Create(new TableOptions<TRequest>()))
                .AddTransient<IOptions<TableOptions<TResponse>>>(sp => Options.Create(new TableOptions<TResponse>()))
                .AddTransient<DeleteEntityCommand<TRequest>>()
                .AddTransient<DeleteEntityCommand<TResponse>>()

                .AddTransient<DeleteEntityRequestBehavior<TRequest, TResponse>>()
                .AddTransient<DeleteEntityResponseBehavior<TRequest, TResponse>>()
                .AddTransient<DeleteEntityRequestProcessor<TRequest>>()
                .AddTransient<DeleteEntityResponseProcessor<TRequest, TResponse>>()

                .BuildServiceProvider();

            _ = svc.GetRequiredService<DeleteEntityRequestBehavior<TRequest, TResponse>>();
            _ = svc.GetRequiredService<DeleteEntityResponseBehavior<TRequest, TResponse>>();
            _ = svc.GetRequiredService<DeleteEntityRequestProcessor<TRequest>>();
            _ = svc.GetRequiredService<DeleteEntityResponseProcessor<TRequest, TResponse>>();
        }

        [Theory(DisplayName = "InsertEntityCommand is injected in all InsertEntity extensions"), MemberData(nameof(TestData))]
        public void Test5<TRequest, TResponse>(TRequest req, TResponse res) where TRequest : IRequest<TResponse>
        {
            var svc = new ServiceCollection()

                .AddTransient<IOptions<TableOptions<TRequest>>>(sp => Options.Create(new TableOptions<TRequest>()))
                .AddTransient<IOptions<TableOptions<TResponse>>>(sp => Options.Create(new TableOptions<TResponse>()))
                .AddTransient<InsertEntityCommand<TRequest>>()
                .AddTransient<InsertEntityCommand<TResponse>>()

                .AddTransient<InsertEntityRequestBehavior<TRequest, TResponse>>()
                .AddTransient<InsertEntityResponseBehavior<TRequest, TResponse>>()
                .AddTransient<InsertEntityRequestProcessor<TRequest>>()
                .AddTransient<InsertEntityResponseProcessor<TRequest, TResponse>>()

                .BuildServiceProvider();

            _ = svc.GetRequiredService<InsertEntityRequestBehavior<TRequest, TResponse>>();
            _ = svc.GetRequiredService<InsertEntityResponseBehavior<TRequest, TResponse>>();
            _ = svc.GetRequiredService<InsertEntityRequestProcessor<TRequest>>();
            _ = svc.GetRequiredService<InsertEntityResponseProcessor<TRequest, TResponse>>();
        }

        [Theory(DisplayName = "RetrieveEntityCommand is injected in all RetrieveEntity extensions"), MemberData(nameof(TestData))]
        public void Test6<TRequest, TResponse>(TRequest req, TResponse res) where TRequest : IRequest<TResponse>
        {
            var svc = new ServiceCollection()

                .AddTransient<IOptions<TableOptions<TRequest>>>(sp => Options.Create(new TableOptions<TRequest>()))
                .AddTransient<IOptions<TableOptions<TResponse>>>(sp => Options.Create(new TableOptions<TResponse>()))
                .AddTransient<RetrieveEntityCommand<TRequest>>()
                .AddTransient<RetrieveEntityCommand<TResponse>>()

                .AddTransient<RetrieveEntityRequestBehavior<TRequest, TResponse>>()
                .AddTransient<RetrieveEntityResponseBehavior<TRequest, TResponse>>()
                .AddTransient<RetrieveEntityRequestProcessor<TRequest>>()
                .AddTransient<RetrieveEntityResponseProcessor<TRequest, TResponse>>()

                .BuildServiceProvider();

            _ = svc.GetRequiredService<RetrieveEntityRequestBehavior<TRequest, TResponse>>();
            _ = svc.GetRequiredService<RetrieveEntityResponseBehavior<TRequest, TResponse>>();
            _ = svc.GetRequiredService<RetrieveEntityRequestProcessor<TRequest>>();
            _ = svc.GetRequiredService<RetrieveEntityResponseProcessor<TRequest, TResponse>>();
        }

        #endregion

        #region Queue Tests

        [Theory(DisplayName = "DeleteMessageCommand is injected in all DeleteMessage extensions"), MemberData(nameof(TestData))]
        public void Test7<TRequest, TResponse>(TRequest req, TResponse res) where TRequest : IRequest<TResponse>
        {
            var svc = new ServiceCollection()

                .AddTransient<IOptions<QueueOptions<TRequest>>>(sp => Options.Create(new QueueOptions<TRequest>()))
                .AddTransient<IOptions<QueueOptions<TResponse>>>(sp => Options.Create(new QueueOptions<TResponse>()))
                .AddTransient<DeleteMessageCommand<TRequest>>()
                .AddTransient<DeleteMessageCommand<TResponse>>()

                .AddTransient<DeleteMessageRequestBehavior<TRequest, TResponse>>()
                .AddTransient<DeleteMessageResponseBehavior<TRequest, TResponse>>()
                .AddTransient<DeleteMessageRequestProcessor<TRequest>>()
                .AddTransient<DeleteMessageResponseProcessor<TRequest, TResponse>>()

                .BuildServiceProvider();

            _ = svc.GetRequiredService<DeleteMessageRequestBehavior<TRequest, TResponse>>();
            _ = svc.GetRequiredService<DeleteMessageResponseBehavior<TRequest, TResponse>>();
            _ = svc.GetRequiredService<DeleteMessageRequestProcessor<TRequest>>();
            _ = svc.GetRequiredService<DeleteMessageResponseProcessor<TRequest, TResponse>>();
        }

        [Theory(DisplayName = "SendMessageCommand is injected in all SendMessage extensions"), MemberData(nameof(TestData))]
        public void Test8<TRequest, TResponse>(TRequest req, TResponse res) where TRequest : IRequest<TResponse>
        {
            var svc = new ServiceCollection()

                .AddTransient<IOptions<QueueOptions<TRequest>>>(sp => Options.Create(new QueueOptions<TRequest>()))
                .AddTransient<IOptions<QueueOptions<TResponse>>>(sp => Options.Create(new QueueOptions<TResponse>()))
                .AddTransient<SendMessageCommand<TRequest>>()
                .AddTransient<SendMessageCommand<TResponse>>()

                .AddTransient<SendMessageRequestBehavior<TRequest, TResponse>>()
                .AddTransient<SendMessageResponseBehavior<TRequest, TResponse>>()
                .AddTransient<SendMessageRequestProcessor<TRequest>>()
                .AddTransient<SendMessageResponseProcessor<TRequest, TResponse>>()

                .BuildServiceProvider();

            _ = svc.GetRequiredService<SendMessageRequestBehavior<TRequest, TResponse>>();
            _ = svc.GetRequiredService<SendMessageResponseBehavior<TRequest, TResponse>>();
            _ = svc.GetRequiredService<SendMessageRequestProcessor<TRequest>>();
            _ = svc.GetRequiredService<SendMessageResponseProcessor<TRequest, TResponse>>();
        }

        [Theory(DisplayName = "ReceiveMessageCommand is injected in all ReceiveMessage extensions"), MemberData(nameof(TestData))]
        public void Test9<TRequest, TResponse>(TRequest req, TResponse res) where TRequest : IRequest<TResponse>
        {
            var svc = new ServiceCollection()

                .AddTransient<IOptions<QueueOptions<TRequest>>>(sp => Options.Create(new QueueOptions<TRequest>()))
                .AddTransient<IOptions<QueueOptions<TResponse>>>(sp => Options.Create(new QueueOptions<TResponse>()))
                .AddTransient<ReceiveMessageCommand<TRequest>>()
                .AddTransient<ReceiveMessageCommand<TResponse>>()

                .AddTransient<ReceiveMessageRequestBehavior<TRequest, TResponse>>()
                .AddTransient<ReceiveMessageResponseBehavior<TRequest, TResponse>>()
                .AddTransient<ReceiveMessageRequestProcessor<TRequest>>()
                .AddTransient<ReceiveMessageResponseProcessor<TRequest, TResponse>>()

                .BuildServiceProvider();

            _ = svc.GetRequiredService<ReceiveMessageRequestBehavior<TRequest, TResponse>>();
            _ = svc.GetRequiredService<ReceiveMessageResponseBehavior<TRequest, TResponse>>();
            _ = svc.GetRequiredService<ReceiveMessageRequestProcessor<TRequest>>();
            _ = svc.GetRequiredService<ReceiveMessageResponseProcessor<TRequest, TResponse>>();
        }

        #endregion
    }
}
