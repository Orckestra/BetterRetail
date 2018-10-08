using System;
using System.Web.Mvc;
using Composite.Core.WebClient.Renderings.Page;

namespace Orckestra.Composer.CompositeC1.Extensions
{
    public static class ControllerExtension
    {
        /// <summary>
        /// Return true if the page is access through C1.
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static bool IsInPreviewMode(this Controller controller)
        {
            return (PageRenderer.RenderingReason == RenderingReason.ScreenshotGeneration
                   || PageRenderer.RenderingReason == RenderingReason.PreviewUnsavedChanges
                   || PageRenderer.RenderingReason == RenderingReason.C1ConsoleBrowserPageView);
        }

        /// <summary>
        /// Simple method which call a delegate with mocked parameters. Useful in preview scenario where for example 
        /// on a product detail page, a default product id need to be provided to controller method. 
        /// If we are not in preview mode, return an empty view.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="methodToExecuteInPreviewMode"></param>
        /// <returns></returns>
        public static ActionResult HandlePreviewMode(this Controller controller, Func<ActionResult> methodToExecuteInPreviewMode)
        {
            if (IsInPreviewMode(controller))
            {
                return methodToExecuteInPreviewMode();
            }
            return new ViewResult();
        }
    }
}
