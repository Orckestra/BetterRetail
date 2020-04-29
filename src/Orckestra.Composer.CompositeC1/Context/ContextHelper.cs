using System;
using Composite.Core.WebClient.Renderings.Page;

namespace Orckestra.Composer.CompositeC1.Context
{
    public class ContextHelper
    {
        /// <summary>
        /// Return true if the page is access through C1.
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static bool IsInPreviewMode()
        {
            return PageRenderer.RenderingReason == RenderingReason.ScreenshotGeneration
                    || PageRenderer.RenderingReason == RenderingReason.PreviewUnsavedChanges
                    || PageRenderer.RenderingReason == RenderingReason.C1ConsoleBrowserPageView;
        }


        public static T HandlePreviewMode<T>(Func<T> methodToExecuteInPreviewMode)
        {
            return IsInPreviewMode() ? methodToExecuteInPreviewMode() : (default);
        }
    }
}
