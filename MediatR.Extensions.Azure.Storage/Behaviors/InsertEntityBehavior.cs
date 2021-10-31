using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Extensions.Azure.Storage
{
    public class InsertEntityBehavior<TRequest> : InsertEntityBehavior<TRequest, Unit> where TRequest : IRequest<Unit>
    {
        public InsertEntityBehavior(IOptions<InsertEntityOptions<TRequest>> opt, PipelineContext ctx, ILogger log = null)
            : base(opt, ctx, log)
        {
        }
    }

    public class InsertEntityBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IOptions<InsertEntityOptions<TRequest>> opt;
        private readonly PipelineContext ctx;
        private readonly ILogger log;

        public InsertEntityBehavior(IOptions<InsertEntityOptions<TRequest>> opt, PipelineContext ctx, ILogger log = null)
        {
            // this parameter is required: if an instance is not supplied, it will be created using the default ctor
            // (which will set IsEnabled = false) - no additional validation is required...
            this.opt = opt;
            this.ctx = ctx;
            this.log = log ?? NullLogger.Instance;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (opt.Value.IsEnabled == false)
            {
                // behavior is disabled - skip
                log.LogDebug("Behavior {Behavior} is not enabled, invoking next behavior in the chain", this.GetType().Name);

                return await next();
            }

            if (opt.Value.CloudTable == null)
            {
                // no table configured - skip
                log.LogError("Behavior {Behavior} requires a valid CloudTable", this.GetType().Name);

                return await next();
            }

            if (opt.Value.TableEntity == null)
            {
                // behavior is enabled, but no TableEntity func specified - use default
                log.LogDebug("Behavior {Behavior} is using the default TableEntity delegate", this.GetType().Name);

                opt.Value.TableEntity = (req, ctx) =>
                {
                    var pk = Guid.NewGuid().ToString();
                    var rk = Guid.NewGuid().ToString();

                    var tableEntity = new DynamicTableEntity(pk, rk);

                    tableEntity.Properties.Add("Request", EntityProperty.GeneratePropertyForString(request.GetType().FullName));
                    tableEntity.Properties.Add("Content", EntityProperty.GeneratePropertyForString(JsonConvert.SerializeObject(req)));

                    return tableEntity;
                };
            }

            try
            {
                var tableEntity = opt.Value.TableEntity(request, ctx);

                var insertOperation = TableOperation.Insert(tableEntity);

                await opt.Value.CloudTable.ExecuteAsync(insertOperation, cancellationToken);

                log.LogInformation("Behavior {Behavior} completed, invoking next behavior in the chain", this.GetType().Name);
            }
            catch (Exception ex)
            {
                // failure should not stop execution - log exception, but don't rethrow
                log.LogError(ex, "Behavior {Behavior} failed, invoking next behavior in the chain", this.GetType().Name);
            }

            return await next();
        }
    }
}
