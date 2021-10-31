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

namespace MediatR.Extensions.Azure.Storage.Tests
{
    public class InsertRequestBehaviorTests
    {
        private readonly IServiceProvider svc;
        private readonly Mock<ILogger> log;
        private readonly Mock<InsertEntityCommand<TestCommand>> cmd;
        private readonly Mock<InsertEntityCommand<TestQuery>> qry;

        public InsertRequestBehaviorTests()
        {
            log = new Mock<ILogger>();
            cmd = new Mock<InsertEntityCommand<TestCommand>>(Options.Create(new InsertEntityOptions<TestCommand>()), null, null);
            qry = new Mock<InsertEntityCommand<TestQuery>>(Options.Create(new InsertEntityOptions<TestQuery>()), null, null);

            svc = new ServiceCollection()

                .AddTransient<InsertRequestBehavior<TestCommand, Unit>>()
                .AddTransient<InsertEntityCommand<TestCommand>>(sp => cmd.Object)

                .AddTransient<InsertRequestBehavior<TestQuery, TestResult>>()
                .AddTransient<InsertEntityCommand<TestQuery>>(sp => qry.Object)

                .AddTransient<ILogger>(sp => log.Object)

                .BuildServiceProvider();
        }

        public static IEnumerable<object[]> TestData()
        {
            yield return new object[] { TestCommand.Default, new Func<Task<Unit>>(() => Unit.Task) };
            yield return new object[] { TestQuery.Default, new Func<Task<TestResult>>(() => Task.FromResult(TestResult.Default)) };
        }

        [Theory(DisplayName = "Behavior executes successfully"), MemberData(nameof(TestData))]
        public async Task Test1<TRequest, TResponse>(TRequest req, Func<Task<TResponse>> res) where TRequest : IRequest<TResponse>
        {
            var bvr = svc.GetRequiredService<InsertRequestBehavior<TRequest, TResponse>>();

            await bvr.Handle(req, CancellationToken.None, () => res());

            var logInvocation = log.Invocations.Where(i => i.Method.Name == "Log").Single();

            logInvocation.Arguments.OfType<LogLevel>().Single().Should().Be(LogLevel.Information);
        }

        [Theory(DisplayName = "Behavior handles exceptions"), MemberData(nameof(TestData))]
        public async Task Test2<TRequest, TResponse>(TRequest req, Func<Task<TResponse>> res) where TRequest : IRequest<TResponse>
        {
            cmd.Setup(m => m.ExecuteAsync(It.IsAny<TestCommand>(), CancellationToken.None)).ThrowsAsync(new Exception("Failed! :("));
            qry.Setup(m => m.ExecuteAsync(It.IsAny<TestQuery>(), CancellationToken.None)).ThrowsAsync(new Exception("Failed! :("));

            var bvr = svc.GetRequiredService<InsertRequestBehavior<TRequest, TResponse>>();

            await bvr.Handle(req, CancellationToken.None, () => res());

            var logInvocation = log.Invocations.Where(i => i.Method.Name == "Log").Single();

            logInvocation.Arguments.OfType<LogLevel>().Single().Should().Be(LogLevel.Error);
            logInvocation.Arguments.OfType<Exception>().Single().Message.Should().Be("Failed! :(");
        }
    }
}
