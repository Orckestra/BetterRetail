using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Orckestra.Media.AutoImageResizing
{
    public static class AutoImageResizingConfiguration
    {
        public static List<string> ImageFormats { get; }
        public static List<int> WidthBreakpoints { get; }
        public static int MaxWidth { get; }

        static AutoImageResizingConfiguration()
        {
            WidthBreakpoints = ConfigurationManager.AppSettings["AutoImageResizing.ImageWidthBreakpoints"].Split(',').Select(item => int.Parse(item.Trim())).ToList();
            ImageFormats = ConfigurationManager.AppSettings["AutoImageResizing.ImageFormats"].Split(',').Select(item => item.Trim()).ToList();
            MaxWidth = int.Parse(ConfigurationManager.AppSettings["AutoImageResizing.MaxWidth"]);

        }
    }
}
