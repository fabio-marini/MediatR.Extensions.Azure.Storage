using FluentAssertions;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MediatR.Extensions.Azure.Storage.Tests.Processors
{
    public class InsertRequestProcessorTests
    {
        private readonly IServiceProvider svc;
        private readonly Mock<ILogger> log;
        private readonly Mock<InsertEntityOptions<TestCommand>> cmd;
        private readonly Mock<InsertEntityOptions<TestQuery>> qry;
        private readonly Mock<PipelineContext> ctx;

        public InsertRequestProcessorTests()
        {
            log = new Mock<ILogger>();
            cmd = new Mock<InsertEntityOptions<TestCommand>>();
            qry = new Mock<InsertEntityOptions<TestQuery>>();
            ctx = new Mock<PipelineContext>();

            svc = new ServiceCollection()

                .AddTransient<InsertRequestProcessor<TestCommand>>()
                .AddTransient<IOptions<InsertEntityOptions<TestCommand>>>(sp => Options.Create(cmd.Object))

                .AddTransient<InsertRequestProcessor<TestQuery>>()
                .AddTransient<IOptions<InsertEntityOptions<TestQuery>>>(sp => Options.Create(qry.Object))

                .AddTransient<PipelineContext>(sp => ctx.Object)
                .AddTransient<ILogger>(sp => log.Object)

                .BuildServiceProvider();
        }

        public static IEnumerable<object[]> TestData()
        {
            yield return new object[] { TestCommand.Default, new Func<Task<Unit>>(() => Unit.Task) };
            yield return new object[] { TestQuery.Default, new Func<Task<TestResult>>(() => Task.FromResult(TestResult.Default)) };
        }

        [Theory(DisplayName = "Processor executes successfully"), MemberData(nameof(TestData))]
        public async Task Test1<TRequest, TResponse>(TRequest req, Func<Task<TResponse>> res) where TRequest : IRequest<TResponse>
        {
            var tbl = new Mock<CloudTable>(new Uri("http://127.0.0.1:10002/devstoreaccount1/table1"), null);

            cmd.SetupProperty(m => m.IsEnabled, true);
            qry.SetupProperty(m => m.IsEnabled, true);

            cmd.SetupProperty(m => m.CloudTable, tbl.Object);
            qry.SetupProperty(m => m.CloudTable, tbl.Object);

            cmd.SetupProperty(m => m.TableEntity, (req, ctx) => new DynamicTableEntity("PK1", "RK1"));
            qry.SetupProperty(m => m.TableEntity, (req, ctx) => new DynamicTableEntity("PK1", "RK1"));

            var prc = svc.GetRequiredService<InsertRequestProcessor<TRequest>>();

            await prc.Process(req, CancellationToken.None);

            var logInvocation = log.Invocations.Where(i => i.Method.Name == "Log").Single();

            logInvocation.Arguments.OfType<LogLevel>().Single().Should().Be(LogLevel.Information);
        }

        [Theory(DisplayName = "Processor handles exceptions"), MemberData(nameof(TestData))]
        public async Task Test2<TRequest, TResponse>(TRequest req, Func<Task<TResponse>> res) where TRequest : IRequest<TResponse>
        {
            cmd.SetupProperty(m => m.IsEnabled, true);
            qry.SetupProperty(m => m.IsEnabled, true);

            var prc = svc.GetRequiredService<InsertRequestProcessor<TRequest>>();

            await prc.Process(req, CancellationToken.None);

            var logInvocation = log.Invocations.Where(i => i.Method.Name == "Log").Single();

            logInvocation.Arguments.OfType<LogLevel>().Single().Should().Be(LogLevel.Error);
            logInvocation.Arguments.OfType<ArgumentNullException>().Single();

            ctx.VerifyGet(m => m.Exceptions, Times.Once);
        }

        [Theory(DisplayName = "Processor handles cancellations"), MemberData(nameof(TestData))]
        public async Task Test3<TRequest, TResponse>(TRequest req, Func<Task<TResponse>> res) where TRequest : IRequest<TResponse>
        {
            var src = new CancellationTokenSource(0);

            var prc = svc.GetRequiredService<InsertRequestProcessor<TRequest>>();

            await prc.Process(req, src.Token);

            var logInvocation = log.Invocations.Where(i => i.Method.Name == "Log").Single();

            logInvocation.Arguments.OfType<LogLevel>().Single().Should().Be(LogLevel.Error);
            logInvocation.Arguments.OfType<OperationCanceledException>().Single();

            ctx.VerifyGet(m => m.Exceptions, Times.Once);
        }
    }
}
