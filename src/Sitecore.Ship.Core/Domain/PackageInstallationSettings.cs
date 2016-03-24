using System.Collections.Generic;

namespace Sitecore.Ship.Core.Domain
{
    public class PackageInstallationSettings
    {
        public PackageInstallationSettings()
        {
            IsEnabled = false;
            AllowRemoteAccess = false;
            AllowPackageStreaming = false;
            RecordInstallationHistory = false;
            AddressWhitelist = new List<string>();
            MuteAuthorisationFailureLogging = false;
            AuthToken = string.Empty;
            AuthHeader = string.Empty;
        }

        public bool IsEnabled { get; set; }
        public bool AllowRemoteAccess { get; set; }
        public bool AllowPackageStreaming { get; set; }
        public bool RecordInstallationHistory { get; set; }
        public List<string> AddressWhitelist { get; set; }
        public bool MuteAuthorisationFailureLogging { get; set; }
        public string AuthHeader { get; set; }
        public string AuthToken { get; set; }

        public bool HasAddressWhitelist { get { return AddressWhitelist.Count > 0; } }
    }
}