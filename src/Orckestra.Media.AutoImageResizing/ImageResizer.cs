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
        private static readonly XName PictureTagXName = Namespaces.Xhtml + "picture";
        private static readonly XName SourceTagXName = Namespaces.Xhtml + "source";
        private static readonly XName ImgTagXName = Namespaces.Xhtml + "img";

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
                var pictureElement = new XElement(PictureTagXName);

                foreach (var widthBreakpoint in WidthBreakpoints.OrderBy(item => item))
                {
                    var mediaRule = $"(max-width: {widthBreakpoint}px)";
                    foreach (var imageSupportFormat in ImageSupportFormats)
                    {
                        if (ImageFormatSupportHelper.IsSupported(imageSupportFormat))
                        {
                            pictureElement.Add(new XElement(SourceTagXName,
                                new XAttribute("srcset", AutoImageResizingHelper.GetResizedImageUrl(imageSrc, widthBreakpoint, imageSupportFormat)),
                                new XAttribute("media", mediaRule),
                                new XAttribute("type", imageSupportFormat)));
                        }
                    }
                }

                var attributesToCopy = imgElement.Attributes()
                    .Where(_ => _.Name.LocalName != "src"
                                && _.Name.LocalName != "loading");

                var newImgElement =  new XElement(ImgTagXName,
                    new XAttribute("src", AutoImageResizingHelper.GetResizedImageUrl(imageSrc, MaxWidth)),
                    new XAttribute("loading", "lazy"));
                newImgElement.Add(attributesToCopy.Select(a => new XAttribute(a.Name, a.Value)));

                pictureElement.Add(newImgElement);
                imgElement.ReplaceWith(pictureElement);
            }
        }

        public int Order { get; }
    }
}
