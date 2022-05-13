using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Hosting;
using System.Web.Mvc;
using HandlebarsDotNet;
using Orckestra.Composer.Configuration;
using Orckestra.Overture.Caching;
using static Orckestra.Composer.ComposerConfiguration;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;


namespace Orckestra.Composer.ViewEngine
{
    /// <summary>
    /// ViewEngine for support of Handlebars template files
    /// </summary>
    internal /*sealed*/ class HandlebarsViewEngine : VirtualPathProviderViewEngine
    {
        private readonly IHandlebars _handlebars;
        protected ICacheProvider CacheProvider { get; }

        private static readonly Regex TemplatePartialFinder = new Regex(@"\{\{>\s*?(?<Partial>[^\s}]+)\s*?\}\}", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
        private Dictionary<string, IEnumerable<string>> dependenciesList = new Dictionary<string, IEnumerable<string>>();
        private FileSystemWatcher fileWacher;

        /// <summary>
        /// Create the core instance of the ViewEngine
        /// </summary>
        /// <param name="cacheProvider">The cache provider holding reusable items</param>
        public HandlebarsViewEngine(ICacheProvider cacheProvider)
        {
            CacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));

            ViewLocationFormats = HandlebarsViewEngineConfiguration.ViewLocationFormats.ToArray();
            PartialViewLocationFormats = ViewLocationFormats;
            ViewLocationCache = new DefaultViewLocationCache(TimeSpan.FromDays(1));
            _handlebars = Handlebars.Create(Handlebars.Configuration);
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
            CacheKey templateCacheKey = new CacheKey(CacheConfigurationCategoryNames.HandlebarsView)
            {
                Key = virtualPath.Substring(virtualPath.LastIndexOf('/') + 1)
            };

            return CacheProvider.GetOrAdd(templateCacheKey, () =>
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

                InitMonitorTamplateFiles();

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
        private Dictionary<string, HandlebarsView> GetDependenciesFromCache(ControllerContext controllerContext, string virtualPath)
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

                if (result.View is HandlebarsView view)
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
        private void InitMonitorTamplateFiles()
        {
            if (fileWacher != null) return;

            fileWacher = MonitorTamplateFiles(HostingEnvironment.MapPath(HandlebarsViewEngineConfiguration.TamplateHbsDirectory));
            dependenciesList = GetTamplateDependencyList(HandlebarsViewEngineConfiguration.TamplateHbsDirectory);
        }

        /// <summary>
        /// Remove cache key if some files was changed 
        /// </summary>
        /// <param name="directory">watch physical directory</param>
        private FileSystemWatcher MonitorTamplateFiles(string directory)
        {
            if (string.IsNullOrEmpty(directory))
                return null;

            string fileWatcherMask = "*.hbs";
            var fileSystemWatcher = new FileSystemWatcher(directory, fileWatcherMask);

            fileSystemWatcher.Created += OnChanged;
            fileSystemWatcher.Changed += OnChanged;
            fileSystemWatcher.Deleted += OnChanged;
            fileSystemWatcher.Renamed += OnRenamed;
            fileSystemWatcher.EnableRaisingEvents = true;
            return fileSystemWatcher;
        }

        /// <summary>
        /// Find all handlebar dependency in folder
        /// </summary>
        /// <param name="virtDirectory">watch virtual directory</param>
        private Dictionary<string, IEnumerable<string>> GetTamplateDependencyList(string virtDirectory)
        {
            var dependencyList = new Dictionary<string, IEnumerable<string>>();
            var temaplateDirectory = VirtualPathProvider.GetDirectory(virtDirectory);

            foreach (VirtualFile file in temaplateDirectory.Files)
            {
                var temaplateDependency = GetViewPartialNames(file.VirtualPath);
                if (temaplateDependency.Any())
                {
                    dependencyList.Add(file.Name, temaplateDependency);
                }
            }

            return dependencyList;
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            RemoveTemplateCacheKey(e.Name);
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            RemoveTemplateCacheKey(e.Name);
        }

        private void RemoveTemplateCacheKey(string templateFileName)
        {
            CacheKey templateCacheKey = new CacheKey(CacheConfigurationCategoryNames.HandlebarsView)
            {
                Key = templateFileName
            };

            CacheProvider.Remove(templateCacheKey);

            var templateName = templateFileName.Substring(0, templateFileName.IndexOf('.'));
            var templatesWitchContain = dependenciesList.Where(x => x.Value.Contains(templateName)).Select(x => x.Key);
            foreach (var template in templatesWitchContain)
            {
                RemoveTemplateCacheKey(template);
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
            if (helper == null) { throw new ArgumentNullException(nameof(helper)); }
            if (string.IsNullOrEmpty(helper.HelperName)) { throw new ArgumentException(GetMessageOfNullEmpty(nameof(helper.HelperName)), nameof(helper)); }

            if (_handlebars.Configuration.Helpers.ContainsKey(helper.HelperName) ||
                _handlebars.Configuration.BlockHelpers.ContainsKey(helper.HelperName))
            {
                throw new InvalidOperationException(string.Format("override not allowed for registered helper {{{{{0}}}}}", helper.HelperName));
            }
        }

        private void ValidateRegisterHelper(IHandlebarsBlockHelper helper)
        {
            if (helper == null) { throw new ArgumentNullException(nameof(helper)); }
            if (string.IsNullOrEmpty(helper.HelperName)) { throw new ArgumentException(GetMessageOfNullEmpty(nameof(helper.HelperName)), nameof(helper)); }

            if (_handlebars.Configuration.Helpers.ContainsKey(helper.HelperName) ||
                _handlebars.Configuration.BlockHelpers.ContainsKey(helper.HelperName))
            {
                throw new InvalidOperationException(string.Format("override not allowed for registered helper {{{{{0}}}}}", helper.HelperName));
            }
        }
    }
}
