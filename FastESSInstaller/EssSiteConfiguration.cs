using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastESSInstaller
{
    class EssSiteConfiguration
    {
        public Logging Logging { get; set; }
        public string AllowedHosts { get; set; }
        public Connectionstrings ConnectionStrings { get; set; }
        public General General { get; set; }
        public Globalization Globalization { get; set; }
        public Authentication Authentication { get; set; }
        public object[] SupportContacts { get; set; }
        public Appearance Appearance { get; set; }
        public Clientconfiguration ClientConfiguration { get; set; }
        public Diagnostics Diagnostics { get; set; }
        public SignPlugin[] SignPlugins { get; set; }
        public Reverseproxy ReverseProxy { get; set; }
    }
    public class General
    {
        public bool EnableDemoMode { get; set; }
        public string CompressionLevel { get; set; }
        public string ServiceEndpoint { get; set; }
        public string UserPhotoDirectory { get; set; }
        public bool EmbeddedMode { get; set; }
        public bool UseShortPaths { get; set; }
        public string ShortPathExpirationPeriod { get; set; }
        public string PrimaryTenant { get; set; }
        
    }
    public class Appearance
    {
        public string SiteTitle { get; set; }
        public string FaviconUrl { get; set; }
        public string LogoUrl { get; set; }
        public string SplashscreenUrl { get; set; }
        public Light Light { get; set; }
    }
    public class Light
    {
        public string Primary50Color { get; set; }
        public string Secondary50Color { get; set; }
        public string Info50Color { get; set; }
        public string Success50Color { get; set; }
        public string Warning50Color { get; set; }
        public string Danger50Color { get; set; }
        public string Surface50Color { get; set; }
        public string SurfaceContainer50Color { get; set; }
        public string TextPrimaryColor { get; set; }
        public string TextSecondary50Color { get; set; }
        public string ShadowColor { get; set; }
        public string ScrollColor { get; set; }
        public string FocusVisibleColor { get; set; }
    }
    public class Clientconfiguration
    {
        public string SessionLifetime { get; set; }
        public string ServiceEndpoint { get; set; }
        public string ServiceAudience { get; set; }
        public string NotificationsUpdateInterval { get; set; }
        public string WidgetUpdateInterval { get; set; }
        public string RequestTimeout { get; set; }
        public string SignOutEndpoint { get; set; }
        public string Audience { get; set; }
        public string ReturnUrl { get; set; }
        public string ReplyUrl { get; set; }
        public string ConfirmationEndpoint { get; set; }
        public string ChangePasswordEndpoint { get; set; }
        public string RefreshCodeInterval { get; set; }
        public string ToastAutoHideTimeout { get; set; }
        public string MaxNumberOfAttempts { get; set; }
        public string PreviewProxyPrefix { get; set; }
        public string StorageProxyPrefix { get; set; }
        public string AutoSignOutDelaySecs { get; set; }
        public string SessionIdleTimeoutSecs { get; set; }
        public string MobileAutoSignOutDelaySecs { get; set; }
        public string MobileSessionIdleTimeoutSecs { get; set; }
        public string AvailableLanguages { get; set; }
        public string SupportContacts { get; set; }
        public bool AllowDocumentDownload { get; set; }
        public bool EnableNotifications { get; set; }
        public bool ShowHistoryButton { get; set; }
        public bool ShowProfileButton { get; set; }
        public string Appearance { get; set; }
    }
    public class SignPlugin
    {
        public string Id { get; set; }
        public string ProviderId { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public string ClassName { get; set; }
        public Instruction Instruction { get; set; }
        public Link Link { get; set; }
    }

    public class Instruction
    {
        public string ru { get; set; }
        public string en { get; set; }
    }
    public class Link
    {
        public string Url { get; set; }
        public string Title { get; set; }
    }
    public class Reverseproxy
    {
        public bool Enabled { get; set; }
        public Routes Routes { get; set; }
        public Clusters Clusters { get; set; }
    }
    public class Routes
    {
        public ServiceRoute StorageService { get; set; }
        public ServiceRoute PreviewStorage { get; set; }
        public ServiceRoute OfficeService { get; set; }
    }
    public class ServiceRoute
    {
        public string ClusterId { get; set; }
        public Match Match { get; set; }
        public Transform[] Transforms { get; set; }
    }
    public class Match
    {
        public string Path { get; set; }
    }
    public class Transform
    {
        public string PathRemovePrefix { get; set; }
        public string RequestHeaderRemove { get; set; }
    }
    public class Clusters
    {
        public ServiceCluster StorageService { get; set; }
        public ServiceCluster PreviewStorage { get; set; }
        public ServiceCluster OfficeService { get; set; }
    }
    public class ServiceCluster
    {
        public Destinations Destinations { get; set; }
    }
    public class Destinations
    {
        public DestinationService StorageService { get; set; }
        public DestinationService PreviewStorage { get; set; }
        public DestinationService OfficeService { get; set; }
    }
    public class DestinationService
    {
        public string Address { get; set; }
    }
}
