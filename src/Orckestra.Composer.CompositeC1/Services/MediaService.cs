using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Composite.Core.Routing;
using Composite.Data;
using Composite.Data.Types;
using Orckestra.Composer.CompositeC1.Media;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.CompositeC1.Services
{
    public class MediaService : IMediaService
    {
        public virtual ImageInfo GetImageInfo(string itemId, CultureInfo cultureInfo)
        {
            if (cultureInfo == null) { throw new ArgumentNullException(nameof(cultureInfo)); }
            if (string.IsNullOrEmpty(itemId)) { throw new ArgumentException(GetMessageOfNullEmpty(), nameof(itemId)); }

            using (var connection = new DataConnection(cultureInfo))
            {
                var image = connection.Get<IImageFile>().FirstOrDefault(p => p.KeyPath == itemId);
                if (image == null)
                {
                    throw new ArgumentException(string.Format("Image file '{0}' does not exist", itemId));
                }

                return new ImageInfo
                {
                    Url = MediaUrls.BuildUrl(image),
                    Alt = image.Title
                };
            }
        }

        public virtual string GetMediaUrl(string mediaPath)
        {
            if (string.IsNullOrEmpty(mediaPath)) { return string.Empty; }

            string[] parts = mediaPath.Split(new[] { ':' });

            string mediaStore = parts[0];
            Guid mediaId = new Guid(parts[1]);

            string mediaUrl = MediaUrls.BuildUrl(new MediaUrlData 
            { 
                MediaStore = mediaStore, 
                MediaId = mediaId, 
                QueryParameters = new NameValueCollection() 
            }, UrlKind.Public);

            // Allows media player to receive a nice url with an extension
            return new Regex("_[^_]").Replace(mediaUrl, "$");
        }
    }
}