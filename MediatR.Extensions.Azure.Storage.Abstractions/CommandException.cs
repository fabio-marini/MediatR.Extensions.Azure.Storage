using System;

namespace MediatR.Extensions.Azure.Storage.Abstractions
{
    public class CommandException<TMessage> : CommandException
    {
        public CommandException(string message, TMessage value) : base(message)
        {
            Value = value;
        }

        public CommandException(string message, TMessage value, Exception innerException) : base(message, innerException)
        {
            Value = value;
        }

        public TMessage Value { get; }
    }

    public class CommandException : Exception
    {
        public CommandException(string message) : base(message)
        {
        }

        public CommandException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
