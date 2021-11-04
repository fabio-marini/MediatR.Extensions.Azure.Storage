using Azure.Storage.Queues;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MediatR.Extensions.Azure.Storage.Tests.Extensions
{
    public class QueueMessageExtensionsTests
    {
        private readonly IServiceProvider svc;
        private readonly Mock<ILogger> log;
        private readonly Mock<PipelineContext> ctx;

        public QueueMessageExtensionsTests()
        {
            log = new Mock<ILogger>();
            ctx = new Mock<PipelineContext>();

            svc = new ServiceCollection()

                .AddQueueMessageExtensions()

                .AddTransient<PipelineContext>(sp => ctx.Object)
                .AddTransient<ILogger>(sp => log.Object)

                .BuildServiceProvider();
        }

        public static IEnumerable<object[]> TestData()
        {
            yield return new object[]
            {
                new Func<IServiceProvider, CancellationToken, Task> (async (svc, tkn) =>
                {
                    var bvr = svc.GetRequiredService<QueueRequestBehavior<TestCommand, Unit>>();

                    await bvr.Handle(TestCommand.Default, tkn, () => Unit.Task);
                }),
                new Func<IServiceProvider, Mock<QueueMessageOptions<TestCommand>>>(svc =>
                {
                    return svc.GetRequiredService<Mock<QueueMessageOptions<TestCommand>>>();
                })
            };
            yield return new object[]
            {
                new Func<IServiceProvider, CancellationToken, Task> (async (svc, tkn) =>
                {
                    var bvr = svc.GetRequiredService<QueueRequestBehavior<TestQuery, TestResult>>();

                    await bvr.Handle(TestQuery.Default, tkn, () => Task.FromResult(TestResult.Default));
                }),
                new Func<IServiceProvider, Mock<QueueMessageOptions<TestQuery>>>(svc =>
                {
                    return svc.GetRequiredService<Mock<QueueMessageOptions<TestQuery>>>();
                })
            };
            yield return new object[]
            {
                new Func<IServiceProvider, CancellationToken, Task> (async (svc, tkn) =>
                {
                    var bvr = svc.GetRequiredService<QueueRequestProcessor<TestCommand>>();

                    await bvr.Process(TestCommand.Default, tkn);
                }),
                new Func<IServiceProvider, Mock<QueueMessageOptions<TestCommand>>>(svc =>
                {
                    return svc.GetRequiredService<Mock<QueueMessageOptions<TestCommand>>>();
                })
            };
            yield return new object[]
            {
                new Func<IServiceProvider, CancellationToken, Task> (async (svc, tkn) =>
                {
                    var bvr = svc.GetRequiredService<QueueRequestProcessor<TestQuery>>();

                    await bvr.Process(TestQuery.Default, tkn);
                }),
                new Func<IServiceProvider, Mock<QueueMessageOptions<TestQuery>>>(svc =>
                {
                    return svc.GetRequiredService<Mock<QueueMessageOptions<TestQuery>>>();
                })
            };
            yield return new object[]
            {
                new Func<IServiceProvider, CancellationToken, Task> (async (svc, tkn) =>
                {
                    var bvr = svc.GetRequiredService<QueueResponseProcessor<TestCommand, Unit>>();

                    await bvr.Process(TestCommand.Default, Unit.Value, tkn);
                }),
                new Func<IServiceProvider, Mock<QueueMessageOptions<Unit>>>(svc =>
                {
                    return svc.GetRequiredService<Mock<QueueMessageOptions<Unit>>>();
                })
            };
            yield return new object[]
            {
                new Func<IServiceProvider, CancellationToken, Task> (async (svc, tkn) =>
                {
                    var bvr = svc.GetRequiredService<QueueResponseProcessor<TestQuery, TestResult>>();

                    await bvr.Process(TestQuery.Default, TestResult.Default, tkn);
                }),
                new Func<IServiceProvider, Mock<QueueMessageOptions<TestResult>>>(svc =>
                {
                    return svc.GetRequiredService<Mock<QueueMessageOptions<TestResult>>>();
                })
            };
        }

        [Theory(DisplayName = "Extension executes successfully"), MemberData(nameof(TestData))]
        public async Task Test1<TMessage>(Func<IServiceProvider, CancellationToken, Task> act,
            Func<IServiceProvider, Mock<QueueMessageOptions<TMessage>>> opt)
        {
            var que = new Mock<QueueClient>("UseDevelopmentStorage=true", "queue1");

            opt(svc).SetupProperty(m => m.IsEnabled, true);
            opt(svc).SetupProperty(m => m.QueueClient, que.Object);
            opt(svc).SetupProperty(m => m.QueueMessage, (req, ctx) => BinaryData.FromString("Hello world"));

            await act(svc, CancellationToken.None);

            var logInvocation = log.Invocations.Where(i => i.Method.Name == "Log").Single();

            logInvocation.Arguments.OfType<LogLevel>().Single().Should().Be(LogLevel.Information);
        }

        [Theory(DisplayName = "Extension handles exceptions"), MemberData(nameof(TestData))]
        public async Task Test2<TMessage>(Func<IServiceProvider, CancellationToken, Task> act,
            Func<IServiceProvider, Mock<QueueMessageOptions<TMessage>>> opt)
        {
            opt(svc).SetupProperty(m => m.IsEnabled, true);

            await act(svc, CancellationToken.None);

            var logInvocation = log.Invocations.Where(i => i.Method.Name == "Log").Single();

            logInvocation.Arguments.OfType<LogLevel>().Single().Should().Be(LogLevel.Error);
            logInvocation.Arguments.OfType<ArgumentNullException>().Single();

            ctx.VerifyGet(m => m.Exceptions, Times.Once);
        }

        [Theory(DisplayName = "Extension handles cancellations"), MemberData(nameof(TestData))]
        public async Task Test3<TMessage>(Func<IServiceProvider, CancellationToken, Task> act,
            Func<IServiceProvider, Mock<QueueMessageOptions<TMessage>>> opt)
        {
            _ = opt(svc);

            var src = new CancellationTokenSource(0);

            await act(svc, src.Token);

            var logInvocation = log.Invocations.Where(i => i.Method.Name == "Log").Single();

            logInvocation.Arguments.OfType<LogLevel>().Single().Should().Be(LogLevel.Error);
            logInvocation.Arguments.OfType<OperationCanceledException>().Single();

            ctx.VerifyGet(m => m.Exceptions, Times.Once);
        }
    }
}
