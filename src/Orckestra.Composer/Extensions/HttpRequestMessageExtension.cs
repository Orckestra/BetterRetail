using System.Net.Http;
using System.ServiceModel.Channels;
using System.Web;

namespace Orckestra.Composer.Extensions
{
    public static class HttpRequestMessageExtension
    {
        /// <summary>
        /// Gets the Client IP..
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>An IP address.</returns>
        public static string GetClientIp(this HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }

            if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                var prop = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];

                return prop.Address;
            }

            return null;
        }
    }
}
