using System;
using System.Collections.Generic;

namespace MediatR.Extensions.Azure.Storage
{
    public class PipelineContext : Dictionary<string, object>
    {
        // https://jimmybogard.com/sharing-context-in-mediatr-pipelines/
        public PipelineContext()
        {
            PipelineId = Guid.NewGuid().ToString();
            Exceptions = new List<Exception>();
        }

        public virtual string PipelineId { get; }
        public virtual List<Exception> Exceptions { get; }
    }
}
