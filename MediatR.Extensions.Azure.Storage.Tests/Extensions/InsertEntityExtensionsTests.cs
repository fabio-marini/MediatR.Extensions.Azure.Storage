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
    public class InsertEntityExtensionsTests
    {
        private readonly IServiceProvider svc;
        private readonly Mock<ILogger> log;
        private readonly Mock<PipelineContext> ctx;

        public InsertEntityExtensionsTests()
        {
            log = new Mock<ILogger>();
            ctx = new Mock<PipelineContext>();

            svc = new ServiceCollection()

                .AddTableExtensions<TestCommand, Unit>()
                .AddTableExtensions<TestQuery, TestResult>()

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
                    var bvr = svc.GetRequiredService<InsertRequestBehavior<TestCommand, Unit>>();

                    await bvr.Handle(TestCommand.Default, tkn, () => Unit.Task);
                }),
                new Func<IServiceProvider, Mock<InsertEntityCommand<TestCommand>>>(svc =>
                {
                    return svc.GetRequiredService<Mock<InsertEntityCommand<TestCommand>>>();
                })
            };
            yield return new object[]
            {
                new Func<IServiceProvider, CancellationToken, Task> (async (svc, tkn) =>
                {
                    var bvr = svc.GetRequiredService<InsertRequestBehavior<TestQuery, TestResult>>();

                    await bvr.Handle(TestQuery.Default, tkn, () => Task.FromResult(TestResult.Default));
                }),
                new Func<IServiceProvider, Mock<InsertEntityCommand<TestQuery>>>(svc =>
                {
                    return svc.GetRequiredService<Mock<InsertEntityCommand<TestQuery>>>();
                })
            };
            yield return new object[]
            {
                new Func<IServiceProvider, CancellationToken, Task> (async (svc, tkn) =>
                {
                    var bvr = svc.GetRequiredService<InsertResponseBehavior<TestCommand, Unit>>();

                    await bvr.Handle(TestCommand.Default, tkn, () => Unit.Task);
                }),
                new Func<IServiceProvider, Mock<InsertEntityCommand<Unit>>>(svc =>
                {
                    return svc.GetRequiredService<Mock<InsertEntityCommand<Unit>>>();
                })
            };
            yield return new object[]
            {
                new Func<IServiceProvider, CancellationToken, Task> (async (svc, tkn) =>
                {
                    var bvr = svc.GetRequiredService<InsertResponseBehavior<TestQuery, TestResult>>();

                    await bvr.Handle(TestQuery.Default, tkn, () => Task.FromResult(TestResult.Default));
                }),
                new Func<IServiceProvider, Mock<InsertEntityCommand<TestResult>>>(svc =>
                {
                    return svc.GetRequiredService<Mock<InsertEntityCommand<TestResult>>>();
                })
            };
            yield return new object[]
            {
                new Func<IServiceProvider, CancellationToken, Task> (async (svc, tkn) =>
                {
                    var bvr = svc.GetRequiredService<InsertRequestProcessor<TestCommand>>();

                    await bvr.Process(TestCommand.Default, tkn);
                }),
                new Func<IServiceProvider, Mock<InsertEntityCommand<TestCommand>>>(svc =>
                {
                    return svc.GetRequiredService<Mock<InsertEntityCommand<TestCommand>>>();
                })
            };
            yield return new object[]
            {
                new Func<IServiceProvider, CancellationToken, Task> (async (svc, tkn) =>
                {
                    var bvr = svc.GetRequiredService<InsertRequestProcessor<TestQuery>>();

                    await bvr.Process(TestQuery.Default, tkn);
                }),
                new Func<IServiceProvider, Mock<InsertEntityCommand<TestQuery>>>(svc =>
                {
                    return svc.GetRequiredService<Mock<InsertEntityCommand<TestQuery>>>();
                })
            };
            yield return new object[]
            {
                new Func<IServiceProvider, CancellationToken, Task> (async (svc, tkn) =>
                {
                    var bvr = svc.GetRequiredService<InsertResponseProcessor<TestCommand, Unit>>();

                    await bvr.Process(TestCommand.Default, Unit.Value, tkn);
                }),
                new Func<IServiceProvider, Mock<InsertEntityCommand<Unit>>>(svc =>
                {
                    return svc.GetRequiredService<Mock<InsertEntityCommand<Unit>>>();
                })
            };
            yield return new object[]
            {
                new Func<IServiceProvider, CancellationToken, Task> (async (svc, tkn) =>
                {
                    var bvr = svc.GetRequiredService<InsertResponseProcessor<TestQuery, TestResult>>();

                    await bvr.Process(TestQuery.Default, TestResult.Default, tkn);
                }),
                new Func<IServiceProvider, Mock<InsertEntityCommand<TestResult>>>(svc =>
                {
                    return svc.GetRequiredService<Mock<InsertEntityCommand<TestResult>>>();
                })
            };
        }

        [Theory(DisplayName = "Extension executes successfully"), MemberData(nameof(TestData))]
        public async Task Test1<TMessage>(Func<IServiceProvider, CancellationToken, Task> act,
            Func<IServiceProvider, Mock<InsertEntityCommand<TMessage>>> cmd)
        {
            cmd(svc).Setup(m => m.ExecuteAsync(It.IsAny<TMessage>(), CancellationToken.None)).Returns(Task.CompletedTask);

            await act(svc, CancellationToken.None);

            var logInvocation = log.Invocations.Where(i => i.Method.Name == "Log").Single();

            logInvocation.Arguments.OfType<LogLevel>().Single().Should().Be(LogLevel.Information);
        }

        [Theory(DisplayName = "Extension handles exceptions"), MemberData(nameof(TestData))]
        public async Task Test2<TMessage>(Func<IServiceProvider, CancellationToken, Task> act,
            Func<IServiceProvider, Mock<InsertEntityCommand<TMessage>>> cmd)
        {
            cmd(svc).Setup(m => m.ExecuteAsync(It.IsAny<TMessage>(), CancellationToken.None)).Throws(new ArgumentNullException());

            await act(svc, CancellationToken.None);

            var logInvocation = log.Invocations.Where(i => i.Method.Name == "Log").Single();

            logInvocation.Arguments.OfType<LogLevel>().Single().Should().Be(LogLevel.Error);
            logInvocation.Arguments.OfType<ArgumentNullException>().Single();

            ctx.VerifyGet(m => m.Exceptions, Times.Once);
        }

        [Theory(DisplayName = "Extension handles cancellations"), MemberData(nameof(TestData))]
        public async Task Test3<TMessage>(Func<IServiceProvider, CancellationToken, Task> act,
            Func<IServiceProvider, Mock<InsertEntityCommand<TMessage>>> cmd)
        {
            _ = cmd(svc);

            var src = new CancellationTokenSource(0);

            Func<Task> act2 = async () => await act(svc, src.Token);

            await act2.Should().ThrowAsync<OperationCanceledException>();
        }
    }
}
