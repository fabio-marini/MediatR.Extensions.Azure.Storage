using FluentAssertions;
using MediatR.Extensions.Azure.Storage.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MediatR.Extensions.Azure.Storage.Tests.Abstractions
{
    public class TestRequestBehaviorBase<TRequest, TResponse> : RequestBehaviorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public TestRequestBehaviorBase(ICommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }

    public class RequestBehaviorBaseTests
    {
        private readonly IServiceProvider svc;
        private readonly Mock<ILogger> log;
        private readonly Mock<PipelineContext> ctx;

        public RequestBehaviorBaseTests()
        {
            log = new Mock<ILogger>();
            ctx = new Mock<PipelineContext>();

            var cmd = new Mock<ICommand<TestCommand>>();
            var qry = new Mock<ICommand<TestQuery>>();

            svc = new ServiceCollection()

                .AddSingleton<Mock<ICommand<TestCommand>>>(sp => cmd)
                .AddSingleton<Mock<ICommand<TestQuery>>>(sp => qry)

                .AddSingleton<ICommand<TestCommand>>(sp => cmd.Object)
                .AddSingleton<ICommand<TestQuery>>(sp => qry.Object)

                .AddTransient<TestRequestBehaviorBase<TestCommand, Unit>>()
                .AddTransient<TestRequestBehaviorBase<TestQuery, TestResult>>()

                .AddTransient<PipelineContext>(sp => ctx.Object)
                .AddTransient<ILogger>(sp => log.Object)

                .BuildServiceProvider();
        }

        public static IEnumerable<object[]> TestData()
        {
            yield return new object[] { TestCommand.Default, Unit.Value };
            yield return new object[] { TestQuery.Default, TestResult.Default };
        }

        [Theory(DisplayName = "Behavior executes successfully"), MemberData(nameof(TestData))]
        public async Task Test1<TRequest, TResponse>(TRequest req, TResponse res) where TRequest : IRequest<TResponse>
        {
            var commandMock = svc.GetRequiredService<Mock<ICommand<TRequest>>>();

            commandMock.Setup(m => m.ExecuteAsync(req, CancellationToken.None)).Returns(Task.CompletedTask);

            var requestBehavior = svc.GetRequiredService<TestRequestBehaviorBase<TRequest, TResponse>>();

            await requestBehavior.Handle(req, CancellationToken.None, () => Task.FromResult(res));

            var logInvocation = log.Invocations.Where(i => i.Method.Name == "Log").Single();

            logInvocation.Arguments.OfType<LogLevel>().Single().Should().Be(LogLevel.Information);
        }

        [Theory(DisplayName = "Behavior handles exceptions"), MemberData(nameof(TestData))]
        public async Task Test2<TRequest, TResponse>(TRequest req, TResponse res) where TRequest : IRequest<TResponse>
        {
            var commandMock = svc.GetRequiredService<Mock<ICommand<TRequest>>>();

            commandMock.Setup(m => m.ExecuteAsync(req, CancellationToken.None)).Throws(new ArgumentException());

            var requestBehavior = svc.GetRequiredService<TestRequestBehaviorBase<TRequest, TResponse>>();

            await requestBehavior.Handle(req, CancellationToken.None, () => Task.FromResult(res));

            var logInvocation = log.Invocations.Where(i => i.Method.Name == "Log").Single();

            logInvocation.Arguments.OfType<LogLevel>().Single().Should().Be(LogLevel.Error);
            logInvocation.Arguments.OfType<ArgumentException>().Single();

            ctx.VerifyGet(m => m.Exceptions, Times.Once);
        }

        [Theory(DisplayName = "Behavior handles cancellations"), MemberData(nameof(TestData))]
        public async Task Test3<TRequest, TResponse>(TRequest req, TResponse res) where TRequest : IRequest<TResponse>
        {
            var requestBehavior = svc.GetRequiredService<TestRequestBehaviorBase<TRequest, TResponse>>();

            var src = new CancellationTokenSource(0);

            Func<Task> act = async () => await requestBehavior.Handle(req, src.Token, () => Task.FromResult(res));

            await act.Should().ThrowAsync<OperationCanceledException>();
        }
    }
}
