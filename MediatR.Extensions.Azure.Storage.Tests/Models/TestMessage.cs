using System;

namespace MediatR.Extensions.Azure.Storage.Tests
{
    public class TestMessage
    {
        public static TestMessage Default => new TestMessage { MessageId = Guid.NewGuid().ToString() };

        public string MessageId { get; set; }
    }
}
