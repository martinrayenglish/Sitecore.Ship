using System;
using System.Linq;
using System.Net;

using Sitecore.Ship.Core.Contracts;
using Sitecore.Ship.Core.Domain;

namespace Sitecore.Ship.Core
{
    public class HttpRequestAuthoriser : IAuthoriser
    {
        private readonly ICheckRequests _checkRequests;
        private readonly PackageInstallationSettings _packageInstallationSettings;
        private readonly ILog _logger;

        public HttpRequestAuthoriser(ICheckRequests checkRequests, PackageInstallationSettings packageInstallationSettings, ILog logger)
        {
            _checkRequests = checkRequests;
            _packageInstallationSettings = packageInstallationSettings;
            _logger = logger;
        }

        public bool IsAllowed()
        {
            if (!_packageInstallationSettings.IsEnabled)
            {
                LogAccessDenial("packageInstallation 'enabled' setting is false");
                return false;
            }

            if ((!_checkRequests.IsLocal) && (!_packageInstallationSettings.AllowRemoteAccess))
            {
                LogAccessDenial("packageInstallation 'allowRemote' setting is false");
                return false;
            }

            if (!_checkRequests.AuthToken(_packageInstallationSettings.AuthHeader).Equals(_packageInstallationSettings.AuthToken))
            {
                LogAccessDenial("packageInstallation 'authtoken' doesn't match configuration");
                return false;
            }

            if (_packageInstallationSettings.HasAddressWhitelist)
            {
                var foundAddress = false;
                var ipList = _packageInstallationSettings.AddressWhitelist.Where(address => !address.Contains("-")).ToList();
                var ipRangeList = _packageInstallationSettings.AddressWhitelist.Where(address => address.Contains("-")).ToList();

                foreach (var ipsInRange in ipRangeList.Select(ipRange => ipRange.Split('-')))
                {
                    foundAddress = IsInRange(IPAddress.Parse(_checkRequests.UserHostAddress), IPAddress.Parse(ipsInRange[0]), IPAddress.Parse(ipsInRange[1]));

                    if (foundAddress)
                    {
                        break;
                    }
                }

                if (!foundAddress)
                {
                    foundAddress = ipList.Any(x => string.Compare(x, _checkRequests.UserHostAddress, StringComparison.InvariantCultureIgnoreCase) == 0);
                }

                if (foundAddress)
                {
                    _logger.Write(string.Format("packageInstallation whitelist ip match found for {0} ", _checkRequests.UserHostAddress));
                    return true;
                }

                LogAccessDenial(string.Format("packageInstallation whitelist is denying access to {0}", _checkRequests.UserHostAddress));

                return false;
            }

            return true;
        }

        private void LogAccessDenial(string diagnostic)
        {
            if (!_packageInstallationSettings.MuteAuthorisationFailureLogging)
            {
                _logger.Write(string.Format("Sitecore.Ship access denied: {0}", diagnostic));
            }
        }
        public static bool IsInRange(IPAddress source, IPAddress start, IPAddress end)
        {
            if (IPAddress.IsLoopback(source) == false)
            {
                return (IpInUint(source.ToString().Split('.')) >= IpInUint(start.ToString().Split('.')) && IpInUint(source.ToString().Split('.')) <= IpInUint(end.ToString().Split('.')));
            }

            return false;
        }

        private static uint IpInUint(string[] s)
        {
            return (Convert.ToUInt32(s[0]) << 24) | (Convert.ToUInt32(s[1]) << 16) | (Convert.ToUInt32(s[2]) << 8) | (Convert.ToUInt32(s[3]));
        }
    }
}