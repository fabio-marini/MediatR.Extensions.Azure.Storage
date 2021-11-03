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
    public class InsertResponseProcessorTests
    {
        private readonly IServiceProvider svc;
        private readonly Mock<ILogger> log;
        private readonly Mock<InsertEntityOptions<Unit>> cmd;
        private readonly Mock<InsertEntityOptions<TestResult>> qry;

        public InsertResponseProcessorTests()
        {
            log = new Mock<ILogger>();
            cmd = new Mock<InsertEntityOptions<Unit>>();
            qry = new Mock<InsertEntityOptions<TestResult>>();

            svc = new ServiceCollection()

                .AddTransient<InsertResponseProcessor<TestCommand, Unit>>()
                .AddTransient<IOptions<InsertEntityOptions<Unit>>>(sp => Options.Create(cmd.Object))

                .AddTransient<InsertResponseProcessor<TestQuery, TestResult>>()
                .AddTransient<IOptions<InsertEntityOptions<TestResult>>>(sp => Options.Create(qry.Object))

                .AddTransient<ILogger>(sp => log.Object)

                .BuildServiceProvider();
        }

        public static IEnumerable<object[]> TestData()
        {
            yield return new object[] { TestCommand.Default, Unit.Value };
            yield return new object[] { TestQuery.Default, TestResult.Default };
        }

        [Theory(DisplayName = "Processor executes successfully"), MemberData(nameof(TestData))]
        public async Task Test1<TRequest, TResponse>(TRequest req, TResponse res) where TRequest : IRequest<TResponse>
        {
            var tbl = new Mock<CloudTable>(new Uri("http://127.0.0.1:10002/devstoreaccount1/table1"), null);

            cmd.SetupProperty(m => m.IsEnabled, true);
            qry.SetupProperty(m => m.IsEnabled, true);

            cmd.SetupProperty(m => m.CloudTable, tbl.Object);
            qry.SetupProperty(m => m.CloudTable, tbl.Object);

            cmd.SetupProperty(m => m.TableEntity, (req, ctx) => new DynamicTableEntity("PK1", "RK1"));
            qry.SetupProperty(m => m.TableEntity, (req, ctx) => new DynamicTableEntity("PK1", "RK1"));

            var prc = svc.GetRequiredService<InsertResponseProcessor<TRequest, TResponse>>();

            await prc.Process(req, res, CancellationToken.None);

            var logInvocation = log.Invocations.Where(i => i.Method.Name == "Log").Single();

            logInvocation.Arguments.OfType<LogLevel>().Single().Should().Be(LogLevel.Information);
        }

        [Theory(DisplayName = "Processor handles exceptions"), MemberData(nameof(TestData))]
        public async Task Test2<TRequest, TResponse>(TRequest req, TResponse res) where TRequest : IRequest<TResponse>
        {
            cmd.SetupProperty(m => m.IsEnabled, true);
            qry.SetupProperty(m => m.IsEnabled, true);

            var prc = svc.GetRequiredService<InsertResponseProcessor<TRequest, TResponse>>();

            await prc.Process(req, res, CancellationToken.None);

            var logInvocation = log.Invocations.Where(i => i.Method.Name == "Log").Single();

            logInvocation.Arguments.OfType<LogLevel>().Single().Should().Be(LogLevel.Error);
            logInvocation.Arguments.OfType<ArgumentNullException>().Single();
        }
    }
}
