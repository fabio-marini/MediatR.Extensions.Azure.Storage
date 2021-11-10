using Microsoft.Azure.Cosmos.Table;
using System;
using System.Threading.Tasks;

namespace MediatR.Extensions.Azure.Storage
{
    /// <summary>
    /// The options used to configure the table storage extensions 
    /// </summary>
    /// <typeparam name="TMessage">The type of message being stored. While this is not constrained, in practice it will be a MediatR request or response.</typeparam>
    public class TableOptions<TMessage>
    {
        /// <summary>
        /// true to enable execution of the extension, false to disable it
        /// </summary>
        public virtual bool IsEnabled { get; set;  }

        /// <summary>
        /// The table where the request/response will be stored
        /// </summary>
        public virtual CloudTable CloudTable { get; set; }

        /// <summary>
        /// The delegate used to transform the request/response to a TableEntity
        /// </summary>
        public virtual Func<TMessage, PipelineContext, ITableEntity> TableEntity { get; set; }

        // the event that is raised after the message is received (allows using the received entity to modify TMessage)
        public virtual Func<TableResult, PipelineContext, TMessage, Task> Received { get; set; }
    }
}
