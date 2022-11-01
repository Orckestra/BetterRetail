using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Configuration
{
    public static class ImageConfiguration
    {
        public static string ProductTileImageSize { get; set; } = "L";
        /// <summary>
        ///     Get/Set the ImageSize for dislaying thumbnails
        /// </summary>
        public static string CartThumbnailImageSize { get; set; } = "M";

        /// <summary>
        ///     Get/Set the ImageSize for dislaying thumbnails
        /// </summary>
        public static string RecurringCartSummaryThumbnailImageSize { get; set; } = "S";
    }
}
