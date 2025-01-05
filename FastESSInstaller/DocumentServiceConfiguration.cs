using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastESSInstaller
{
    class DocumentServiceConfiguration
    {
        
        public Logging Logging { get; set; }
        public string AllowedHosts { get; set; }
        public Connectionstrings ConnectionStrings { get; set; }
        public Globalization Globalization { get; set; }
        public Diagnostics Diagnostics { get; set; }
        public Authentication Authentication { get; set; }
        public Caching Caching { get; set; }
        public Durability Durability { get; set; }
    }
}
