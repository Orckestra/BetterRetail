using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.ViewEngine
{
    /// <summary>
    /// Renderable View as created and owned by the HandlebarsViewEngine
    /// </summary>
    internal class HandlebarsView : IView
    {
        public Action<TextWriter, object> CompiledTemplate { get; private set; }
        public string VirtualPath  { get; private set; }
        public Dictionary<string,HandlebarsView> Dependencies { get; private set; }

        /// <summary>
        /// Creates a Handlebar View
        /// </summary>
        public HandlebarsView(Action<TextWriter, object> compiledTemplate, string virtualPath, Dictionary<string, HandlebarsView> dependencies)
        {
            CompiledTemplate = compiledTemplate;
            VirtualPath      = virtualPath;
            Dependencies     = dependencies;
        }

        /// <summary>
        /// Renders the specified view context by using the specified the writer object.
        /// </summary>
        /// <param name="viewContext">The view context.</param>
        /// <param name="writer">The writer object.</param>
        public void Render(ViewContext viewContext, TextWriter writer)
        {
            var model = ManageDynamicModels(viewContext.ViewData.Model);

            CompiledTemplate.Invoke(writer, model);
        }

        protected object ManageDynamicModels(object model)
        {
            var dynamicModel = model as BaseViewModel;
            if (dynamicModel == null)
            {
                return model;
            }
            var dictModel = dynamicModel.ToDictionary();
            return dictModel;
        }
    }
}
