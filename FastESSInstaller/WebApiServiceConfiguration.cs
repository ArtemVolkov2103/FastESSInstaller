using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastESSInstaller
{
    class WebApiServiceConfiguration
    {
        public Logging Logging { get; set; }
        public string AllowedHosts { get; set; }
        public Diagnostics Diagnostics { get; set; }
        public Connectionstrings ConnectionStrings { get; set; }
        public Secrets Secrets { get; set; }
        public Authentication Authentication { get; set; }
        public Messaging Messaging { get; set; }
        public Scheduler Scheduler { get; set; }
    }
    public class Messaging
    {
        public string DefaultMessageExpirationPeriod { get; set; }
    }

    public class Scheduler
    {
        public string HealthCheckUrl { get; set; }
    }
}
