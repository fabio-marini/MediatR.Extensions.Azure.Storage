using FluentAssertions;
using MediatR.Extensions.Azure.Storage.Abstractions;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MediatR.Extensions.Azure.Storage.Tests.Commands
{
    public class InsertEntityCommandTests
    {
        private readonly IServiceProvider svc;
        private readonly Mock<TableOptions<TestMessage>> opt;
        private readonly Mock<CloudTable> tbl;

        private readonly InsertEntityCommand<TestMessage> cmd;

        public InsertEntityCommandTests()
        {
            opt = new Mock<TableOptions<TestMessage>>();
            tbl = new Mock<CloudTable>(new Uri("http://127.0.0.1:10002/devstoreaccount1/table1"), null);

            svc = new ServiceCollection()

                .AddTransient<InsertEntityCommand<TestMessage>>()
                .AddTransient<IOptions<TableOptions<TestMessage>>>(sp => Options.Create(opt.Object))

                .BuildServiceProvider();

            cmd = svc.GetRequiredService<InsertEntityCommand<TestMessage>>();
        }

        [Fact(DisplayName = "Command is disabled")]
        public async Task Test1a()
        {
            await cmd.ExecuteAsync(TestMessage.Default, CancellationToken.None);

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.CloudTable, Times.Never);
            opt.VerifyGet(m => m.TableEntity, Times.Never);
        }

        [Fact(DisplayName = "Command is cancelled")]
        public async Task Test1b()
        {
            var src = new CancellationTokenSource(0);

            Func<Task> act = async () => await cmd.ExecuteAsync(TestMessage.Default, src.Token);

            await act.Should().ThrowAsync<OperationCanceledException>();

            opt.VerifyGet(m => m.IsEnabled, Times.Never);
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

        [Fact(DisplayName = "Command uses default TableEntity")]
        public async Task Test3()
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.CloudTable, tbl.Object);
            opt.SetupProperty(m => m.TableEntity, null);

            var res = new TableResult { HttpStatusCode = 200 };

            tbl.Setup(m => m.ExecuteAsync(It.IsAny<TableOperation>(), CancellationToken.None)).ReturnsAsync(res);

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

        [Fact(DisplayName = "Exceptions are wrapped in a CommandException")]
        public async Task Test4()
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.CloudTable, tbl.Object);
            opt.SetupProperty(m => m.TableEntity, null);

            tbl.Setup(m => m.ExecuteAsync(It.IsAny<TableOperation>(), CancellationToken.None))
                .ThrowsAsync(new ArgumentNullException());

            Func<Task> act = async () => await cmd.ExecuteAsync(TestMessage.Default, CancellationToken.None);

            await act.Should().ThrowAsync<CommandException>();

            var tableOperations = new List<TableOperation>();

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.CloudTable, Times.Exactly(2));
            opt.VerifyGet(m => m.TableEntity, Times.Exactly(2));

            opt.VerifySet(m => m.TableEntity = It.IsAny<Func<TestMessage, PipelineContext, ITableEntity>>(), Times.Once);

            opt.Verify(m => m.CloudTable.ExecuteAsync(Capture.In(tableOperations), CancellationToken.None), Times.Once);

            tableOperations.Should().HaveCount(1);
            tableOperations.Single().OperationType.Should().Be(TableOperationType.Insert);
        }

        [Fact(DisplayName = "Command uses specified TableEntity")]
        public async Task Test5()
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.CloudTable, tbl.Object);
            opt.SetupProperty(m => m.TableEntity, (cmd, ctx) => new DynamicTableEntity("PK1", "RK1"));

            var res = new TableResult { HttpStatusCode = 200 };

            tbl.Setup(m => m.ExecuteAsync(It.IsAny<TableOperation>(), CancellationToken.None)).ReturnsAsync(res);

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
    }
}
