using FluentAssertions;
using MediatR.Extensions.Azure.Storage.Abstractions;
using Microsoft.Azure.Cosmos.Table;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Extensions.Azure.Storage.Tests.Commands.Tables
{
    public class TableCommandFixture<TMessage>
    {
        public Mock<TableOptions<TMessage>> Options { get; set; }
        public Mock<CloudTable> Table { get; set; }
        public ICommand<TMessage> Command { get; set; }

        public async Task Test1a(TMessage msg)
        {
            await Command.ExecuteAsync(msg, CancellationToken.None);

            Options.VerifyGet(m => m.IsEnabled, Times.Once);
            Options.VerifyGet(m => m.CloudTable, Times.Never);
            Options.VerifyGet(m => m.TableEntity, Times.Never);
        }

        public async Task Test1b(TMessage msg)
        {
            var src = new CancellationTokenSource(0);

            Func<Task> act = async () => await Command.ExecuteAsync(msg, src.Token);

            await act.Should().ThrowAsync<OperationCanceledException>();

            Options.VerifyGet(m => m.IsEnabled, Times.Never);
        }

        public async Task Test2(TMessage msg)
        {
            Options.SetupProperty(m => m.IsEnabled, true);

            Func<Task> act = async () => await Command.ExecuteAsync(msg, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentNullException>();

            Options.VerifyGet(m => m.IsEnabled, Times.Once);
            Options.VerifyGet(m => m.CloudTable, Times.Once);
            Options.VerifyGet(m => m.TableEntity, Times.Never);
        }
    }
}
