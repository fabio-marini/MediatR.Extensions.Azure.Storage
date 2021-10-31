namespace MediatR.Extensions.Azure.Storage.Tests
{
    public class TestMessage
    {
        public static TestMessage Default => new TestMessage { Message = "Hello message!" };

        public string Message { get; set; }
    }
}
