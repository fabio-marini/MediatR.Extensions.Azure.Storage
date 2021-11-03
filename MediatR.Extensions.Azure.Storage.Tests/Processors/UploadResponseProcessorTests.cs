using Azure.Storage.Blobs;
using FluentAssertions;
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
    public class UploadResponseProcessorTests
    {
        private readonly IServiceProvider svc;
        private readonly Mock<ILogger> log;
        private readonly Mock<UploadBlobOptions<Unit>> cmd;
        private readonly Mock<UploadBlobOptions<TestResult>> qry;

        public UploadResponseProcessorTests()
        {
            log = new Mock<ILogger>();
            cmd = new Mock<UploadBlobOptions<Unit>>();
            qry = new Mock<UploadBlobOptions<TestResult>>();

            svc = new ServiceCollection()

                .AddTransient<UploadResponseProcessor<TestCommand, Unit>>()
                .AddTransient<IOptions<UploadBlobOptions<Unit>>>(sp => Options.Create(cmd.Object))

                .AddTransient<UploadResponseProcessor<TestQuery, TestResult>>()
                .AddTransient<IOptions<UploadBlobOptions<TestResult>>>(sp => Options.Create(qry.Object))

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
            var blb = new Mock<BlobClient>("UseDevelopmentStorage=true", "container1", "blob1");

            cmd.SetupProperty(m => m.IsEnabled, true);
            qry.SetupProperty(m => m.IsEnabled, true);

            cmd.SetupProperty(m => m.BlobClient, (req, ctx) => blb.Object);
            qry.SetupProperty(m => m.BlobClient, (req, ctx) => blb.Object);

            cmd.SetupProperty(m => m.BlobContent, (req, ctx) => BinaryData.FromString("Hello world"));
            qry.SetupProperty(m => m.BlobContent, (req, ctx) => BinaryData.FromString("Hello world"));

            var prc = svc.GetRequiredService<UploadResponseProcessor<TRequest, TResponse>>();

            await prc.Process(req, res, CancellationToken.None);

            var logInvocation = log.Invocations.Where(i => i.Method.Name == "Log").Single();

            logInvocation.Arguments.OfType<LogLevel>().Single().Should().Be(LogLevel.Information);
        }

        [Theory(DisplayName = "Processor handles exceptions"), MemberData(nameof(TestData))]
        public async Task Test2<TRequest, TResponse>(TRequest req, TResponse res) where TRequest : IRequest<TResponse>
        {
            cmd.SetupProperty(m => m.IsEnabled, true);
            qry.SetupProperty(m => m.IsEnabled, true);

            var prc = svc.GetRequiredService<UploadResponseProcessor<TRequest, TResponse>>();

            await prc.Process(req, res, CancellationToken.None);

            var logInvocation = log.Invocations.Where(i => i.Method.Name == "Log").Single();

            logInvocation.Arguments.OfType<LogLevel>().Single().Should().Be(LogLevel.Error);
            logInvocation.Arguments.OfType<ArgumentNullException>().Single();
        }
    }
}
