using Azure.Storage.Blobs;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MediatR.Extensions.Azure.Storage.Tests.Extensions
{
    public class UploadBlobExtensionsTests
    {
        private readonly IServiceProvider svc;
        private readonly Mock<ILogger> log;
        private readonly Mock<PipelineContext> ctx;

        public UploadBlobExtensionsTests()
        {
            log = new Mock<ILogger>();
            ctx = new Mock<PipelineContext>();

            svc = new ServiceCollection()

                .AddUploadBlobExtensions()

                .AddTransient<PipelineContext>(sp => ctx.Object)
                .AddTransient<ILogger>(sp => log.Object)

                .BuildServiceProvider();
        }

        public static IEnumerable<object[]> TestData()
        {
            yield return new object[]
            {
                new Func<IServiceProvider, CancellationToken, Task> (async (svc, tkn) =>
                {
                    var bvr = svc.GetRequiredService<UploadRequestBehavior<TestCommand, Unit>>();

                    await bvr.Handle(TestCommand.Default, tkn, () => Unit.Task);
                }),
                new Func<IServiceProvider, Mock<UploadBlobOptions<TestCommand>>>(svc =>
                {
                    return svc.GetRequiredService<Mock<UploadBlobOptions<TestCommand>>>();
                })
            };
            yield return new object[]
            {
                new Func<IServiceProvider, CancellationToken, Task> (async (svc, tkn) =>
                {
                    var bvr = svc.GetRequiredService<UploadRequestBehavior<TestQuery, TestResult>>();

                    await bvr.Handle(TestQuery.Default, tkn, () => Task.FromResult(TestResult.Default));
                }),
                new Func<IServiceProvider, Mock<UploadBlobOptions<TestQuery>>>(svc =>
                {
                    return svc.GetRequiredService<Mock<UploadBlobOptions<TestQuery>>>();
                })
            };
            yield return new object[]
            {
                new Func<IServiceProvider, CancellationToken, Task> (async (svc, tkn) =>
                {
                    var bvr = svc.GetRequiredService<UploadRequestProcessor<TestCommand>>();

                    await bvr.Process(TestCommand.Default, tkn);
                }),
                new Func<IServiceProvider, Mock<UploadBlobOptions<TestCommand>>>(svc =>
                {
                    return svc.GetRequiredService<Mock<UploadBlobOptions<TestCommand>>>();
                })
            };
            yield return new object[]
            {
                new Func<IServiceProvider, CancellationToken, Task> (async (svc, tkn) =>
                {
                    var bvr = svc.GetRequiredService<UploadRequestProcessor<TestQuery>>();

                    await bvr.Process(TestQuery.Default, tkn);
                }),
                new Func<IServiceProvider, Mock<UploadBlobOptions<TestQuery>>>(svc =>
                {
                    return svc.GetRequiredService<Mock<UploadBlobOptions<TestQuery>>>();
                })
            };
            yield return new object[]
            {
                new Func<IServiceProvider, CancellationToken, Task> (async (svc, tkn) =>
                {
                    var bvr = svc.GetRequiredService<UploadResponseProcessor<TestCommand, Unit>>();

                    await bvr.Process(TestCommand.Default, Unit.Value, tkn);
                }),
                new Func<IServiceProvider, Mock<UploadBlobOptions<Unit>>>(svc =>
                {
                    return svc.GetRequiredService<Mock<UploadBlobOptions<Unit>>>();
                })
            };
            yield return new object[]
            {
                new Func<IServiceProvider, CancellationToken, Task> (async (svc, tkn) =>
                {
                    var bvr = svc.GetRequiredService<UploadResponseProcessor<TestQuery, TestResult>>();

                    await bvr.Process(TestQuery.Default, TestResult.Default, tkn);
                }),
                new Func<IServiceProvider, Mock<UploadBlobOptions<TestResult>>>(svc =>
                {
                    return svc.GetRequiredService<Mock<UploadBlobOptions<TestResult>>>();
                })
            };
        }

        [Theory(DisplayName = "Extension executes successfully"), MemberData(nameof(TestData))]
        public async Task Test1<TMessage>(Func<IServiceProvider, CancellationToken, Task> act,
            Func<IServiceProvider, Mock<UploadBlobOptions<TMessage>>> opt)
        {
            var blb = new Mock<BlobClient>("UseDevelopmentStorage=true", "container1", "blob1");

            opt(svc).SetupProperty(m => m.IsEnabled, true);
            opt(svc).SetupProperty(m => m.BlobClient, (req, ctx) => blb.Object);
            opt(svc).SetupProperty(m => m.BlobContent, (req, ctx) => BinaryData.FromString("Hello world"));

            await act(svc, CancellationToken.None);

            var logInvocation = log.Invocations.Where(i => i.Method.Name == "Log").Single();

            logInvocation.Arguments.OfType<LogLevel>().Single().Should().Be(LogLevel.Information);
        }

        [Theory(DisplayName = "Extension handles exceptions"), MemberData(nameof(TestData))]
        public async Task Test2<TMessage>(Func<IServiceProvider, CancellationToken, Task> act,
            Func<IServiceProvider, Mock<UploadBlobOptions<TMessage>>> opt)
        {
            opt(svc).SetupProperty(m => m.IsEnabled, true);

            await act(svc, CancellationToken.None);

            var logInvocation = log.Invocations.Where(i => i.Method.Name == "Log").Single();

            logInvocation.Arguments.OfType<LogLevel>().Single().Should().Be(LogLevel.Error);
            logInvocation.Arguments.OfType<ArgumentNullException>().Single();

            ctx.VerifyGet(m => m.Exceptions, Times.Once);
        }

        [Theory(DisplayName = "Extension handles cancellations"), MemberData(nameof(TestData))]
        public async Task Test3<TMessage>(Func<IServiceProvider, CancellationToken, Task> act,
            Func<IServiceProvider, Mock<UploadBlobOptions<TMessage>>> opt)
        {
            _ = opt(svc);
            
            var src = new CancellationTokenSource(0);

            await act(svc, src.Token);

            var logInvocation = log.Invocations.Where(i => i.Method.Name == "Log").Single();

            logInvocation.Arguments.OfType<LogLevel>().Single().Should().Be(LogLevel.Error);
            logInvocation.Arguments.OfType<OperationCanceledException>().Single();

            ctx.VerifyGet(m => m.Exceptions, Times.Once);
        }
    }
}
