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

namespace MediatR.Extensions.Azure.Storage.Tests
{
    public class InsertEntityBehaviorFixture<TRequest> : InsertEntityBehaviorFixture<TRequest, Unit> where TRequest : IRequest<Unit>
    {
    }

    public class InsertEntityBehaviorFixture<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IServiceProvider svc;
        private readonly Mock<InsertEntityOptions<TRequest>> opt;
        private readonly IMediator med;
        private readonly Mock<CloudTable> tbl;
        private readonly Mock<ILogger> log;

        public InsertEntityBehaviorFixture()
        {
            opt = new Mock<InsertEntityOptions<TRequest>>();
            tbl = new Mock<CloudTable>(new Uri("http://127.0.0.1:10002/devstoreaccount1/table1"), null);
            log = new Mock<ILogger>();

            svc = new ServiceCollection()

                .AddMediatR(this.GetType())

                .AddTransient<InsertEntityCommand<TRequest>>()

                .AddTransient<IPipelineBehavior<TRequest, TResponse>, InsertRequestBehavior<TRequest, TResponse>>()

                .AddTransient<IOptions<InsertEntityOptions<TRequest>>>(sp => Options.Create(opt.Object))

                .AddTransient<PipelineContext>()

                .AddTransient<ILogger>(sp => log.Object)

                .BuildServiceProvider();

            med = svc.GetRequiredService<IMediator>();
        }

        public async Task<TResponse> Test1(TRequest req)
        {
            var res = await med.Send(req);

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.CloudTable, Times.Never);
            opt.VerifyGet(m => m.TableEntity, Times.Never);

            return res;
        }

        public async Task<TResponse> Test2(TRequest req)
        {
            opt.SetupProperty(m => m.IsEnabled, true);

            var res = await med.Send(req);

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.CloudTable, Times.Once);
            opt.VerifyGet(m => m.TableEntity, Times.Never);

            return res;
        }

        public async Task<TResponse> Test3(TRequest req)
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.CloudTable, tbl.Object);
            opt.SetupProperty(m => m.TableEntity, null);

            var res = await med.Send(req);

            var tableOperations = new List<TableOperation>();

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.CloudTable, Times.Exactly(2));
            opt.VerifyGet(m => m.TableEntity, Times.Exactly(2));

            opt.VerifySet(m => m.TableEntity = It.IsAny<Func<TRequest, PipelineContext, ITableEntity>>(), Times.Once);

            opt.Verify(m => m.CloudTable.ExecuteAsync(Capture.In(tableOperations), CancellationToken.None), Times.Once);

            tableOperations.Should().HaveCount(1);
            tableOperations.Single().OperationType.Should().Be(TableOperationType.Insert);

            return res;
        }

        public async Task<TResponse> Test4(TRequest req)
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.CloudTable, tbl.Object);
            opt.SetupProperty(m => m.TableEntity, (cmd, ctx) => new DynamicTableEntity("PK1", "RK1"));

            var res = await med.Send(req);

            var tableOperations = new List<TableOperation>();

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.CloudTable, Times.Exactly(2));
            opt.VerifyGet(m => m.TableEntity, Times.Exactly(2));

            opt.VerifySet(m => m.TableEntity = It.IsAny<Func<TRequest, PipelineContext, ITableEntity>>(), Times.Never);

            opt.Verify(m => m.CloudTable.ExecuteAsync(Capture.In(tableOperations), CancellationToken.None), Times.Once);

            tableOperations.Should().HaveCount(1);
            tableOperations.Single().OperationType.Should().Be(TableOperationType.Insert);

            return res;
        }

        public async Task<TResponse> Test5(TRequest req)
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.CloudTable, tbl.Object);
            opt.SetupProperty(m => m.TableEntity, (cmd, ctx) => new DynamicTableEntity("PK1", "RK1"));

            tbl.Setup(m => m.ExecuteAsync(It.IsAny<TableOperation>(), CancellationToken.None)).Throws(new Exception("Failed! :("));

            var res = await med.Send(req);

            var tableOperations = new List<TableOperation>();

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.CloudTable, Times.Exactly(2));
            opt.VerifyGet(m => m.TableEntity, Times.Exactly(2));

            opt.VerifySet(m => m.TableEntity = It.IsAny<Func<TRequest, PipelineContext, ITableEntity>>(), Times.Never);

            opt.Verify(m => m.CloudTable.ExecuteAsync(Capture.In(tableOperations), CancellationToken.None), Times.Once);

            tableOperations.Should().HaveCount(1);
            tableOperations.Single().OperationType.Should().Be(TableOperationType.Insert);

            var logInvocation = log.Invocations.Where(i => i.Method.Name == "Log").Single();

            logInvocation.Arguments.OfType<LogLevel>().Single().Should().Be(LogLevel.Error);
            logInvocation.Arguments.OfType<Exception>().Single().Message.Should().Be("Failed! :(");

            return res;
        }
    }
}
