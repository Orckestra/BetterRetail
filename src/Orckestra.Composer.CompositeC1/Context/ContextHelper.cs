using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
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
            return (PageRenderer.RenderingReason == RenderingReason.ScreenshotGeneration
                    || PageRenderer.RenderingReason == RenderingReason.PreviewUnsavedChanges
                    || PageRenderer.RenderingReason == RenderingReason.C1ConsoleBrowserPageView);
        }


        public static T HandlePreviewMode<T>(Func<T> methodToExecuteInPreviewMode)
        {
            if (IsInPreviewMode())
            {
                return methodToExecuteInPreviewMode();
            }
            return default(T);
        }
    }
}
