using Composite.Plugins.Components.ComponentTags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Composite.Data;

namespace Orckestra.Composer.Utils
{
    public static class PageHelper
    {
        public static Dictionary<string, string> GetWebsitesRootPages()
        {
            return PageManager.GetChildrenIDs(Guid.Empty)
                .Select(rootPageId => PageManager.GetPageById(rootPageId))
                .Where(page => page != null)
                .ToDictionary(page => page.Id.ToString(), page => page.Title);
        }
    }
}
