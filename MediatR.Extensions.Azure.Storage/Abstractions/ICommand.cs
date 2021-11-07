using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Extensions.Azure.Storage
{
    public interface ICommand<TMessage>
    {
        Task ExecuteAsync(TMessage message, CancellationToken cancellationToken);
    }
}
