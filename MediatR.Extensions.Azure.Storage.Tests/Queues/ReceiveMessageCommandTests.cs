using Azure;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using FluentAssertions;
using MediatR.Extensions.Azure.Storage.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MediatR.Extensions.Azure.Storage.Tests.Commands.Queues
{
    public class ReceiveMessageCommandTests
    {
        private readonly IServiceProvider svc;
        private readonly Mock<QueueOptions<TestMessage>> opt;
        private readonly Mock<QueueClient> que;

        private readonly ReceiveMessageCommand<TestMessage> cmd;

        public ReceiveMessageCommandTests()
        {
            opt = new Mock<QueueOptions<TestMessage>>();
            que = new Mock<QueueClient>("UseDevelopmentStorage=true", "queue1");

            svc = new ServiceCollection()

                .AddTransient<ReceiveMessageCommand<TestMessage>>()
                .AddTransient<IOptions<QueueOptions<TestMessage>>>(sp => Options.Create(opt.Object))

                .BuildServiceProvider();

            cmd = svc.GetRequiredService<ReceiveMessageCommand<TestMessage>>();
        }

        [Fact(DisplayName = "Command is disabled")]
        public async Task Test1()
        {
            await cmd.ExecuteAsync(TestMessage.Default, CancellationToken.None);

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.QueueClient, Times.Never);
            opt.VerifyGet(m => m.QueueMessage, Times.Never);
        }

        [Fact(DisplayName = "Command is cancelled")]
        public async Task Test2()
        {
            var src = new CancellationTokenSource(0);

            Func<Task> act = async () => await cmd.ExecuteAsync(TestMessage.Default, src.Token);

            await act.Should().ThrowAsync<OperationCanceledException>();

            opt.VerifyGet(m => m.IsEnabled, Times.Never);
        }

        [Fact(DisplayName = "QueueClient is not specified")]
        public async Task Test3()
        {
            opt.SetupProperty(m => m.IsEnabled, true);

            Func<Task> act = async () => await cmd.ExecuteAsync(TestMessage.Default, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentNullException>();

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.QueueClient, Times.Once);
            opt.VerifyGet(m => m.QueueMessage, Times.Never);
        }

        [Fact(DisplayName = "Command throws CommandException")]
        public async Task Test4()
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.QueueClient, que.Object);

            que.Setup(m => m.ReceiveMessageAsync(opt.Object.Visibility, CancellationToken.None))
                .ThrowsAsync(new ArgumentNullException());

            Func<Task> act = async () => await cmd.ExecuteAsync(TestMessage.Default, CancellationToken.None);

            await act.Should().ThrowAsync<CommandException>();

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.QueueClient, Times.Exactly(2));

            opt.Verify(m => m.QueueClient.ReceiveMessageAsync(opt.Object.Visibility, CancellationToken.None), Times.Once);
        }

        [Fact(DisplayName = "Command completes successfully")]
        public async Task Test5()
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.QueueClient, que.Object);

            var res = new Mock<Response>();
            res.SetupGet(r => r.Status).Returns(200);

            que.Setup(m => m.ReceiveMessageAsync(opt.Object.Visibility, CancellationToken.None))
                .ReturnsAsync(Response.FromValue<QueueMessage>(default, res.Object));

            await cmd.ExecuteAsync(TestMessage.Default, CancellationToken.None);

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.QueueClient, Times.Exactly(2));
            opt.VerifyGet(m => m.Received, Times.Once);

            opt.Verify(m => m.QueueClient.ReceiveMessageAsync(opt.Object.Visibility, CancellationToken.None), Times.Once);
        }
    }
}
