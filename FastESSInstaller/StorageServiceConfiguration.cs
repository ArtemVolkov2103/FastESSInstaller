using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastESSInstaller
{
    class StorageServiceConfiguration
    {
        public Logging Logging { get; set; }
        public string AllowedHosts { get; set; }
        public Diagnostics Diagnostics { get; set; }
        public StorageServiceGeneral General { get; set; }
        public Authentication Authentication { get; set; }
        public Obsoletedatacleaning ObsoleteDataCleaning { get; set; }
    }
    public class StorageServiceGeneral
    {
        public string FileStoragePath { get; set; }
        public bool EnableScaling { get; set; }
    }
    public class Obsoletedatacleaning
    {
        public object ObsoleteDataCleanInterval { get; set; }
        public object ObsoleteDataLifePeriod { get; set; }
    }
}
