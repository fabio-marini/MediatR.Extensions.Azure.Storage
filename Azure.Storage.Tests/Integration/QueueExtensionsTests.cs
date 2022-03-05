using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace MediatR.Extensions.Azure.Storage.Tests.Integration
{
    [Trait("TestCategory", "Integration")]
    [TestCaseOrderer("MediatR.Extensions.Tests.TestMethodNameOrderer", "MediatR.Extensions.Azure.Storage.Tests")]
    public class QueueExtensionsTests : IClassFixture<QueueFixture>
    {
        private readonly QueueFixture queueFixture;
        private readonly ITestOutputHelper log;

        public QueueExtensionsTests(QueueFixture queueFixture, ITestOutputHelper log)
        {
            this.queueFixture = queueFixture;
            this.log = log;
        }

        [Fact(DisplayName = "01. Queue is empty")]
        public void Test1() => queueFixture.GivenQueueIsEmpty();

        [Fact(DisplayName = "02. Messages are sent")]
        public async Task Test2()
        {
            var serviceProvider = new ServiceCollection()
                
                .AddMediatR(this.GetType())
                .AddTransient<QueueClient>(sp => queueFixture.QueueClient)
                .AddTransient<ITestOutputHelper>(sp => log)
                .AddQueueOptions()
                .AddSendMessageExtensions()
                .AddSingleton<Queue<QueueMessage>>(sp => queueFixture.Messages)

                .BuildServiceProvider();

            var med = serviceProvider.GetRequiredService<IMediator>();

            var res = await med.Send(EchoRequest.Default);

            res.Message.Should().Be(EchoRequest.Default.Message);
        }

        [Fact(DisplayName = "03. Queue has messages")]
        public void Test3() => queueFixture.ThenQueueHasMessages(4);

        [Fact(DisplayName = "04. Messages are received")]
        public async Task Test4()
        {
            var serviceProvider = new ServiceCollection()

                .AddMediatR(this.GetType())
                .AddTransient<QueueClient>(sp => queueFixture.QueueClient)
                .AddTransient<ITestOutputHelper>(sp => log)
                .AddQueueOptions()
                .AddReceiveMessageExtensions()
                .AddSingleton<Queue<QueueMessage>>(sp => queueFixture.Messages)

                .BuildServiceProvider();

            var med = serviceProvider.GetRequiredService<IMediator>();

            var res = await med.Send(EchoRequest.Default);

            res.Message.Should().Be(EchoRequest.Default.Message);

            var ctx = serviceProvider.GetRequiredService<Queue<QueueMessage>>();

            ctx.Should().HaveCount(4);
        }

        [Fact(DisplayName = "05. Messages are deleted")]
        public async Task Test5()
        {
            var serviceProvider = new ServiceCollection()

                .AddMediatR(this.GetType())
                .AddTransient<QueueClient>(sp => queueFixture.QueueClient)
                .AddTransient<ITestOutputHelper>(sp => log)
                .AddQueueOptions()
                .AddDeleteMessageExtensions()
                .AddSingleton<Queue<QueueMessage>>(sp => queueFixture.Messages)

                .BuildServiceProvider();

            var med = serviceProvider.GetRequiredService<IMediator>();

            var res = await med.Send(EchoRequest.Default);

            res.Message.Should().Be(EchoRequest.Default.Message);

            var ctx = serviceProvider.GetRequiredService<Queue<QueueMessage>>();

            ctx.Should().HaveCount(0);
        }

        [Fact(DisplayName = "06. Queue is empty")]
        public void Test6() => queueFixture.ThenQueueIsEmpty();
    }
}
