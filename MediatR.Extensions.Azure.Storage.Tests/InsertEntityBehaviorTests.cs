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
    public class InsertEntityBehaviorTests
    {
        private readonly IServiceProvider svc;
        private readonly Mock<InsertEntityOptions<TestCommand>> opt;
        private readonly IMediator med;
        private readonly Mock<CloudTable> tbl;
        private readonly Mock<ILogger> log;

        public InsertEntityBehaviorTests()
        {
            opt = new Mock<InsertEntityOptions<TestCommand>>();
            tbl = new Mock<CloudTable>(new Uri("http://127.0.0.1:10002/devstoreaccount1/table1"), null);
            log = new Mock<ILogger>();

            svc = new ServiceCollection()

                .AddMediatR(this.GetType())

                .AddTransient<IPipelineBehavior<TestCommand, Unit>, InsertEntityBehavior<TestCommand>>()

                .AddTransient<IOptions<InsertEntityOptions<TestCommand>>>(sp => Options.Create(opt.Object))

                .AddTransient<PipelineContext>()

                .AddTransient<ILogger>(sp => log.Object)

                .BuildServiceProvider();

            med = svc.GetRequiredService<IMediator>();
        }

        [Fact(DisplayName = "Behavior is disabled")]
        public async Task Test1()
        {
            var cmd = new TestCommand { Message = "Hello! :)" };

            _ = await med.Send(cmd);

            opt.Verify(m => m.IsEnabled, Times.Once);
            opt.Verify(m => m.CloudTable, Times.Never);
            opt.Verify(m => m.TableEntity, Times.Never);
        }

        [Fact(DisplayName = "CloudTable is not specified")]
        public async Task Test2()
        {
            opt.SetupProperty(m => m.IsEnabled, true);

            var cmd = new TestCommand { Message = "Hello! :)" };

            _ = await med.Send(cmd);

            opt.Verify(m => m.IsEnabled, Times.Once);
            opt.Verify(m => m.CloudTable, Times.Once);
            opt.Verify(m => m.TableEntity, Times.Never);
        }

        [Fact(DisplayName = "Behavior uses default TableEntity")]
        public async Task Test3()
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.CloudTable, tbl.Object);
            opt.SetupProperty(m => m.TableEntity, null);

            var cmd = new TestCommand { Message = "Hello! :)" };

            _ = await med.Send(cmd);

            var tableOperations = new List<TableOperation>();

            opt.Verify(m => m.IsEnabled, Times.Once);
            opt.Verify(m => m.CloudTable, Times.Exactly(2));
            opt.Verify(m => m.CloudTable.ExecuteAsync(Capture.In(tableOperations), CancellationToken.None), Times.Once);

            tableOperations.Should().HaveCount(1);
            tableOperations.Single().OperationType.Should().Be(TableOperationType.Insert);

            opt.Invocations.Where(i => i.Method.Name == "get_TableEntity").Should().HaveCount(2);
            opt.Invocations.Where(i => i.Method.Name == "set_TableEntity").Should().HaveCount(1);
        }

        [Fact(DisplayName = "Behavior uses specified TableEntity")]
        public async Task Test4()
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.CloudTable, tbl.Object);
            opt.SetupProperty(m => m.TableEntity, (cmd, ctx) => new DynamicTableEntity("PK1", "RK1"));

            var cmd = new TestCommand { Message = "Hello! :)" };

            _ = await med.Send(cmd);

            var tableOperations = new List<TableOperation>();

            opt.Verify(m => m.IsEnabled, Times.Once);
            opt.Verify(m => m.CloudTable, Times.Exactly(2));
            opt.Verify(m => m.CloudTable.ExecuteAsync(Capture.In(tableOperations), CancellationToken.None), Times.Once);

            tableOperations.Should().HaveCount(1);
            tableOperations.Single().OperationType.Should().Be(TableOperationType.Insert);

            opt.Invocations.Where(i => i.Method.Name == "get_TableEntity").Should().HaveCount(2);
            opt.Invocations.Where(i => i.Method.Name == "set_TableEntity").Should().HaveCount(0);
        }

        [Fact(DisplayName = "Exceptions are logged")]
        public async Task Test5()
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.CloudTable, tbl.Object);
            opt.SetupProperty(m => m.TableEntity, (cmd, ctx) => new DynamicTableEntity("PK1", "RK1"));

            tbl.Setup(m => m.ExecuteAsync(It.IsAny<TableOperation>(), CancellationToken.None)).Throws(new Exception("Failed ! :("));

            var cmd = new TestCommand { Message = "Hello! :)" };

            _ = await med.Send(cmd);

            var tableOperations = new List<TableOperation>();

            opt.Verify(m => m.IsEnabled, Times.Once);
            opt.Verify(m => m.CloudTable, Times.Exactly(2));
            opt.Verify(m => m.CloudTable.ExecuteAsync(Capture.In(tableOperations), CancellationToken.None), Times.Once);

            tableOperations.Should().HaveCount(1);
            tableOperations.Single().OperationType.Should().Be(TableOperationType.Insert);

            opt.Invocations.Where(i => i.Method.Name == "get_TableEntity").Should().HaveCount(2);
            opt.Invocations.Where(i => i.Method.Name == "set_TableEntity").Should().HaveCount(0);

            var logInvocation = log.Invocations.Where(i => i.Method.Name == "Log").Single();

            logInvocation.Arguments.OfType<LogLevel>().Single().Should().Be(LogLevel.Error);
            logInvocation.Arguments.OfType<Exception>().Single();
        }
    }
}
