using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastESSInstaller
{
    class GeneralConfiguration
    {
        public string InstanceFolder { get; set; }
        public string InstanceTag { get; set; }
        public string PathToArhievedServices { get; set; }
        public PGDataBase DataBase { get; set; }
        public string RabbitMQ { get; set; }
        public string TokenIssuer { get; set; }
        public string SigningCertificateThumbprint { get; set; }
        public string IntegrationServiceEndpoint { get; set; }
        public string IntegrationServiceUser { get; set; }
        public string IntegrationServicePassword { get; set; }
        public string EssPlatformVersion { get; set; }
        public List<string> ServiceFolders { get; set; }
        public string DocumentServiceHost { get; set; }
        public string EssServiceHost { get; set; }
        public string EssSiteHost { get; set; }
        public string IdentityServiceHost { get; set; }
        public string MessagingServiceHost { get; set; }
        public string ShedulingServiceHost { get; set; }
        public string SignServiceHost { get; set; }
        public string StorageServiceHost { get; set; }
        
        
        public string DocumentServicePort { get; set; }
        public string EssServicePort { get; set; }
        public string EssSitePort { get; set; }
        public string IdentityServicePort { get; set; }
        public string MessagingServicePort { get; set; }
        public string SignServicePort { get; set; }
        public string ShedulingServicePort { get; set; }
        public string StorageServicePort { get; set; }
        
        public Durability Durability { get; set; }

    }
    public class PGDataBase
    {
        public string DBHost { get; set; }
        public string Port { get; set; }
        public string EssDatabaseName { get; set; }
        public string IdDatabaseName { get; set; }
        public string MessagesDatabaseName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class Connectionstrings
    {
        public string Database { get; set; }
        public string RabbitMQ { get; set; }
        public string IdentityService { get; set; }
        public string StorageService { get; set; }
        public string SignService { get; set; }
        public string PreviewService { get; set; }
        public string PreviewStorage { get; set; }
        public string MessagingService { get; set; }
        public string DocumentService { get; set; }
        public string OfficeService { get; set; }
        public string SessionServer { get; set; }

    }
    public class Authentication
    {
        public string TokenIssuer { get; set; }
        public string Audience { get; set; }
        public string SigningCertificateThumbprint { get; set; }
        public string ReturnUrl { get; set; }
        public object[] Providers { get; set; }

    }

    public class Logging
    {
        public Loglevel LogLevel { get; set; }
    }

    public class Loglevel
    {
        public string Default { get; set; }
        public string Microsoft { get; set; }
        public string MicrosoftHostingLifetime { get; set; }
    }

    public class Globalization
    {
        public string DefaultCulture { get; set; }
    }

    public class Diagnostics
    {
        public bool EnableRequestProfiling { get; set; }
        public string HealthCheckTimeout { get; set; }
        public bool EnableConfigLogging { get; set; }
        public bool EnableAuditLogging { get; set; }
        public FileLogOutput FileLogOutput { get; set; }
    }

    public class FileLogOutput
    {
        public string Format { get; set; }
        public string Directory { get; set; }
        public string File { get; set; }
    }

    public class Caching
    {
        public string CacheDirectory { get; set; }
        public string CacheLifetime { get; set; }
        public int InMemoryCacheSizeMB { get; set; }
    }

    public class Durability
    {
        public HttpPolicyOptions HttpPolicyOptions { get; set; }
        public HttpPolicies HttpPolicies { get; set; }
    }

    public class HttpPolicyOptions
    {
        public string[] RetryPolicyIntervals { get; set; }
    }
    public class HttpPolicies
    {
        public string[] RetryPolicyIntervals { get; set; }
    }
    public class Facilityflowprocessingservice
    {
        public int ProcessDelayMs { get; set; }
    }

    public class Cachevalidationservice
    {
        public int ProcessDelayMs { get; set; }
    }
    
}
