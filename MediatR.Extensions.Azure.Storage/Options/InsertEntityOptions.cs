using Microsoft.Azure.Cosmos.Table;
using System;

namespace MediatR.Extensions.Azure.Storage
{
    /// <summary>
    /// The options used to configure the table storage extensions 
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public class InsertEntityOptions<TMessage>
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
    }
}
