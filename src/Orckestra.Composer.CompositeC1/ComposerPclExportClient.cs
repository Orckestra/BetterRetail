using System.Net;
using ServiceStack;

namespace Orckestra.Composer.CompositeC1
{
    /// <exclude />
    public static class ComposerRequestFilter
    {
        /// <summary>
        /// Registers a global ServiceStack client request filter that sets the HTTP Referer header
        /// for performance profiling logging on every outbound OCS API call.
        /// </summary>
        public static void Register()
        {
            JsonServiceClient.GlobalRequestFilter += (HttpWebRequest req) =>
            {
                var context = ContextPreservationHttpModule.PreservedHttpContext.Value;

                // Setting the 'Referer' for the performance profiling logging
                if (string.IsNullOrEmpty(req.Referer) && context != null)
                {
                    req.Referer = context.Request.Url.ToString();
                }
            };
        }
    }
}
