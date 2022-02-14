using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.Management.Storage
{
    public class PayloadOperationResult<TPayload> : OperationResult
    {
        public TPayload Payload { get; set; }
    }
}
