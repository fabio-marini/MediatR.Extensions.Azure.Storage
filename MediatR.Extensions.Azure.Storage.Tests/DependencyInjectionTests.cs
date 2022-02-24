using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace MediatR.Extensions.Azure.Storage.Tests.Commands
{
    public class DependencyInjectionTests
    {
        #region Blob Tests

        [Fact(DisplayName = "DeleteBlobCommand is injected in all DeleteBlob extensions")]
        public void Test1()
        {
            var svc = new ServiceCollection()

                .AddTransient<IOptions<BlobOptions<EchoRequest>>>(sp => Options.Create(new BlobOptions<EchoRequest>()))
                .AddTransient<IOptions<BlobOptions<EchoResponse>>>(sp => Options.Create(new BlobOptions<EchoResponse>()))
                .AddTransient<DeleteBlobCommand<EchoRequest>>()
                .AddTransient<DeleteBlobCommand<EchoResponse>>()

                .AddTransient<DeleteBlobRequestBehavior<EchoRequest, EchoResponse>>()
                .AddTransient<DeleteBlobResponseBehavior<EchoRequest, EchoResponse>>()
                .AddTransient<DeleteBlobRequestProcessor<EchoRequest>>()
                .AddTransient<DeleteBlobResponseProcessor<EchoRequest, EchoResponse>>()

                .BuildServiceProvider();

            _ = svc.GetRequiredService<DeleteBlobRequestBehavior<EchoRequest, EchoResponse>>();
            _ = svc.GetRequiredService<DeleteBlobResponseBehavior<EchoRequest, EchoResponse>>();
            _ = svc.GetRequiredService<DeleteBlobRequestProcessor<EchoRequest>>();
            _ = svc.GetRequiredService<DeleteBlobResponseProcessor<EchoRequest, EchoResponse>>();
        }

        [Fact(DisplayName = "UploadBlobCommand is injected in all UploadBlob extensions")]
        public void Test2()
        {
            var svc = new ServiceCollection()

                .AddTransient<IOptions<BlobOptions<EchoRequest>>>(sp => Options.Create(new BlobOptions<EchoRequest>()))
                .AddTransient<IOptions<BlobOptions<EchoResponse>>>(sp => Options.Create(new BlobOptions<EchoResponse>()))
                .AddTransient<UploadBlobCommand<EchoRequest>>()
                .AddTransient<UploadBlobCommand<EchoResponse>>()

                .AddTransient<UploadBlobRequestBehavior<EchoRequest, EchoResponse>>()
                .AddTransient<UploadBlobResponseBehavior<EchoRequest, EchoResponse>>()
                .AddTransient<UploadBlobRequestProcessor<EchoRequest>>()
                .AddTransient<UploadBlobResponseProcessor<EchoRequest, EchoResponse>>()

                .BuildServiceProvider();

            _ = svc.GetRequiredService<UploadBlobRequestBehavior<EchoRequest, EchoResponse>>();
            _ = svc.GetRequiredService<UploadBlobResponseBehavior<EchoRequest, EchoResponse>>();
            _ = svc.GetRequiredService<UploadBlobRequestProcessor<EchoRequest>>();
            _ = svc.GetRequiredService<UploadBlobResponseProcessor<EchoRequest, EchoResponse>>();
        }

        [Fact(DisplayName = "DownloadBlobCommand is injected in all DownloadBlob extensions")]
        public void Test3()
        {
            var svc = new ServiceCollection()

                .AddTransient<IOptions<BlobOptions<EchoRequest>>>(sp => Options.Create(new BlobOptions<EchoRequest>()))
                .AddTransient<IOptions<BlobOptions<EchoResponse>>>(sp => Options.Create(new BlobOptions<EchoResponse>()))
                .AddTransient<DownloadBlobCommand<EchoRequest>>()
                .AddTransient<DownloadBlobCommand<EchoResponse>>()

                .AddTransient<DownloadBlobRequestBehavior<EchoRequest, EchoResponse>>()
                .AddTransient<DownloadBlobResponseBehavior<EchoRequest, EchoResponse>>()
                .AddTransient<DownloadBlobRequestProcessor<EchoRequest>>()
                .AddTransient<DownloadBlobResponseProcessor<EchoRequest, EchoResponse>>()

                .BuildServiceProvider();

            _ = svc.GetRequiredService<DownloadBlobRequestBehavior<EchoRequest, EchoResponse>>();
            _ = svc.GetRequiredService<DownloadBlobResponseBehavior<EchoRequest, EchoResponse>>();
            _ = svc.GetRequiredService<DownloadBlobRequestProcessor<EchoRequest>>();
            _ = svc.GetRequiredService<DownloadBlobResponseProcessor<EchoRequest, EchoResponse>>();
        }

        #endregion

        #region Table Tests

        [Fact(DisplayName = "DeleteEntityCommand is injected in all DeleteEntity extensions")]
        public void Test4()
        {
            var svc = new ServiceCollection()

                .AddTransient<IOptions<TableOptions<EchoRequest>>>(sp => Options.Create(new TableOptions<EchoRequest>()))
                .AddTransient<IOptions<TableOptions<EchoResponse>>>(sp => Options.Create(new TableOptions<EchoResponse>()))
                .AddTransient<DeleteEntityCommand<EchoRequest>>()
                .AddTransient<DeleteEntityCommand<EchoResponse>>()

                .AddTransient<DeleteEntityRequestBehavior<EchoRequest, EchoResponse>>()
                .AddTransient<DeleteEntityResponseBehavior<EchoRequest, EchoResponse>>()
                .AddTransient<DeleteEntityRequestProcessor<EchoRequest>>()
                .AddTransient<DeleteEntityResponseProcessor<EchoRequest, EchoResponse>>()

                .BuildServiceProvider();

            _ = svc.GetRequiredService<DeleteEntityRequestBehavior<EchoRequest, EchoResponse>>();
            _ = svc.GetRequiredService<DeleteEntityResponseBehavior<EchoRequest, EchoResponse>>();
            _ = svc.GetRequiredService<DeleteEntityRequestProcessor<EchoRequest>>();
            _ = svc.GetRequiredService<DeleteEntityResponseProcessor<EchoRequest, EchoResponse>>();
        }

        [Fact(DisplayName = "InsertEntityCommand is injected in all InsertEntity extensions")]
        public void Test5()
        {
            var svc = new ServiceCollection()

                .AddTransient<IOptions<TableOptions<EchoRequest>>>(sp => Options.Create(new TableOptions<EchoRequest>()))
                .AddTransient<IOptions<TableOptions<EchoResponse>>>(sp => Options.Create(new TableOptions<EchoResponse>()))
                .AddTransient<InsertEntityCommand<EchoRequest>>()
                .AddTransient<InsertEntityCommand<EchoResponse>>()

                .AddTransient<InsertEntityRequestBehavior<EchoRequest, EchoResponse>>()
                .AddTransient<InsertEntityResponseBehavior<EchoRequest, EchoResponse>>()
                .AddTransient<InsertEntityRequestProcessor<EchoRequest>>()
                .AddTransient<InsertEntityResponseProcessor<EchoRequest, EchoResponse>>()

                .BuildServiceProvider();

            _ = svc.GetRequiredService<InsertEntityRequestBehavior<EchoRequest, EchoResponse>>();
            _ = svc.GetRequiredService<InsertEntityResponseBehavior<EchoRequest, EchoResponse>>();
            _ = svc.GetRequiredService<InsertEntityRequestProcessor<EchoRequest>>();
            _ = svc.GetRequiredService<InsertEntityResponseProcessor<EchoRequest, EchoResponse>>();
        }

        [Fact(DisplayName = "RetrieveEntityCommand is injected in all RetrieveEntity extensions")]
        public void Test6()
        {
            var svc = new ServiceCollection()

                .AddTransient<IOptions<TableOptions<EchoRequest>>>(sp => Options.Create(new TableOptions<EchoRequest>()))
                .AddTransient<IOptions<TableOptions<EchoResponse>>>(sp => Options.Create(new TableOptions<EchoResponse>()))
                .AddTransient<RetrieveEntityCommand<EchoRequest>>()
                .AddTransient<RetrieveEntityCommand<EchoResponse>>()

                .AddTransient<RetrieveEntityRequestBehavior<EchoRequest, EchoResponse>>()
                .AddTransient<RetrieveEntityResponseBehavior<EchoRequest, EchoResponse>>()
                .AddTransient<RetrieveEntityRequestProcessor<EchoRequest>>()
                .AddTransient<RetrieveEntityResponseProcessor<EchoRequest, EchoResponse>>()

                .BuildServiceProvider();

            _ = svc.GetRequiredService<RetrieveEntityRequestBehavior<EchoRequest, EchoResponse>>();
            _ = svc.GetRequiredService<RetrieveEntityResponseBehavior<EchoRequest, EchoResponse>>();
            _ = svc.GetRequiredService<RetrieveEntityRequestProcessor<EchoRequest>>();
            _ = svc.GetRequiredService<RetrieveEntityResponseProcessor<EchoRequest, EchoResponse>>();
        }

        #endregion

        #region Queue Tests

        [Fact(DisplayName = "DeleteMessageCommand is injected in all DeleteMessage extensions")]
        public void Test7()
        {
            var svc = new ServiceCollection()

                .AddTransient<IOptions<QueueOptions<EchoRequest>>>(sp => Options.Create(new QueueOptions<EchoRequest>()))
                .AddTransient<IOptions<QueueOptions<EchoResponse>>>(sp => Options.Create(new QueueOptions<EchoResponse>()))
                .AddTransient<DeleteMessageCommand<EchoRequest>>()
                .AddTransient<DeleteMessageCommand<EchoResponse>>()

                .AddTransient<DeleteMessageRequestBehavior<EchoRequest, EchoResponse>>()
                .AddTransient<DeleteMessageResponseBehavior<EchoRequest, EchoResponse>>()
                .AddTransient<DeleteMessageRequestProcessor<EchoRequest>>()
                .AddTransient<DeleteMessageResponseProcessor<EchoRequest, EchoResponse>>()

                .BuildServiceProvider();

            _ = svc.GetRequiredService<DeleteMessageRequestBehavior<EchoRequest, EchoResponse>>();
            _ = svc.GetRequiredService<DeleteMessageResponseBehavior<EchoRequest, EchoResponse>>();
            _ = svc.GetRequiredService<DeleteMessageRequestProcessor<EchoRequest>>();
            _ = svc.GetRequiredService<DeleteMessageResponseProcessor<EchoRequest, EchoResponse>>();
        }

        [Fact(DisplayName = "SendMessageCommand is injected in all SendMessage extensions")]
        public void Test8()
        {
            var svc = new ServiceCollection()

                .AddTransient<IOptions<QueueOptions<EchoRequest>>>(sp => Options.Create(new QueueOptions<EchoRequest>()))
                .AddTransient<IOptions<QueueOptions<EchoResponse>>>(sp => Options.Create(new QueueOptions<EchoResponse>()))
                .AddTransient<SendMessageCommand<EchoRequest>>()
                .AddTransient<SendMessageCommand<EchoResponse>>()

                .AddTransient<SendMessageRequestBehavior<EchoRequest, EchoResponse>>()
                .AddTransient<SendMessageResponseBehavior<EchoRequest, EchoResponse>>()
                .AddTransient<SendMessageRequestProcessor<EchoRequest>>()
                .AddTransient<SendMessageResponseProcessor<EchoRequest, EchoResponse>>()

                .BuildServiceProvider();

            _ = svc.GetRequiredService<SendMessageRequestBehavior<EchoRequest, EchoResponse>>();
            _ = svc.GetRequiredService<SendMessageResponseBehavior<EchoRequest, EchoResponse>>();
            _ = svc.GetRequiredService<SendMessageRequestProcessor<EchoRequest>>();
            _ = svc.GetRequiredService<SendMessageResponseProcessor<EchoRequest, EchoResponse>>();
        }

        [Fact(DisplayName = "ReceiveMessageCommand is injected in all ReceiveMessage extensions")]
        public void Test9()
        {
            var svc = new ServiceCollection()

                .AddTransient<IOptions<QueueOptions<EchoRequest>>>(sp => Options.Create(new QueueOptions<EchoRequest>()))
                .AddTransient<IOptions<QueueOptions<EchoResponse>>>(sp => Options.Create(new QueueOptions<EchoResponse>()))
                .AddTransient<ReceiveMessageCommand<EchoRequest>>()
                .AddTransient<ReceiveMessageCommand<EchoResponse>>()

                .AddTransient<ReceiveMessageRequestBehavior<EchoRequest, EchoResponse>>()
                .AddTransient<ReceiveMessageResponseBehavior<EchoRequest, EchoResponse>>()
                .AddTransient<ReceiveMessageRequestProcessor<EchoRequest>>()
                .AddTransient<ReceiveMessageResponseProcessor<EchoRequest, EchoResponse>>()

                .BuildServiceProvider();

            _ = svc.GetRequiredService<ReceiveMessageRequestBehavior<EchoRequest, EchoResponse>>();
            _ = svc.GetRequiredService<ReceiveMessageResponseBehavior<EchoRequest, EchoResponse>>();
            _ = svc.GetRequiredService<ReceiveMessageRequestProcessor<EchoRequest>>();
            _ = svc.GetRequiredService<ReceiveMessageResponseProcessor<EchoRequest, EchoResponse>>();
        }

        #endregion
    }
}
