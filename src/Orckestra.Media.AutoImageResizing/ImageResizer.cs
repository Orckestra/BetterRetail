using Composite.Core.WebClient.Renderings.Page;
using Composite.Core.Xml;
using Composite.Data.Types;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;

namespace Orckestra.Media.AutoImageResizing
{
    public class ImageResizer : IPageContentFilter
    {
        private static IReadOnlyCollection<string> WidthBreakpoints;
        private static string MaxWidthLimit;
        private static IReadOnlyCollection<string> ImageSupportFormats;

        static ImageResizer()
        {
            WidthBreakpoints = ConfigurationManager.AppSettings["ImageWidthBreakpoints"]
                .Split(',')
                .Select(item => item.Trim()).ToList();
            MaxWidthLimit = ConfigurationManager.AppSettings["ImageMaxWidthLimit"].Trim();
            ImageSupportFormats = ConfigurationManager.AppSettings["ImageSupportFormats"]
                .Split(',')
                .Select(item => item.Trim()).ToList();
        }
        public void Filter(XhtmlDocument document, IPage page)
        {
            ConvertImagesToPictureTags(document.Body);
        }
        private static void ConvertImagesToPictureTags(XElement root)
        {
            var imgTags = root.Descendants()
                .Where(e => e.Name.LocalName.Equals("img", StringComparison.OrdinalIgnoreCase)
                            && e.Parent != null
                            && !e.Parent.Name.LocalName.Equals("picture", StringComparison.OrdinalIgnoreCase))
                .ToList();
            imgTags.ForEach(ConvertImageToPictureTag);
        }

        private static void ConvertImageToPictureTag(XElement imgElement)
        {
            var imageSrc = imgElement.Attribute("src")?.Value;
            if (imageSrc == null) return;

            imageSrc = string.Concat(imageSrc, imageSrc.Contains("?") ? "&amp;" : "?");
            var pictureElement = new XElement("picture");
            foreach (var widthBreakpoint in WidthBreakpoints)
            {
                var minWidth = $"(max-width: {widthBreakpoint}px)";
                foreach (var imageSupportFormat in ImageSupportFormats)
                {
                    if (ImageFormatSupportHelper.IsSupported(imageSupportFormat))
                    {
                        pictureElement.Add(new XElement("source",
                            new XAttribute("srcset",
                                $"{imageSrc}mw={widthBreakpoint}&amp;ResizingAction=3&amp;mt={imageSupportFormat}"),
                            new XAttribute("media", minWidth),
                            new XAttribute("type", imageSupportFormat)));
                    }
                }
            }

            pictureElement.Add(new XElement("img",
                new XAttribute("src",
                    $"{imageSrc}?mw={MaxWidthLimit}"),
                new XAttribute("alt", imgElement.Attributes().FirstOrDefault(item => item.Name.LocalName == "alt")?.ToString() ?? string.Empty),
                new XAttribute("loading", "lazy")));
            imgElement.ReplaceWith(pictureElement);
        }

        public int Order { get; }
    }
}
