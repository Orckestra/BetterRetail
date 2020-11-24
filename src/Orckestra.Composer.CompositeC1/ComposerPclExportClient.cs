using System;
using System.Net;
using ServiceStack;

namespace Orckestra.Composer.CompositeC1
{
    /// <exclude />
    public class ComposerPclExport : Net40PclExport
    {
        public override void Config(HttpWebRequest req, bool? allowAutoRedirect, TimeSpan? timeout,
            TimeSpan? readWriteTimeout, string userAgent, bool? preAuthenticate)
        {
            base.Config(req, allowAutoRedirect, timeout, readWriteTimeout, userAgent, preAuthenticate);

            var context = ContextPreservationHttpModule.PreservedHttpContext.Value;

            // Setting the 'Referer' for the performance profiling logging
            if (string.IsNullOrEmpty(req.Referer) && context != null)
            {
                req.Referer = context.Request.Url.ToString();
            }
        }
    }
}
