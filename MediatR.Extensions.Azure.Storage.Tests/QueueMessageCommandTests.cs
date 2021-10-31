﻿using Azure.Storage.Queues;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MediatR.Extensions.Azure.Storage.Tests
{
    public class QueueMessageCommandTests
    {
        private readonly IServiceProvider svc;
        private readonly Mock<QueueMessageOptions<TestMessage>> opt;
        private readonly Mock<QueueClient> que;

        private readonly QueueMessageCommand<TestMessage> cmd;

        public QueueMessageCommandTests()
        {
            opt = new Mock<QueueMessageOptions<TestMessage>>();
            que = new Mock<QueueClient>("UseDevelopmentStorage=true", "queue1");

            svc = new ServiceCollection()

                .AddTransient<QueueMessageCommand<TestMessage>>()
                .AddTransient<IOptions<QueueMessageOptions<TestMessage>>>(sp => Options.Create(opt.Object))

                .BuildServiceProvider();

            cmd = svc.GetRequiredService<QueueMessageCommand<TestMessage>>();
        }

        [Fact(DisplayName = "Command is disabled")]
        public async Task Test1()
        {
            await cmd.ExecuteAsync(TestMessage.Default, CancellationToken.None);

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.QueueClient, Times.Never);
            opt.VerifyGet(m => m.QueueMessage, Times.Never);
        }

        [Fact(DisplayName = "QueueClient is not specified")]
        public async Task Test2()
        {
            opt.SetupProperty(m => m.IsEnabled, true);

            Func<Task> act = async () => await cmd.ExecuteAsync(TestMessage.Default, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentNullException>();

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.QueueClient, Times.Once);
            opt.VerifyGet(m => m.QueueMessage, Times.Never);
        }

        [Fact(DisplayName = "Command uses default QueueMessage")]
        public async Task Test3()
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.QueueClient, que.Object);
            opt.SetupProperty(m => m.QueueMessage, null);

            await cmd.ExecuteAsync(TestMessage.Default, CancellationToken.None);

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.QueueClient, Times.Exactly(2));
            opt.VerifyGet(m => m.QueueMessage, Times.Exactly(2));

            opt.VerifySet(m => m.QueueMessage = It.IsAny<Func<TestMessage, PipelineContext, BinaryData>>(), Times.Once);

            opt.Verify(m => m.QueueClient.SendMessageAsync(It.IsAny<BinaryData>(), opt.Object.Visibility, opt.Object.TimeToLive, CancellationToken.None), Times.Once);
        }

        [Fact(DisplayName = "Command uses specified QueueMessage")]
        public async Task Test4()
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.QueueClient, que.Object);
            opt.SetupProperty(m => m.QueueMessage, (cmd, ctx) => BinaryData.FromString("Hello world"));

            await cmd.ExecuteAsync(TestMessage.Default, CancellationToken.None);

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.QueueClient, Times.Exactly(2));
            opt.VerifyGet(m => m.QueueMessage, Times.Exactly(2));

            opt.VerifySet(m => m.QueueMessage = It.IsAny<Func<TestMessage, PipelineContext, BinaryData>>(), Times.Never);

            opt.Verify(m => m.QueueClient.SendMessageAsync(It.IsAny<BinaryData>(), opt.Object.Visibility, opt.Object.TimeToLive, CancellationToken.None), Times.Once);
        }
    }
}
