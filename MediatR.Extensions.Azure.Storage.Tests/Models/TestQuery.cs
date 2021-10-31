using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Extensions.Azure.Storage.Tests
{
    public class TestQuery : IRequest<TestResult>
    {
        public static TestQuery Default => new TestQuery { Message = "Hello query!" };

        public string Message { get; set; }
    }

    public class TestResult
    {
        public static TestResult Default => new TestResult { Length = 4 };

        public int Length { get; set; }
    }

    public class TestQueryHandler : IRequestHandler<TestQuery, TestResult>
    {
        public Task<TestResult> Handle(TestQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new TestResult { Length = request.Message.Length });
        }
    }
}
