using Azure.Storage.Blobs;
using FluentAssertions;
using MediatR.Extensions.Abstractions;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace MediatR.Extensions.Azure.Storage.Tests.Integration
{
    [Trait("TestCategory", "Integration")]
    [TestCaseOrderer("MediatR.Extensions.Tests.TestMethodNameOrderer", "MediatR.Extensions.Azure.Storage.Tests")]
    public class TableExtensionsTests : IClassFixture<TableFixture>
    {
        private readonly TableFixture tableFixture;
        private readonly ITestOutputHelper log;

        public TableExtensionsTests(TableFixture tableFixture, ITestOutputHelper log)
        {
            this.tableFixture = tableFixture;
            this.log = log;
        }

        [Fact(DisplayName = "01. Table is empty")]
        public void Test1() => tableFixture.GivenTableIsEmpty();

        [Fact(DisplayName = "02. Entities are inserted")]
        public async Task Test2()
        {
            var serviceProvider = new ServiceCollection()
                
                .AddMediatR(this.GetType())
                .AddTransient<CloudTable>(sp => tableFixture.CloudTable)
                .AddTransient<ITestOutputHelper>(sp => log)
                .AddTableOptions()
                .AddInsertEntityExtensions()

                .BuildServiceProvider();

            var med = serviceProvider.GetRequiredService<IMediator>();

            var res = await med.Send(EchoRequest.Default);

            res.Message.Should().Be(EchoRequest.Default.Message);
        }

        [Fact(DisplayName = "03. Table has entities")]
        public void Test3() => tableFixture.ThenTableHasEntities(4);

        [Fact(DisplayName = "04. Entities are retrieved")]
        public async Task Test4()
        {
            var serviceProvider = new ServiceCollection()

                .AddMediatR(this.GetType())
                .AddTransient<CloudTable>(sp => tableFixture.CloudTable)
                .AddTransient<ITestOutputHelper>(sp => log)
                .AddTableOptions()
                .AddRetrieveEntityExtensions()
                .AddSingleton<PipelineContext>()

                .BuildServiceProvider();

            var med = serviceProvider.GetRequiredService<IMediator>();

            var res = await med.Send(EchoRequest.Default);

            res.Message.Should().Be(EchoRequest.Default.Message);

            var ctx = serviceProvider.GetRequiredService<PipelineContext>();

            ctx.Should().HaveCount(4);
        }

        [Fact(DisplayName = "05. Entities are deleted")]
        public async Task Test5()
        {
            var serviceProvider = new ServiceCollection()

                .AddMediatR(this.GetType())
                .AddTransient<CloudTable>(sp => tableFixture.CloudTable)
                .AddTransient<ITestOutputHelper>(sp => log)
                .AddTableOptions()
                .AddDeleteEntityExtensions()

                .BuildServiceProvider();

            var med = serviceProvider.GetRequiredService<IMediator>();

            var res = await med.Send(EchoRequest.Default);

            res.Message.Should().Be(EchoRequest.Default.Message);
        }

        [Fact(DisplayName = "06. Table is empty")]
        public void Test6() => tableFixture.ThenTableIsEmpty();
    }
}
