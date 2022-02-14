using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.Management.Storage
{
    public class Path
    {
        public int ContentLength { get; set; }

        public string ETag { get; set; }

        public string Group { get; set; }

        public bool IsDirectory { get; set; }

        public string LastModified { get; set; }

        public string Name { get; set; }

        public string Owner { get; set; }

        public string Permissions { get; set; }
    }
}
