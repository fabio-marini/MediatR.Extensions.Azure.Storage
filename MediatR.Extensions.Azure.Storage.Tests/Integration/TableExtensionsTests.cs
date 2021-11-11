using FluentAssertions;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MediatR.Extensions.Azure.Storage.Tests.Integration
{
    [Trait("TestCategory", "Integration")]
    public class TableExtensionsTests : IClassFixture<TableFixture>
    {
        private readonly TableFixture tableFixture;

        public TableExtensionsTests(TableFixture tableFixture)
        {
            this.tableFixture = tableFixture;
        }

        public static IEnumerable<object[]> TestData()
        { 
            yield return new object[] { TestCommand.Default, Unit.Value };
            yield return new object[] { TestQuery.Default, TestResult.Default };
        }

        [Theory(DisplayName = "All TableRequestBehaviors execute correctly"), MemberData(nameof(TestData))]
        public async Task Test1<TRequest, TResponse>(TRequest req, TResponse res) where TRequest : IRequest<TResponse>
        {
            var rk = Guid.NewGuid().ToString();

            var opt = new TableOptions<TRequest>
            {
                IsEnabled = true,
                CloudTable = tableFixture.CloudTable,
                TableEntity = (req, ctx) => new DynamicTableEntity("IntegrationTests", rk) { ETag = "*" },
                Retrieved = (res, req, ctx) =>
                {
                    var dte = res.Result as DynamicTableEntity;

                    dte.Should().NotBeNull();

                    var obj = JsonConvert.DeserializeObject<TRequest>(dte.Properties["Content"].StringValue);

                    obj.Should().NotBeNull();

                    return Task.CompletedTask;
                }
            };

            var serviceProvider = new ServiceCollection()

                .AddTableExtensions<TRequest, TResponse>(sp => Options.Create(opt))
                .BuildServiceProvider();

            var insertExtension = serviceProvider.GetRequiredService<InsertEntityRequestBehavior<TRequest, TResponse>>();
            var retrieveExtension = serviceProvider.GetRequiredService<RetrieveEntityRequestBehavior<TRequest, TResponse>>();
            var deleteExtension = serviceProvider.GetRequiredService<DeleteEntityRequestBehavior<TRequest, TResponse>>();

            opt.CloudTable.ExecuteQuery(new TableQuery()).Should().HaveCount(0);

            _ = await insertExtension.Handle(req, CancellationToken.None, () => Task.FromResult(res));

            opt.CloudTable.ExecuteQuery(new TableQuery()).Should().HaveCount(1);

            _ = await retrieveExtension.Handle(req, CancellationToken.None, () => Task.FromResult(res));

            _ = await deleteExtension.Handle(req, CancellationToken.None, () => Task.FromResult(res));

            opt.CloudTable.ExecuteQuery(new TableQuery()).Should().HaveCount(0);
        }

        [Theory(DisplayName = "All TableResponseBehaviors execute correctly"), MemberData(nameof(TestData))]
        public async Task Test2<TRequest, TResponse>(TRequest req, TResponse res) where TRequest : IRequest<TResponse>
        {
            var rk = Guid.NewGuid().ToString();

            var opt = new TableOptions<TResponse>
            {
                IsEnabled = true,
                CloudTable = tableFixture.CloudTable,
                TableEntity = (req, ctx) => new DynamicTableEntity("IntegrationTests", rk) { ETag = "*" },
                Retrieved = (res, req, ctx) =>
                {
                    var dte = res.Result as DynamicTableEntity;

                    dte.Should().NotBeNull();

                    var obj = JsonConvert.DeserializeObject<TRequest>(dte.Properties["Content"].StringValue);

                    obj.Should().NotBeNull();

                    return Task.CompletedTask;
                }
            };

            var serviceProvider = new ServiceCollection()

                .AddTableExtensions<TRequest, TResponse>(sp => Options.Create(opt))
                .BuildServiceProvider();

            var insertExtension = serviceProvider.GetRequiredService<InsertEntityResponseBehavior<TRequest, TResponse>>();
            var retrieveExtension = serviceProvider.GetRequiredService<RetrieveEntityResponseBehavior<TRequest, TResponse>>();
            var deleteExtension = serviceProvider.GetRequiredService<DeleteEntityResponseBehavior<TRequest, TResponse>>();

            opt.CloudTable.ExecuteQuery(new TableQuery()).Should().HaveCount(0);

            _ = await insertExtension.Handle(req, CancellationToken.None, () => Task.FromResult(res));

            opt.CloudTable.ExecuteQuery(new TableQuery()).Should().HaveCount(1);

            _ = await retrieveExtension.Handle(req, CancellationToken.None, () => Task.FromResult(res));

            _ = await deleteExtension.Handle(req, CancellationToken.None, () => Task.FromResult(res));

            opt.CloudTable.ExecuteQuery(new TableQuery()).Should().HaveCount(0);
        }

        [Theory(DisplayName = "All TableRequestProcessors execute correctly"), MemberData(nameof(TestData))]
        public async Task Test3<TRequest, TResponse>(TRequest req, TResponse res) where TRequest : IRequest<TResponse>
        {
            var rk = Guid.NewGuid().ToString();

            var opt = new TableOptions<TRequest>
            {
                IsEnabled = true,
                CloudTable = tableFixture.CloudTable,
                TableEntity = (req, ctx) => new DynamicTableEntity("IntegrationTests", rk) { ETag = "*" },
                Retrieved = (res, req, ctx) =>
                {
                    var dte = res.Result as DynamicTableEntity;

                    dte.Should().NotBeNull();

                    var obj = JsonConvert.DeserializeObject<TRequest>(dte.Properties["Content"].StringValue);

                    obj.Should().NotBeNull();

                    return Task.CompletedTask;
                }
            };

            var serviceProvider = new ServiceCollection()

                .AddTableExtensions<TRequest, TResponse>(sp => Options.Create(opt))
                .BuildServiceProvider();

            var insertExtension = serviceProvider.GetRequiredService<InsertEntityRequestProcessor<TRequest>>();
            var retrieveExtension = serviceProvider.GetRequiredService<RetrieveEntityRequestProcessor<TRequest>>();
            var deleteExtension = serviceProvider.GetRequiredService<DeleteEntityRequestProcessor<TRequest>>();

            opt.CloudTable.ExecuteQuery(new TableQuery()).Should().HaveCount(0);

            await insertExtension.Process(req, CancellationToken.None);

            opt.CloudTable.ExecuteQuery(new TableQuery()).Should().HaveCount(1);

            await retrieveExtension.Process(req, CancellationToken.None);

            await deleteExtension.Process(req, CancellationToken.None);

            opt.CloudTable.ExecuteQuery(new TableQuery()).Should().HaveCount(0);
        }

        [Theory(DisplayName = "All TableResponseProcessors execute correctly"), MemberData(nameof(TestData))]
        public async Task Test4<TRequest, TResponse>(TRequest req, TResponse res) where TRequest : IRequest<TResponse>
        {
            var rk = Guid.NewGuid().ToString();

            var opt = new TableOptions<TResponse>
            {
                IsEnabled = true,
                CloudTable = tableFixture.CloudTable,
                TableEntity = (req, ctx) => new DynamicTableEntity("IntegrationTests", rk) { ETag = "*" },
                Retrieved = (res, req, ctx) =>
                {
                    var dte = res.Result as DynamicTableEntity;

                    dte.Should().NotBeNull();

                    var obj = JsonConvert.DeserializeObject<TRequest>(dte.Properties["Content"].StringValue);

                    obj.Should().NotBeNull();

                    return Task.CompletedTask;
                }
            };

            var serviceProvider = new ServiceCollection()

                .AddTableExtensions<TRequest, TResponse>(sp => Options.Create(opt))
                .BuildServiceProvider();

            var insertExtension = serviceProvider.GetRequiredService<InsertEntityResponseProcessor<TRequest, TResponse>>();
            var retrieveExtension = serviceProvider.GetRequiredService<RetrieveEntityResponseProcessor<TRequest, TResponse>>();
            var deleteExtension = serviceProvider.GetRequiredService<DeleteEntityResponseProcessor<TRequest, TResponse>>();

            opt.CloudTable.ExecuteQuery(new TableQuery()).Should().HaveCount(0);

            await insertExtension.Process(req, res, CancellationToken.None);

            opt.CloudTable.ExecuteQuery(new TableQuery()).Should().HaveCount(1);

            await retrieveExtension.Process(req, res, CancellationToken.None);

            await deleteExtension.Process(req, res, CancellationToken.None);

            opt.CloudTable.ExecuteQuery(new TableQuery()).Should().HaveCount(0);
        }
    }
}
