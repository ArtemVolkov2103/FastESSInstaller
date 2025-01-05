using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastESSInstaller
{
    class SignServiceConfiguration
    {
        public Logging Logging { get; set; }
        public string AllowedHosts { get; set; }
        public Connectionstrings ConnectionStrings { get; set; }
        public SignServiceAuthentication Authentication { get; set; }
        public Secrets Secrets { get; set; }
        public List<CloudSignProvider> Providers { get; set; }
        public IdentityDiagnostics Diagnostics { get; set; }
        public Tracing Tracing { get; set; }
    }
    public class SignServiceAuthentication
    {
        
        public string Audience { get; set; }
        public Issuers[] TrustedIssuers { get; set; }
        
    }
    public class Issuers
    {
        public string Issuer { get; set; }
        public string EncryptionKey { get; set; }
        public string SigningCertificateThumbprint { get; set; }
        public string SigningCertificatePath { get; set; }
    }
    public class CloudSignProvider
    {
        public string CloudSignService { get; set; }
        public string CloudAuthService { get; set; }
        public string Name { get; set; }
        public string ProviderId { get; set; }
        public string ProviderType { get; set; }
        public string PluginType { get; set; }
        public List<string> IdentificationTypes { get; set; }
        public CloudSignProvider()
        {
            IdentificationTypes = new List<string>();
        }
        
        public string ClientId { get; set; }
        public string SignServiceName { get; set; }
        public string OperatorLogin { get; set; }
        public string OperatorPassword { get; set; }
    }
    public class IdentityDiagnostics
    {
        public bool EnableAuditLogging { get; set; }
        public string LogOutputs { get; set; }
        public FileLogOutput FileLogOutput { get; set; }
        public ElasticSearchLogOutput ElasticSearchLogOutput { get; set; }
    }
    public class ElasticSearchLogOutput
    {
        public string ServiceAddress { get; set; }
        public string Index { get; set; }
        public string ApiKeyId { get; set; }
        public string ApiKey { get; set; }

    }
}
