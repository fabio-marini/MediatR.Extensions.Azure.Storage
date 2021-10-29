using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Extensions.Azure.Storage.Tests
{
    public class TestQuery : IRequest<TestResult>
    {
        public string Message { get; set; }
    }

    public class TestResult
    {
        public int Length { get; set; }
    }

    public class TestQueryHandler : IRequestHandler<TestQuery, TestResult>
    {
        public string Message { get; set; }

        public Task<TestResult> Handle(TestQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new TestResult { Length = request.Message.Length });
        }
    }
}
