﻿using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class DownloadBlobResponseBehavior<TRequest, TResponse> : BlobResponseBehaviorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public DownloadBlobResponseBehavior(DownloadBlobCommand<TResponse> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }
}
