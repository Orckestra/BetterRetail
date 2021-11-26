using Composite.Core;
using Composite.Core.WebClient.Media;
using System.Collections.Generic;
using System.Linq;

namespace Orckestra.Media.AutoImageResizing.Helpers
{
    public static class ImageFormatSupportHelper
    {
        private static HashSet<string> _supportedFormats;
        public static bool IsSupported(string mediaType)
        {
            if (_supportedFormats == null)
            {
                var providers = ServiceLocator.GetServices<IImageFileFormatProvider>();
                _supportedFormats = new HashSet<string>(providers.Select(_ => _.MediaType));
            }
            return _supportedFormats.Contains(mediaType);
        }
    }
}
