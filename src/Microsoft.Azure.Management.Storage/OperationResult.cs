using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.Management.Storage
{
    public class OperationResult
    {

        public bool IsSuccessStatusCode { get; set; }

        public string StatusMessage { get; set; }

    }
}
