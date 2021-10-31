﻿using MediatR.Extensions.Azure.Storage;
using Microsoft.Extensions.Logging;

namespace ClassLibrary1
{
    public class RetrieveCustomerQueryPostProcessor : InsertResponseProcessor<RetrieveCustomerQuery, RetrieveCustomerResult>
    {
        public RetrieveCustomerQueryPostProcessor(InsertEntityCommand<RetrieveCustomerResult> cmd, ILogger log = null) : base(cmd, log)
        {
        }
    }
}
