using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastESSInstaller
{
    class IdentityServiceConfiguration
    {
        public Logging Logging { get; set; }
        public Tracing Tracing { get; set; }
        public Connectionstrings ConnectionStrings { get; set; }
        public General General { get; set; }
        public UserDevices UserDevices { get; set; }
        public Diagnostics Diagnostics { get; set; }
        public Cachevalidationservice CacheValidationService { get; set; }
        public Durability Durability { get; set; }
        public Globalization Globalization { get; set; }
        public Accountmanagement AccountManagement { get; set; }
        public Tokenissuer TokenIssuer { get; set; }
        public object[] TrustedIssuers { get; set; }
        public Useraccounts UserAccounts { get; set; }
        public Passwords Passwords { get; set; }
        public Authentication Authentication { get; set; }
        public Uservalidation UserValidation { get; set; }
        public Accountenrichment AccountEnrichment { get; set; }
    }
    public class UserDevices
    {
        public string DeviceFingerprintSalt { get; set; }
    }
    public class Tracing
    {
        public bool Enabled { get; set; }
        public string ServiceName { get; set; }
        public string AgentHost { get; set; }
        public int AgentPort { get; set; }
    }
    public class Accountmanagement
    {
        public bool AllowAccountRegister { get; set; }
        public bool AllowPhoneNumberUpdates { get; set; }
        public bool AllowEmailAddressUpdates { get; set; }
        public bool AllowUserPhotoUpdates { get; set; }
        public bool AllowManualLoginBinding { get; set; }
    }

    public class Tokenissuer
    {
        public string Issuer { get; set; }
        public string SigningCertificateThumbprint { get; set; }
        public int LifetimeMins { get; set; }
    }

    public class Useraccounts
    {
        public string DefaultDomain { get; set; }
        public string DefaultMessagingProvider { get; set; }
        public bool LockoutEnabled { get; set; }
        public string LockoutInterval { get; set; }
        public int MaxFailedAccessAttempts { get; set; }
        public bool UsePersistentCookie { get; set; }
        public bool AllowCyrillicUserNames { get; set; }
        public string PasswordMaxAge { get; set; }
        public bool UseTwoFactorAuthentication { get; set; }
        public string DefaultTwoFactorTokenProvider { get; set; }
        public int SecurityCodeTimeStep { get; set; }
        public int SecurityCodeValidTime { get; set; }
        public int SecurityCodeDigitsNumber { get; set; }
        public string PasswordExpirationPrompt { get; set; }
        public string[] IdentityCredentials { get; set; }
        public bool PreventPasswordReuse { get; set; }
        public int NumberOfPasswordsToRemember { get; set; }
        public int MaxCodeSendingAttempts { get; set; }
        public string CodeSendingLockoutInterval { get; set; }
        public string CodeResendingInterval { get; set; }
    }

    public class Passwords
    {
        public int RequiredLength { get; set; }
        public int RequiredUniqueChars { get; set; }
        public bool RequireNonAlphanumeric { get; set; }
        public bool RequireLowercase { get; set; }
        public bool RequireUppercase { get; set; }
        public bool RequireDigit { get; set; }
    }

    public class Uservalidation
    {
        public object[] Validators { get; set; }
    }

    public class Accountenrichment
    {
        public Provider[] Providers { get; set; }
    }
    public class Provider
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string[] Audiences { get; set; }
        public string ClaimsPrefix { get; set; }
        public IdentityConfiguration Configuration { get; set; }
    }

    public class IdentityConfiguration
    {
        public string IntegrationServiceEndpoint { get; set; }
        public string ServiceUsername { get; set; }
        public string ServiceUserPassword { get; set; }
        public string EssPlatformVersion { get; set; }
    }
}
