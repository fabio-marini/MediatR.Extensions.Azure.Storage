using FluentAssertions;
using MediatR.Extensions.Abstractions;
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

namespace MediatR.Extensions.Azure.Storage.Tests.Commands.Tables
{
    public class RetrieveEntityCommandTests
    {
        private readonly IServiceProvider svc;
        private readonly Mock<TableOptions<EchoRequest>> opt;
        private readonly Mock<CloudTable> tbl;

        private readonly RetrieveEntityCommand<EchoRequest> cmd;

        public RetrieveEntityCommandTests()
        {
            opt = new Mock<TableOptions<EchoRequest>>();
            tbl = new Mock<CloudTable>(new Uri("http://127.0.0.1:10002/devstoreaccount1/table1"), null);

            svc = new ServiceCollection()

                .AddTransient<RetrieveEntityCommand<EchoRequest>>()
                .AddTransient<IOptions<TableOptions<EchoRequest>>>(sp => Options.Create(opt.Object))

                .BuildServiceProvider();

            cmd = svc.GetRequiredService<RetrieveEntityCommand<EchoRequest>>();
        }

        [Fact(DisplayName = "Command is disabled")]
        public async Task Test1()
        {
            await cmd.ExecuteAsync(EchoRequest.Default, CancellationToken.None);

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.CloudTable, Times.Never);
            opt.VerifyGet(m => m.TableEntity, Times.Never);
        }

        [Fact(DisplayName = "Command is cancelled")]
        public async Task Test2()
        {
            var src = new CancellationTokenSource(0);

            Func<Task> act = async () => await cmd.ExecuteAsync(EchoRequest.Default, src.Token);

            await act.Should().ThrowAsync<OperationCanceledException>();

            opt.VerifyGet(m => m.IsEnabled, Times.Never);
        }

        [Fact(DisplayName = "CloudTable is not specified")]
        public async Task Test3()
        {
            opt.SetupProperty(m => m.IsEnabled, true);

            Func<Task> act = async () => await cmd.ExecuteAsync(EchoRequest.Default, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentNullException>();

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.CloudTable, Times.Once);
            opt.VerifyGet(m => m.TableEntity, Times.Never);
        }

        [Fact(DisplayName = "TableEntity is not specified")]
        public async Task Test4()
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.CloudTable, tbl.Object);
            opt.SetupProperty(m => m.TableEntity, null);

            Func<Task> act = async () => await cmd.ExecuteAsync(EchoRequest.Default, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentNullException>();

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.CloudTable, Times.Once);
            opt.VerifyGet(m => m.TableEntity, Times.Once);
        }

        [Fact(DisplayName = "Command throws CommandException")]
        public async Task Test5()
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.CloudTable, tbl.Object);
            opt.SetupProperty(m => m.TableEntity, (req, ctx) => new DynamicTableEntity("PK1", "RK1"));

            tbl.Setup(m => m.ExecuteAsync(It.IsAny<TableOperation>(), CancellationToken.None))
                .ThrowsAsync(new ArgumentNullException());

            Func<Task> act = async () => await cmd.ExecuteAsync(EchoRequest.Default, CancellationToken.None);

            await act.Should().ThrowAsync<CommandException>();

            var tableOperations = new List<TableOperation>();

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.CloudTable, Times.Exactly(2));
            opt.VerifyGet(m => m.TableEntity, Times.Exactly(2));

            opt.Verify(m => m.CloudTable.ExecuteAsync(Capture.In(tableOperations), CancellationToken.None), Times.Once);

            tableOperations.Should().HaveCount(1);
            tableOperations.Single().OperationType.Should().Be(TableOperationType.Retrieve);
        }

        [Fact(DisplayName = "Command completes successfully")]
        public async Task Test6()
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.CloudTable, tbl.Object);
            opt.SetupProperty(m => m.TableEntity, (req, ctx) => new DynamicTableEntity("PK1", "RK1"));

            var res = new TableResult { HttpStatusCode = 200 };

            tbl.Setup(m => m.ExecuteAsync(It.IsAny<TableOperation>(), CancellationToken.None)).ReturnsAsync(res);

            await cmd.ExecuteAsync(EchoRequest.Default, CancellationToken.None);

            var tableOperations = new List<TableOperation>();

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.CloudTable, Times.Exactly(2));
            opt.VerifyGet(m => m.TableEntity, Times.Exactly(2));
            opt.VerifyGet(m => m.Retrieved, Times.Once);

            opt.Verify(m => m.CloudTable.ExecuteAsync(Capture.In(tableOperations), CancellationToken.None), Times.Once);

            tableOperations.Should().HaveCount(1);
            tableOperations.Single().OperationType.Should().Be(TableOperationType.Retrieve);
        }
    }
}
