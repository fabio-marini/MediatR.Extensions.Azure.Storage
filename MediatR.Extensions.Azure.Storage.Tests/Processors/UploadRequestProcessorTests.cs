﻿using FluentAssertions;
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
    public class UploadRequestProcessorTests
    {
        private readonly IServiceProvider svc;
        private readonly Mock<ILogger> log;
        private readonly Mock<UploadBlobCommand<TestCommand>> cmd;
        private readonly Mock<UploadBlobCommand<TestQuery>> qry;

        public UploadRequestProcessorTests()
        {
            log = new Mock<ILogger>();
            cmd = new Mock<UploadBlobCommand<TestCommand>>(Options.Create(new UploadBlobOptions<TestCommand>()), null, null);
            qry = new Mock<UploadBlobCommand<TestQuery>>(Options.Create(new UploadBlobOptions<TestQuery>()), null, null);

            svc = new ServiceCollection()

                .AddTransient<UploadRequestProcessor<TestCommand>>()
                .AddTransient<UploadBlobCommand<TestCommand>>(sp => cmd.Object)

                .AddTransient<UploadRequestProcessor<TestQuery>>()
                .AddTransient<UploadBlobCommand<TestQuery>>(sp => qry.Object)

                .AddTransient<ILogger>(sp => log.Object)

                .BuildServiceProvider();
        }

        public static IEnumerable<object[]> TestData()
        {
            yield return new object[] { TestCommand.Default, new Func<Task<Unit>>(() => Unit.Task) };
            yield return new object[] { TestQuery.Default, new Func<Task<TestResult>>(() => Task.FromResult(TestResult.Default)) };
        }

        [Theory(DisplayName = "Processor executes successfully"), MemberData(nameof(TestData))]
        public async Task Test1<TRequest, TResponse>(TRequest req, Func<Task<TResponse>> res) where TRequest : IRequest<TResponse>
        {
            var prc = svc.GetRequiredService<UploadRequestProcessor<TRequest>>();

            await prc.Process(req, CancellationToken.None);

            var logInvocation = log.Invocations.Where(i => i.Method.Name == "Log").Single();

            logInvocation.Arguments.OfType<LogLevel>().Single().Should().Be(LogLevel.Information);
        }

        [Theory(DisplayName = "Processor handles exceptions"), MemberData(nameof(TestData))]
        public async Task Test2<TRequest, TResponse>(TRequest req, Func<Task<TResponse>> res) where TRequest : IRequest<TResponse>
        {
            cmd.Setup(m => m.ExecuteAsync(It.IsAny<TestCommand>(), CancellationToken.None)).ThrowsAsync(new Exception("Failed! :("));
            qry.Setup(m => m.ExecuteAsync(It.IsAny<TestQuery>(), CancellationToken.None)).ThrowsAsync(new Exception("Failed! :("));

            var prc = svc.GetRequiredService<UploadRequestProcessor<TRequest>>();

            await prc.Process(req, CancellationToken.None);

            var logInvocation = log.Invocations.Where(i => i.Method.Name == "Log").Single();

            logInvocation.Arguments.OfType<LogLevel>().Single().Should().Be(LogLevel.Error);
            logInvocation.Arguments.OfType<Exception>().Single().Message.Should().Be("Failed! :(");
        }
    }
}
