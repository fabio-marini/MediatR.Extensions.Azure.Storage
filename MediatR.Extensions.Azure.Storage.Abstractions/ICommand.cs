using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Extensions.Azure.Storage.Abstractions
{
    public interface ICommand<TMessage>
    {
        Task ExecuteAsync(TMessage message, CancellationToken cancellationToken);
    }

    //public interface ICommand2<TRequest> where TRequest : IRequest<Unit>
    //{
    //    Task ExecuteAsync(TRequest message, CancellationToken cancellationToken);
    //}

    //public interface ICommand2<TRequest, TResponse> where TRequest : IRequest<TResponse>
    //{
    //    Task ExecuteAsync(TResponse message, CancellationToken cancellationToken);
    //}

    //public interface ICommandOptions<TCommand, TMessage> where TCommand : ICommand<TMessage>
    //{
    //    bool IsEnabled { get; }
    //}
}
