using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Extensions.Azure.Storage.Tests
{
    public class TestCommand : IRequest
    {
        public string Message { get; set; }
    }

    public class TestCommandHandler : IRequestHandler<TestCommand>
    {
        public string Message { get; set; }

        public Task<Unit> Handle(TestCommand request, CancellationToken cancellationToken)
        {
            return Unit.Task;
        }
    }
}
