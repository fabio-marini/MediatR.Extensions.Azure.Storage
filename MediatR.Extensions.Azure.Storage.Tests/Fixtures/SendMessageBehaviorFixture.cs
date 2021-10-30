using Azure.Storage.Queues;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Extensions.Azure.Storage.Tests
{
    public class SendMessageBehaviorFixture<TRequest> : SendMessageBehaviorFixture<TRequest, Unit> where TRequest : IRequest<Unit>
    {
    }

    public class SendMessageBehaviorFixture<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IServiceProvider svc;
        private readonly Mock<SendMessageOptions<TRequest, TResponse>> opt;
        private readonly IMediator med;
        private readonly Mock<QueueClient> que;
        private readonly Mock<ILogger> log;

        public SendMessageBehaviorFixture()
        {
            opt = new Mock<SendMessageOptions<TRequest, TResponse>>();
            que = new Mock<QueueClient>("UseDevelopmentStorage=true", "queue1");
            log = new Mock<ILogger>();

            svc = new ServiceCollection()

                .AddMediatR(this.GetType())

                .AddTransient<IPipelineBehavior<TRequest, TResponse>, SendMessageBehavior<TRequest, TResponse>>()

                .AddTransient<IOptions<SendMessageOptions<TRequest, TResponse>>>(sp => Options.Create(opt.Object))

                .AddTransient<PipelineContext>()

                .AddTransient<ILogger>(sp => log.Object)

                .BuildServiceProvider();

            med = svc.GetRequiredService<IMediator>();
        }

        public async Task<TResponse> Test1(TRequest req)
        {
            var res = await med.Send(req);

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.QueueClient, Times.Never);
            opt.VerifyGet(m => m.QueueMessage, Times.Never);

            return res;
        }

        public async Task<TResponse> Test2(TRequest req)
        {
            opt.SetupProperty(m => m.IsEnabled, true);

            var res = await med.Send(req);

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.QueueClient, Times.Once);
            opt.VerifyGet(m => m.QueueMessage, Times.Never);

            return res;
        }

        public async Task<TResponse> Test3(TRequest req)
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.QueueClient, que.Object);
            opt.SetupProperty(m => m.QueueMessage, null);

            var res = await med.Send(req);

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.QueueClient, Times.Exactly(2));
            opt.VerifyGet(m => m.QueueMessage, Times.Exactly(2));

            opt.VerifySet(m => m.QueueMessage = It.IsAny<Func<TRequest, PipelineContext, BinaryData>>(), Times.Once);

            opt.Verify(m => m.QueueClient.SendMessageAsync(It.IsAny<BinaryData>(), opt.Object.Visibility, opt.Object.TimeToLive, CancellationToken.None), Times.Once);

            return res;
        }

        public async Task<TResponse> Test4(TRequest req)
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.QueueClient, que.Object);
            opt.SetupProperty(m => m.QueueMessage, (cmd, ctx) => BinaryData.FromString("Hello world"));

            var res = await med.Send(req);

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.QueueClient, Times.Exactly(2));
            opt.VerifyGet(m => m.QueueMessage, Times.Exactly(2));

            opt.VerifySet(m => m.QueueMessage = It.IsAny<Func<TRequest, PipelineContext, BinaryData>>(), Times.Never);

            opt.Verify(m => m.QueueClient.SendMessageAsync(It.IsAny<BinaryData>(), opt.Object.Visibility, opt.Object.TimeToLive, CancellationToken.None), Times.Once);

            return res;
        }

        public async Task<TResponse> Test5(TRequest req)
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.QueueClient, que.Object);
            opt.SetupProperty(m => m.QueueMessage, (cmd, ctx) => BinaryData.FromString("Hello world"));

            que.Setup(m => m.SendMessageAsync(It.IsAny<BinaryData>(), opt.Object.Visibility, opt.Object.TimeToLive, CancellationToken.None)).Throws(new Exception("Failed! :("));

            var res = await med.Send(req);

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.QueueClient, Times.Exactly(2));
            opt.VerifyGet(m => m.QueueMessage, Times.Exactly(2));

            opt.VerifySet(m => m.QueueMessage = It.IsAny<Func<TRequest, PipelineContext, BinaryData>>(), Times.Never);

            opt.Verify(m => m.QueueClient.SendMessageAsync(It.IsAny<BinaryData>(), opt.Object.Visibility, opt.Object.TimeToLive, CancellationToken.None), Times.Once);

            var logInvocation = log.Invocations.Where(i => i.Method.Name == "Log").Single();

            logInvocation.Arguments.OfType<LogLevel>().Single().Should().Be(LogLevel.Error);
            logInvocation.Arguments.OfType<Exception>().Single().Message.Should().Be("Failed! :(");

            return res;
        }
    }
}
