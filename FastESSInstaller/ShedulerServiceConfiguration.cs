using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastESSInstaller
{
    class ShedulerServiceConfiguration
    {
        public Logging Logging { get; set; }
        public Connectionstrings ConnectionStrings { get; set; }
        public Secrets Secrets { get; set; }
        public Diagnostics Diagnostics { get; set; }
        public Transport Transport { get; set; }
        public string[] RetryPolicyIntervals { get; set; }
        public Messageprocessingservice MessageProcessingService { get; set; }
    }
    public class Secrets
    {
        public Database Database { get; set; }
    }

    public class Database
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class Transport
    {
        public string SmsDeliveryProxy { get; set; }
        public Proxy[] Proxies { get; set; }
    }

    public class Proxy
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public Configuration Configuration { get; set; }
    }
    public class Configuration
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool UseSsl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Sender { get; set; }
        public float[] LimitProportionByPriority { get; set; }
        public int MaxTransmitRetryCount { get; set; }
        public bool UseTinyUrl { get; set; }
        public int MessagesPerSecond { get; set; }
        public string NotificationTitleTemplate { get; set; }
        public string ApplicationID { get; set; }
        public string ServiceName { get; set; }
        public string FromAddress { get; set; }
        public string FromContact { get; set; }
        public string ApiKey { get; set; }
        public string Path { get; set; }
    }

    public class Messageprocessingservice
    {
        public int ProcessDelayMs { get; set; }
        public bool TryPushBeforeSms { get; set; }
    }
}

