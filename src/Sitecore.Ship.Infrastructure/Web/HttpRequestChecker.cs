using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Ship.Core.Contracts;
using Sitecore.Ship.Core.Domain;

namespace Sitecore.Ship.Infrastructure.Web
{
    public class HttpRequestChecker : ICheckRequests
    {
        public bool IsLocal
        {
            get { return HttpContext.Current.Request.IsLocal; }
        }

        public string UserHostAddress
        {
            get { return HttpContext.Current.Request.UserHostAddress; }
        }

        public string AuthToken(string configAuthHeader)
        {
            var headerValues = HttpContext.Current.Request.Headers.GetValues(configAuthHeader);

            if (headerValues != null)
            {
                var token = headerValues.FirstOrDefault();
                return token;
            }

            return string.Empty;
        }
    }
}