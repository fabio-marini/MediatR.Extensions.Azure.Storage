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
using Xunit;

namespace MediatR.Extensions.Azure.Storage.Tests
{
    public class SendMessageBehaviorTests
    {
        private readonly IServiceProvider svc;
        private readonly Mock<SendMessageOptions<TestCommand>> opt;
        private readonly IMediator med;
        private readonly Mock<QueueClient> que;
        private readonly Mock<ILogger> log;

        public SendMessageBehaviorTests()
        {
            opt = new Mock<SendMessageOptions<TestCommand>>();
            que = new Mock<QueueClient>("UseDevelopmentStorage=true", "queue1");
            log = new Mock<ILogger>();

            svc = new ServiceCollection()

                .AddMediatR(this.GetType())

                .AddTransient<IPipelineBehavior<TestCommand, Unit>, SendMessageBehavior<TestCommand>>()

                .AddTransient<IOptions<SendMessageOptions<TestCommand>>>(sp => Options.Create(opt.Object))

                .AddTransient<PipelineContext>()

                .AddTransient<ILogger>(sp => log.Object)

                .BuildServiceProvider();

            med = svc.GetRequiredService<IMediator>();
        }

        [Fact(DisplayName = "Behavior is disabled")]
        public async Task Test1()
        {
            var cmd = new TestCommand { Message = "Hello! :)" };

            _ = await med.Send(cmd);

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.QueueClient, Times.Never);
            opt.VerifyGet(m => m.QueueMessage, Times.Never);
        }

        [Fact(DisplayName = "QueueClient is not specified")]
        public async Task Test2()
        {
            opt.SetupProperty(m => m.IsEnabled, true);

            var cmd = new TestCommand { Message = "Hello! :)" };

            _ = await med.Send(cmd);

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.QueueClient, Times.Once);
            opt.VerifyGet(m => m.QueueMessage, Times.Never);
        }

        [Fact(DisplayName = "Behavior uses default QueueMessage")]
        public async Task Test3()
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.QueueClient, que.Object);
            opt.SetupProperty(m => m.QueueMessage, null);

            var cmd = new TestCommand { Message = "Hello! :)" };

            _ = await med.Send(cmd);

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.QueueClient, Times.Exactly(2));
            opt.VerifyGet(m => m.QueueMessage, Times.Exactly(2));

            opt.VerifySet(m => m.QueueMessage = It.IsAny<Func<TestCommand, PipelineContext, BinaryData>>(), Times.Once);

            opt.Verify(m => m.QueueClient.SendMessageAsync(It.IsAny<BinaryData>(), opt.Object.Visibility, opt.Object.TimeToLive, CancellationToken.None), Times.Once);
        }

        [Fact(DisplayName = "Behavior uses specified QueueMessage")]
        public async Task Test4()
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.QueueClient, que.Object);
            opt.SetupProperty(m => m.QueueMessage, (cmd, ctx) => BinaryData.FromString("Hello world"));

            var cmd = new TestCommand { Message = "Hello! :)" };

            _ = await med.Send(cmd);

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.QueueClient, Times.Exactly(2));
            opt.VerifyGet(m => m.QueueMessage, Times.Exactly(2));

            opt.VerifySet(m => m.QueueMessage = It.IsAny<Func<TestCommand, PipelineContext, BinaryData>>(), Times.Never);

            opt.Verify(m => m.QueueClient.SendMessageAsync(It.IsAny<BinaryData>(), opt.Object.Visibility, opt.Object.TimeToLive, CancellationToken.None), Times.Once);
        }

        [Fact(DisplayName = "Exceptions are logged")]
        public async Task Test5()
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.QueueClient, que.Object);
            opt.SetupProperty(m => m.QueueMessage, (cmd, ctx) => BinaryData.FromString("Hello world"));

            que.Setup(m => m.SendMessageAsync(It.IsAny<BinaryData>(), opt.Object.Visibility, opt.Object.TimeToLive, CancellationToken.None)).Throws(new Exception("Failed! :("));

            var cmd = new TestCommand { Message = "Hello! :)" };

            _ = await med.Send(cmd);

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.QueueClient, Times.Exactly(2));
            opt.VerifyGet(m => m.QueueMessage, Times.Exactly(2));

            opt.VerifySet(m => m.QueueMessage = It.IsAny<Func<TestCommand, PipelineContext, BinaryData>>(), Times.Never);

            opt.Verify(m => m.QueueClient.SendMessageAsync(It.IsAny<BinaryData>(), opt.Object.Visibility, opt.Object.TimeToLive, CancellationToken.None), Times.Once);

            var logInvocation = log.Invocations.Where(i => i.Method.Name == "Log").Single();

            logInvocation.Arguments.OfType<LogLevel>().Single().Should().Be(LogLevel.Error);
            logInvocation.Arguments.OfType<Exception>().Single().Message.Should().Be("Failed! :(");
        }
    }
}
