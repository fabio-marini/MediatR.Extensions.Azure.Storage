using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Extensions.Azure.Storage.Tests
{
    public class TestCommand : IRequest
    {
        public static TestCommand Default => new TestCommand { Message = "Hello command!" };

        public string Message { get; set; }
    }

    public class TestCommandHandler : IRequestHandler<TestCommand>
    {
        public Task<Unit> Handle(TestCommand request, CancellationToken cancellationToken)
        {
            return Unit.Task;
        }
    }
}
