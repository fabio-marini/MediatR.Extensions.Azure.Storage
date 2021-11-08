﻿using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class UploadRequestProcessor<TRequest> : RequestProcessorBase<TRequest>
    {
        public UploadRequestProcessor(UploadBlobCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }
}
