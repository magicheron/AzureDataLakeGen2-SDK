using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.Management.Storage
{
    public class Filesystem
    {
        public string ETag { get; set; }

        public string LastModified { get; set; }

        public string Name { get; set; }
    }
}
