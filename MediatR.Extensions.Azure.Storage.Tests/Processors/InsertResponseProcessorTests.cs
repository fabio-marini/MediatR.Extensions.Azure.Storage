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
    public class InsertResponseProcessorTests
    {
        private readonly IServiceProvider svc;
        private readonly Mock<ILogger> log;
        private readonly Mock<InsertEntityCommand<Unit>> cmd;
        private readonly Mock<InsertEntityCommand<TestResult>> qry;

        public InsertResponseProcessorTests()
        {
            log = new Mock<ILogger>();
            cmd = new Mock<InsertEntityCommand<Unit>>(Options.Create(new InsertEntityOptions<Unit>()), null, null);
            qry = new Mock<InsertEntityCommand<TestResult>>(Options.Create(new InsertEntityOptions<TestResult>()), null, null);

            svc = new ServiceCollection()

                .AddTransient<InsertResponseProcessor<TestCommand, Unit>>()
                .AddTransient<InsertEntityCommand<Unit>>(sp => cmd.Object)

                .AddTransient<InsertResponseProcessor<TestQuery, TestResult>>()
                .AddTransient<InsertEntityCommand<TestResult>>(sp => qry.Object)

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
            var prc = svc.GetRequiredService<InsertResponseProcessor<TRequest, TResponse>>();

            await prc.Process(req, res, CancellationToken.None);

            var logInvocation = log.Invocations.Where(i => i.Method.Name == "Log").Single();

            logInvocation.Arguments.OfType<LogLevel>().Single().Should().Be(LogLevel.Information);
        }

        [Theory(DisplayName = "Processor handles exceptions"), MemberData(nameof(TestData))]
        public async Task Test2<TRequest, TResponse>(TRequest req, TResponse res) where TRequest : IRequest<TResponse>
        {
            cmd.Setup(m => m.ExecuteAsync(It.IsAny<Unit>(), CancellationToken.None)).ThrowsAsync(new Exception("Failed! :("));
            qry.Setup(m => m.ExecuteAsync(It.IsAny<TestResult>(), CancellationToken.None)).ThrowsAsync(new Exception("Failed! :("));

            var prc = svc.GetRequiredService<InsertResponseProcessor<TRequest, TResponse>>();

            await prc.Process(req, res, CancellationToken.None);

            var logInvocation = log.Invocations.Where(i => i.Method.Name == "Log").Single();

            logInvocation.Arguments.OfType<LogLevel>().Single().Should().Be(LogLevel.Error);
            logInvocation.Arguments.OfType<Exception>().Single().Message.Should().Be("Failed! :(");
        }
    }
}
