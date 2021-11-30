using Composite.Core.WebClient.Renderings.Page;
using Composite.Core.Xml;
using Composite.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Orckestra.Media.AutoImageResizing.Helpers;

namespace Orckestra.Media.AutoImageResizing
{
    public class ImageResizer : IPageContentFilter
    {
        private static IReadOnlyCollection<int> WidthBreakpoints;
        private static int MaxWidth;
        private static IReadOnlyCollection<string> ImageSupportFormats;
        static ImageResizer()
        {
            WidthBreakpoints = AutoImageResizingConfiguration.WidthBreakpoints;
            MaxWidth = AutoImageResizingConfiguration.MaxWidth;
            ImageSupportFormats = AutoImageResizingConfiguration.ImageFormats;
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

            if (AutoImageResizingHelper.IsLocalC1MediaWithoutResizingOptions(imageSrc))
            {
                var imageNamespace = imgElement.GetDefaultNamespace();
                var pictureElement = new XElement(imageNamespace + "picture");

                foreach (var widthBreakpoint in WidthBreakpoints.OrderBy(item => item))
                {
                    var mediaRule = $"(max-width: {widthBreakpoint}px)";
                    foreach (var imageSupportFormat in ImageSupportFormats)
                    {
                        if (ImageFormatSupportHelper.IsSupported(imageSupportFormat))
                        {
                            pictureElement.Add(new XElement("source",
                                new XAttribute("srcset", AutoImageResizingHelper.GetResizedImageUrl(imageSrc, widthBreakpoint, imageSupportFormat)),
                                new XAttribute("media", mediaRule),
                                new XAttribute("type", imageSupportFormat)));
                        }
                    }
                }

                var altText = imgElement.Attributes().FirstOrDefault(item => item.Name.LocalName == "alt")?.Value;
                var title = imgElement.Attributes().FirstOrDefault(item => item.Name.LocalName == "title")?.Value;
                var id = imgElement.Attributes().FirstOrDefault(item => item.Name.LocalName == "id")?.Value;

                pictureElement.Add(new XElement("img",
                    new XAttribute("src", AutoImageResizingHelper.GetResizedImageUrl(imageSrc, MaxWidth)),
                    string.IsNullOrWhiteSpace(altText) ? null : new XAttribute("alt", altText),
                    string.IsNullOrWhiteSpace(title) ? null : new XAttribute("title", title),
                    string.IsNullOrWhiteSpace(id) ? null : new XAttribute("id", id),
                    new XAttribute("loading", "lazy")));

                imgElement.ReplaceWith(pictureElement);
            }
        }

        public int Order { get; }
    }
}
