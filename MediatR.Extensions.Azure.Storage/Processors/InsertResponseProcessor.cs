﻿using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Extensions.Azure.Storage
{
    public class InsertResponseProcessor<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly InsertEntityCommand<TResponse> cmd;
        private readonly ILogger log;

        public InsertResponseProcessor(InsertEntityCommand<TResponse> cmd, ILogger log = null)
        {
            this.cmd = cmd ?? throw new ArgumentException($"A valid {nameof(InsertEntityCommand<TResponse>)} is required");
            this.log = log ?? NullLogger.Instance;
        }

        public async Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
        {
            try
            {
                await cmd.ExecuteAsync(response, cancellationToken);

                log.LogInformation("Processor {Processor} completed, returning", this.GetType().Name);
            }
            catch (Exception ex)
            {
                // failure should not stop execution - log exception, but don't rethrow
                log.LogError(ex, "Processor {Processor} failed, returning", this.GetType().Name);
            }
        }
    }
}
