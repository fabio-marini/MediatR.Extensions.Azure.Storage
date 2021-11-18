﻿using MediatR.Extensions.Abstractions;
using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class SendMessageRequestProcessor<TRequest> : RequestProcessorBase<TRequest>
    {
        public SendMessageRequestProcessor(SendMessageCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }
}
