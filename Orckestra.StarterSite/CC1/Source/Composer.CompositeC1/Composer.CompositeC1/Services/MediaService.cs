using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Composite.Core.Routing;
using Composite.Data;
using Composite.Data.Types;
using Orckestra.Composer.CompositeC1.Media;
using Orckestra.Composer.CompositeC1.Pages;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.CompositeC1.Services
{
    public class MediaService : IMediaService
    {
        public ImageInfo GetImageInfo(Guid itemId, CultureInfo cultureInfo)
        {
            if (cultureInfo == null) { throw new ArgumentNullException("cultureInfo"); }
            if (itemId == Guid.Empty) { throw new ArgumentNullException("itemId"); }

            using (var connection = new DataConnection(cultureInfo))
            {
                var page = connection.Get<ComposerImage>().FirstOrDefault(p => p.Id == itemId);
                if (page == null)
                {
                    throw new ArgumentException(string.Format("Item '{0}' does not exist or it is not a ComposerImage.", itemId));
                }

                return new ImageInfo
                {
                    Url = GetMediaUrl(page.Image),
                    Alt = page.Alt
                };
            }
        }

        public string GetMediaUrl(string mediaPath)
        {
            if (string.IsNullOrEmpty(mediaPath))
            {
                return string.Empty;
            }

            string[] parts = mediaPath.Split(new[] { ':' });

            string mediaStore = parts[0];
            Guid mediaId = new Guid(parts[1]);

            string mediaUrl = MediaUrls.BuildUrl(new MediaUrlData { MediaStore = mediaStore, MediaId = mediaId, QueryParameters = new NameValueCollection() },
                                                 UrlKind.Public);

            // Allows media player to receive a nice url with an extension
            return new Regex("_[^_]").Replace(mediaUrl, "$");
        }

    }
}
