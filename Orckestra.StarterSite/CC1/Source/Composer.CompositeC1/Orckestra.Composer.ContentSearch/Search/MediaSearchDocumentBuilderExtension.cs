using Composite.Core.Routing;
using Composite.Data;
using Composite.Data.Types;
using Composite.Search.Crawling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.ContentSearch.Search
{
    public class MediaSearchDocumentBuilderExtension : ISearchDocumentBuilderExtension
    {
        public virtual void Populate(SearchDocumentBuilder searchDocumentBuilder, IData data)
        {   
            if (data is IMediaFile mediaFile
            && !string.IsNullOrWhiteSpace(mediaFile.MimeType)
            && FrontendSearchSupported(mediaFile.MimeType))
            {
                searchDocumentBuilder.Url = MediaUrls.BuildUrl(mediaFile);
            }
        }

        protected virtual bool FrontendSearchSupported(string mimeType)
        {
            return mimeType.StartsWith("video/", StringComparison.InvariantCultureIgnoreCase)
                || mimeType.StartsWith("image/", StringComparison.InvariantCultureIgnoreCase)
                || mimeType.StartsWith("application/", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
