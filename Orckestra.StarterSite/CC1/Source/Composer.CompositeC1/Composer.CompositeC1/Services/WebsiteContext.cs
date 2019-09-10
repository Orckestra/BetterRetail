using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Composite.Data;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.CompositeC1.Services
{
    public class WebsiteContext: IWebsiteContext
    {
        public WebsiteContext()
        {
            if (_websiteId == Guid.Empty)
            {
                if (SitemapNavigator.CurrentHomePageId != Guid.Empty)
                {
                    _websiteId = SitemapNavigator.CurrentHomePageId;
                }
                else
                {
                    var websiteId = HttpContext.Current.Request.Headers["WebsiteId"];
                    Guid.TryParse(websiteId, out _websiteId);
                }
            }
        }

        private Guid _websiteId;
        public Guid WebsiteId {
            get
            {
               

                return _websiteId;
            }
        }
    }
}
