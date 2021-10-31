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

namespace MediatR.Extensions.Azure.Storage.Tests
{
    public class InsertEntityCommandTests
    {
        private readonly IServiceProvider svc;
        private readonly Mock<InsertEntityOptions<TestMessage>> opt;
        private readonly Mock<CloudTable> tbl;
        private readonly Mock<ILogger> log;

        private readonly InsertEntityCommand<TestMessage> cmd;

        public InsertEntityCommandTests()
        {
            opt = new Mock<InsertEntityOptions<TestMessage>>();
            tbl = new Mock<CloudTable>(new Uri("http://127.0.0.1:10002/devstoreaccount1/table1"), null);
            log = new Mock<ILogger>();

            svc = new ServiceCollection()

                .AddTransient<InsertEntityCommand<TestMessage>>()

                .AddTransient<IOptions<InsertEntityOptions<TestMessage>>>(sp => Options.Create(opt.Object))
                .AddTransient<ILogger>(sp => log.Object)
                .AddTransient<PipelineContext>()

                .BuildServiceProvider();

            cmd = svc.GetRequiredService<InsertEntityCommand<TestMessage>>();
        }

        [Fact(DisplayName = "Behavior is disabled")]
        public async Task Test1()
        {
            await cmd.ExecuteAsync(TestMessage.Default, CancellationToken.None);

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.CloudTable, Times.Never);
            opt.VerifyGet(m => m.TableEntity, Times.Never);
        }

        [Fact(DisplayName = "CloudTable is not specified")]
        public async Task Test2()
        {
            opt.SetupProperty(m => m.IsEnabled, true);

            Func<Task> act = async () => await cmd.ExecuteAsync(TestMessage.Default, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentNullException>();

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.CloudTable, Times.Once);
            opt.VerifyGet(m => m.TableEntity, Times.Never);
        }

        [Fact(DisplayName = "Behavior uses default TableEntity")]
        public async Task Test3()
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.CloudTable, tbl.Object);
            opt.SetupProperty(m => m.TableEntity, null);

            await cmd.ExecuteAsync(TestMessage.Default, CancellationToken.None);

            var tableOperations = new List<TableOperation>();

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.CloudTable, Times.Exactly(2));
            opt.VerifyGet(m => m.TableEntity, Times.Exactly(2));

            opt.VerifySet(m => m.TableEntity = It.IsAny<Func<TestMessage, PipelineContext, ITableEntity>>(), Times.Once);

            opt.Verify(m => m.CloudTable.ExecuteAsync(Capture.In(tableOperations), CancellationToken.None), Times.Once);

            tableOperations.Should().HaveCount(1);
            tableOperations.Single().OperationType.Should().Be(TableOperationType.Insert);
        }

        [Fact(DisplayName = "Behavior uses specified TableEntity")]
        public async Task Test4()
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.CloudTable, tbl.Object);
            opt.SetupProperty(m => m.TableEntity, (cmd, ctx) => new DynamicTableEntity("PK1", "RK1"));

            await cmd.ExecuteAsync(TestMessage.Default, CancellationToken.None);

            var tableOperations = new List<TableOperation>();

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.CloudTable, Times.Exactly(2));
            opt.VerifyGet(m => m.TableEntity, Times.Exactly(2));

            opt.VerifySet(m => m.TableEntity = It.IsAny<Func<TestMessage, PipelineContext, ITableEntity>>(), Times.Never);

            opt.Verify(m => m.CloudTable.ExecuteAsync(Capture.In(tableOperations), CancellationToken.None), Times.Once);

            tableOperations.Should().HaveCount(1);
            tableOperations.Single().OperationType.Should().Be(TableOperationType.Insert);
        }

        [Fact(DisplayName = "Exceptions are logged", Skip = "TODO: Move me to behavior/processor test class")]
        public async Task Test5()
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.CloudTable, tbl.Object);
            opt.SetupProperty(m => m.TableEntity, (cmd, ctx) => new DynamicTableEntity("PK1", "RK1"));

            tbl.Setup(m => m.ExecuteAsync(It.IsAny<TableOperation>(), CancellationToken.None)).Throws(new Exception("Failed! :("));

            await cmd.ExecuteAsync(TestMessage.Default, CancellationToken.None);

            var tableOperations = new List<TableOperation>();

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.CloudTable, Times.Exactly(2));
            opt.VerifyGet(m => m.TableEntity, Times.Exactly(2));

            opt.VerifySet(m => m.TableEntity = It.IsAny<Func<TestMessage, PipelineContext, ITableEntity>>(), Times.Never);

            opt.Verify(m => m.CloudTable.ExecuteAsync(Capture.In(tableOperations), CancellationToken.None), Times.Once);

            tableOperations.Should().HaveCount(1);
            tableOperations.Single().OperationType.Should().Be(TableOperationType.Insert);

            var logInvocation = log.Invocations.Where(i => i.Method.Name == "Log").Single();

            logInvocation.Arguments.OfType<LogLevel>().Single().Should().Be(LogLevel.Error);
            logInvocation.Arguments.OfType<Exception>().Single().Message.Should().Be("Failed! :(");
        }
    }
}
