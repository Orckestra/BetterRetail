using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Caching;
using System.Web.Hosting;
using System.Web.Mvc;
using HandlebarsDotNet;
using Orckestra.Caching;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Utils;
using Orckestra.Overture.Caching;
using CacheItemPriority = System.Web.Caching.CacheItemPriority;

namespace Orckestra.Composer.ViewEngine
{
    /// <summary>
    /// ViewEngine for support of Handlebars template files
    /// </summary>
    internal /*sealed*/ class HandlebarsViewEngine : VirtualPathProviderViewEngine
    {
        private readonly IHandlebars _handlebars;
        private readonly ICacheProvider _cacheProvider;

        private static readonly Regex TemplatePartialFinder = new Regex(@"\{\{>\s*?(?<Partial>[^\s}]+)\s*?\}\}", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Create the core instance of the ViewEngine
        /// </summary>
        /// <param name="cacheProvider">The cache provider holding reusable items</param>
        public HandlebarsViewEngine(ICacheProvider cacheProvider)
        {
            if(cacheProvider == null) { throw new ArgumentNullException("cacheProvider"); }

            ViewLocationFormats = ComposerConfiguration.HandlebarsViewEngineConfiguration.ViewLocationFormats.ToArray();
            PartialViewLocationFormats = ViewLocationFormats;
            ViewLocationCache = new DefaultViewLocationCache(TimeSpan.FromDays(1));

            _handlebars = Handlebars.Create(Handlebars.Configuration);
            _cacheProvider = cacheProvider;
        }

        /// <summary>
        /// Creates the specified view by using the controller context, path of the view, and path of the master view.
        /// </summary>
        /// <param name="controllerContext"> The controller context.</param>
        /// <param name="viewPath">The virtual path of the view.</param>
        /// <param name="masterPath">An empty string (not supported).</param>
        /// <returns>A reference to the view</returns>
        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            return GetViewFromCache(controllerContext, viewPath);
        }

        /// <summary>
        /// Creates the specified view by using the controller context, path of the view, and path of the master view.
        /// </summary>
        /// <param name="controllerContext"> The controller context.</param>
        /// <param name="partialPath">The virtual path of the view.</param>
        /// <returns>A reference to the view</returns>
        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            return GetViewFromCache(controllerContext, partialPath);
        }

        private HandlebarsView GetViewFromCache(ControllerContext controllerContext, string virtualPath)
        {
            CacheKey templateCacheKey =  new CacheKey(CacheConfigurationCategoryNames.HandlebarsView)
            {
                Key = virtualPath
            };

            return _cacheProvider.GetOrAdd(templateCacheKey, () =>
            {
                //First, let's make sure the dependencies exist
                Dictionary<string, HandlebarsView> dependencies = GetDependenciesFromCache(controllerContext,
                    virtualPath);

                //Then let's compile
                Action<TextWriter, object> compiledTemplate;
                var file = VirtualPathProvider.GetFile(virtualPath);
                using (TextReader template = new StreamReader(file.Open()))
                {
                    compiledTemplate = _handlebars.Compile(template);
                }

                //Watch for file changes
                List<string> fileDependencies = new List<string>();
                fileDependencies.Add(virtualPath);
                fileDependencies.AddRange(dependencies.Select(kvp => kvp.Value.VirtualPath));
                MonitorFileChanges(templateCacheKey, fileDependencies);

                return new HandlebarsView(compiledTemplate, virtualPath, dependencies);
            });
        }

        /// <summary>
        /// There is no callback mecanism to resolve the TemplateNames when compiling an handlebars Template
        /// So we have to manually lookup the file, find the dependencies and precompile them.
        /// 
        /// Each template found is resolved by the ViewEngine to benefit from all caching mecanisms.
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="virtualPath"></param>
        private Dictionary<string,HandlebarsView> GetDependenciesFromCache(ControllerContext controllerContext, string virtualPath)
        {
            Dictionary<string, HandlebarsView> dependencies = new Dictionary<string, HandlebarsView>();

            foreach (var templateName in GetViewPartialNames(virtualPath))
            {
                //Find the partial view from cache
                var result = FindPartialView(controllerContext, templateName, true);
                if (result.View == null)
                {
                    //Partial view was not in cache, make sure we update the registred template
                    result = FindPartialView(controllerContext, templateName, false);
                }

                HandlebarsView view = result.View as HandlebarsView;
                if (view != null)
                {
                    _handlebars.RegisterTemplate(templateName, view.CompiledTemplate);

                    dependencies[templateName] = view;
                    foreach (var subDependency in view.Dependencies)
                    {
                        dependencies[subDependency.Key] = subDependency.Value;
                    }
                }
            }

            return dependencies;
        }

        /// <summary>
        /// Parses the HBS file to find all {{> templateName}} tags
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns>A distinct list of all templateNames found in the file</returns>
        private IEnumerable<string> GetViewPartialNames(string virtualPath)
        {
            List<string> partialNames = new List<string>();

            if (VirtualPathProvider.FileExists(virtualPath))
            {
                var file = VirtualPathProvider.GetFile(virtualPath);
                using (TextReader template = new StreamReader(file.Open()))
                {
                    string line;
                    while ((line = template.ReadLine()) != null)
                    {
                        partialNames.AddRange(TemplatePartialFinder
                            .Matches(line)
                            .OfType<Match>()
                            .Where(m => m.Success)
                            .Select(m => m.Groups["Partial"].Value)
                            );
                    }
                }
            }

            return partialNames;
        }

        //--

        /// <summary>
        /// Add a dummy cache entry, with file dependencies. 
        /// On this dummy cache expiracy, purge the _cacheProvider.
        /// </summary>
        /// <param name="cacheKey">Key to invalidate on file change</param>
        /// <param name="fileDependencies"></param>
        private void MonitorFileChanges(CacheKey cacheKey, IList<string> fileDependencies)
        {
            if (fileDependencies != null && fileDependencies.Any())
            {
                var monitoredFile = VirtualPathProvider.GetCacheDependency(fileDependencies.First(), fileDependencies, DateTime.UtcNow);

                HostingEnvironment.Cache.Add(cacheKey.GetFullCacheKey(),
                    cacheKey,
                    monitoredFile,
                    Cache.NoAbsoluteExpiration,
                    Cache.NoSlidingExpiration,
                    CacheItemPriority.High,
                    (s, removedCacheKey, reason) =>
                    {
                        _cacheProvider.Remove((CacheKey)removedCacheKey);
                    });
            }
        }
        //--

        public void RegisterHelper(IHandlebarsHelper helper)
        {
            ValidateRegisterHelper(helper);

            _handlebars.RegisterHelper(helper.HelperName, helper.HelperFunction);
        }
        public void RegisterHelper(IHandlebarsBlockHelper helper)
        {
            ValidateRegisterHelper(helper);

            _handlebars.RegisterHelper(helper.HelperName, helper.HelperFunction);
        }
        
        private void ValidateRegisterHelper(IHandlebarsHelper helper)
        {
            if (helper == null)
            {
                throw new ArgumentNullException("helper");
            }
        
            if (string.IsNullOrEmpty(helper.HelperName))
            {
                throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("HelperName"), "helper");
            }

            if (_handlebars.Configuration.Helpers.ContainsKey(helper.HelperName) ||
                _handlebars.Configuration.BlockHelpers.ContainsKey(helper.HelperName))
            {
                throw new InvalidOperationException(string.Format("override not allowed for registered helper {{{{{0}}}}}", helper.HelperName));
            }
        }

        private void ValidateRegisterHelper(IHandlebarsBlockHelper helper)
        {
            if (helper == null)
            {
                throw new ArgumentNullException("helper");
            }

            if (string.IsNullOrEmpty(helper.HelperName))
            {
                throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("HelperName"), "helper");
            }

            if (_handlebars.Configuration.Helpers.ContainsKey(helper.HelperName) ||
                _handlebars.Configuration.BlockHelpers.ContainsKey(helper.HelperName))
            {
                throw new InvalidOperationException(string.Format("override not allowed for registered helper {{{{{0}}}}}", helper.HelperName));
            }
        }
    }
}
