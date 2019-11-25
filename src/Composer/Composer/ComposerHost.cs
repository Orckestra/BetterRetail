using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Runtime.InteropServices;
using Autofac;
using Autofac.Core;
using Autofac.Integration.Mvc;
using Newtonsoft.Json;
using Orckestra.Caching;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Kernel;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.ViewEngine;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.Components.Caching;

namespace Orckestra.Composer
{
    public sealed class ComposerHost : IComposerHost
    {
        public List<Type> RegisteredInterfaces { get; } = new List<Type>();
        private const string ComposerDllRegex = "Orckestra\\.Composer(\\.(.+))?.dll$";

        public static ComposerHost Current { get; private set; }

        public CacheClientRegistry CacheRegistry { get; private set; }

        private readonly AutofacDependencyContainer _dependencyContainer;
        private IAspNetConfigurationManager _aspNetConfigurationManager;
        private AssemblyHelper _assemblyHelper;
        private readonly IDictionary<string, _Assembly> _appDomainAssemblies;
        private IComposerEnvironment _environment;
        public bool AutoCrawlEnabled { get; private set; }
        public _Assembly[] AssembliesToInclude { get; private set; }
        public IViewModelMetadataRegistry MetadataRegistry { get; private set; }

        private ILifetimeScope _rootResolver;
        private ILifetimeScope RootResolver
        {
            get
            {
                if (_rootResolver == null)
                {
                    throw new InvalidOperationException("Host must be initialized before resolving.");
                }
                return _rootResolver;
            }
        }

        private HandlebarsViewEngine _viewEngine = null;
        private HandlebarsViewEngine ViewEngine
        {
            get
            {
                if (_viewEngine == null)
                {
                    throw new InvalidOperationException("Host must be initialized before registering new handlerbars helpers.");
                }
                return _viewEngine;
        }
            set { _viewEngine = value; }
        }

        public event EventHandler Initialized;

        public JsonSerializerSettings JsonSettings { get; set; }

        public ComposerHost()
            : this(true)
        {
        }

        public ComposerHost(params _Assembly[] assembliesToInclude)
            : this(false, assembliesToInclude)
        {
        }

        private ComposerHost(bool autoCrawlEnabled, params _Assembly[] assembliesToInclude)
        {
            AutoCrawlEnabled = autoCrawlEnabled;
            AssembliesToInclude = assembliesToInclude ?? new _Assembly[0];

            if (!AutoCrawlEnabled && !AssembliesToInclude.Any())
            {
                throw new ArgumentException("The constructor you used does not support Auto crawling of assemblies and you didn't define any assemblies to include.");
            }

            _dependencyContainer = new AutofacDependencyContainer();
            _aspNetConfigurationManager = new AspNetConfigurationManager();
            _assemblyHelper = new AssemblyHelper();
            _appDomainAssemblies = new Dictionary<string, _Assembly>();
            _environment = new ComposerEnvironment();
        }

        public void Init()
        {
            if(Current == null)
            {
                LoadPlugins();
            }

            var overtureClient = CreateOvertureClient();
            Register(overtureClient);

            _rootResolver = _dependencyContainer.Build();
            ConfigureAsp();

            
            if (Initialized != null)
            {
                Initialized(this, EventArgs.Empty);
            }
        }

        public void LoadPlugins()
        {
            if (Current != null)
            {
                throw new InvalidOperationException("One ComposerHost is already initialized.");
            }

            InitializeRegistry();
            DefaultRegistration();
            LoadAssemblies();
            Current = this;
        }

        // Internal setters used in unit tests
        internal void SetAspNetConfigurationManager(IAspNetConfigurationManager aspNetConfigurationManager)
        {
            _aspNetConfigurationManager = aspNetConfigurationManager;
        }

        internal void SetAssemblyHelper(AssemblyHelper assemblyHelper)
        {
            _assemblyHelper = assemblyHelper;
        }

        internal void SetComposerEnvironment(IComposerEnvironment environment)
        {
            _environment = environment;
        }

        private void InitializeRegistry()
        {
            var metadataRegistry = new ViewModelMetadataRegistry();
            ViewModelMetadataRegistry.Current = metadataRegistry;
            MetadataRegistry = metadataRegistry;
        }

        private void LoadAssemblies()
        {
            var assemblies = new List<_Assembly>();

            // Load composer assemblies
            var loadedAssemblies = _assemblyHelper.SafeLoadAssemblies(ComposerDllRegex);
            assemblies.AddRange(loadedAssemblies);

            // Load other assemblies next to executing assembly
            if (AutoCrawlEnabled)
            {
                assemblies.AddRange(_assemblyHelper.SafeLoadAssemblies());
            }

            // Load specific assemblies
            if (AssembliesToInclude.Any())
            {
                assemblies.AddRange(AssembliesToInclude);
            }

            // Add assemblies to dictionary
            foreach (var assembly in assemblies)
            {
                _appDomainAssemblies[assembly.FullName] = assembly;
            }

            LoadExtensionsFromAssemblies();
        }

        private void LoadExtensionsFromAssemblies()
        {
            var sortedExtensionAssemblies = SortExtensionAssembliesByWeight(_appDomainAssemblies.Values);

            foreach (var assembly in sortedExtensionAssemblies)
            {
                foreach (var type in assembly.SafeGetTypes().Where(t => !t.IsAbstract))
                {
                    if (typeof(IComposerPlugin).IsAssignableFrom(type))
                    {
                        LoadPlugin(type);
                    }
                }
            }
        }
        //todo: document it on wiki and in composer doc. MK can you add it cause you did it and you know what it does?
        internal List<_Assembly> SortExtensionAssembliesByWeight(ICollection<_Assembly> values)
        {
            List<KeyValuePair<int, _Assembly>> sortedExtensions = new List<KeyValuePair<int, _Assembly>>();

            foreach (var assembly in values)
            {
                if (!assembly.ReferencesAssembly(typeof(IComposerPlugin).Assembly))
                {
                    continue;
                }

                foreach (var type in assembly.SafeGetTypes().Where(t => !t.IsAbstract))
                {
                    if (typeof(IComposerPlugin).IsAssignableFrom(type))
                    {
                        var attributes = assembly.GetCustomAttributes(typeof(ComposerAssemblyWeightAttribute), false);

                        int weight = int.MaxValue;

                        if (attributes.Length > 0)
                        {
                            var attribute = attributes[0] as ComposerAssemblyWeightAttribute;
                            if (attribute != null)
                            {
                                weight = attribute.ComposerAssemblyWeigh;
                            }
                        }
                        sortedExtensions.Add(new KeyValuePair<int, _Assembly>(weight, assembly));
                    }
                }
            }
            return sortedExtensions.OrderBy(sortBy => sortBy.Key).Select(keyValuePair => keyValuePair.Value).ToList();
        }

        private void LoadPlugin(Type type)
        {
            var plugin = (IComposerPlugin)Activator.CreateInstance(type);
            plugin.Register(this);
        }

        private void DefaultRegistration()
        {
            Register(MetadataRegistry);
            Register(_environment);

            Register<AutofacDependancyResolver, IDependencyResolver>();
            Register<ResxViewModelPropertyFormatter, IViewModelPropertyFormatter>(ComponentLifestyle.Singleton);
            
            Register<ResourceLocalizationProvider, ILocalizationProvider>(ComponentLifestyle.Singleton);

            Register<ViewModelMapper, IViewModelMapper>(ComponentLifestyle.Singleton);

            //Register mappings to commonly referenced contextual application properties such as HttpContextBase, HttpRequestBase, HttpResponseBase
            RegisterModule(new AutofacWebTypesModule());

            Register<ICacheClient>(NullCacheClient.Default);
            CacheRegistry = new CacheClientRegistry();
            CacheRegistry.ApplyConfiguration(GetCacheConfiguration());
            Register<ICacheClientRegistry>(CacheRegistry);
            Register<CacheProvider, ICacheProvider>();
            Register<PerRequestCacheClient>(ComponentLifestyle.PerRequest);

            Register<ComposerJsonSerializer, IComposerJsonSerializer>();
        }

        private CacheConfiguration GetCacheConfiguration()
        {
            var configCacheSection = ConfigurationManager.GetSection("composer/caching") as CacheConfiguration;
           
            if (configCacheSection != null)
            {
                return configCacheSection;
            }
            throw new Exception("Unable to get the cache configuration.");
        }

        private IOvertureClient CreateOvertureClient()
        {
            return ComposerOvertureClient.CreateFromConfig();
        }

        private void ConfigureAsp()
        {
            var jsonFormatter = new JsonMediaTypeFormatter();
            var viewModelMapper = Resolve<IViewModelMapper>();

            JsonSettings = jsonFormatter.SerializerSettings;

            JsonSettings.NullValueHandling = NullValueHandling.Ignore;
            JsonSettings.MissingMemberHandling = MissingMemberHandling.Ignore;

            JsonSettings.Converters
                .Add(new ViewModelSerialization(viewModelMapper, MetadataRegistry));

            ViewEngine = new HandlebarsViewEngine(Resolve<ICacheProvider>());

            _aspNetConfigurationManager.Configure(
                RootResolver,
                ViewEngine,
                jsonFormatter);
        }

        public void Register<T>(T instance)
        {
            _dependencyContainer.Register(instance);
        }

        public void Register<TAs>(object instance)
        {
            _dependencyContainer.Register<TAs>(instance);
        }

        public void RegisterAs<T, TAs>() where T : TAs
        {
            RegisterAs<T, TAs>(ComponentLifestyle.Transient);
        }

        public void RegisterAs<T, TAs>(ComponentLifestyle lifestyle) where T : TAs
        {
            _dependencyContainer.RegisterAs<T, TAs>(lifestyle);
        }

        public T Resolve<T>()
        {
            return RootResolver.Resolve<T>();
        }

        public object Resolve(Type type)
        {
            return RootResolver.Resolve(type);
        }

        public T TryResolve<T>()
            where T : class
        {
            return (T)TryResolve(typeof(T));
        }

        public object TryResolve(Type type)
        {
            object instance;

            return RootResolver.TryResolve(type, out instance) 
                ? instance 
                : null;
        }

        public void Register(Type implementationType, ComponentLifestyle lifestyle, params Type[] asType)
        {
            _dependencyContainer.Register(implementationType, lifestyle, asType);
            RegisteredInterfaces.AddRange(asType);
        }

        public void Register<T>(ComponentLifestyle lifestyle)
        {
            Register(typeof(T), lifestyle);
        }

        public void Register<T>()
        {
            Register(typeof(T), ComponentLifestyle.Transient);
        }

        public void Register(Type implementationType)
        {
            Register(implementationType, ComponentLifestyle.Transient);
        }

        public void Register(Type implementationType, ComponentLifestyle lifestyle)
        {
            Register(implementationType, lifestyle, new Type[] { });
        }

        public void Register(Type implementationType, Type asType)
        {
            Register(implementationType, asType, ComponentLifestyle.Transient);
        }

        public void Register(Type implementationType, Type asType, ComponentLifestyle lifestyle)
        {

            Register(implementationType, lifestyle, new[] { asType });
        }

        public void Register<T, TAs>(ComponentLifestyle lifestyle) where T : TAs
        {
            Register(typeof(T), lifestyle, new[] { typeof(TAs) });
        }

        public void Register<T, TAs>() where T : TAs
        {
            Register(typeof(T), ComponentLifestyle.Transient, new[] { typeof(TAs) });
        }

        public void RegisterControllers(Assembly assembly)
        {
            _dependencyContainer.RegisterControllers(assembly);
        }

        public void RegisterApiControllers(Assembly assembly)
        {
            _dependencyContainer.RegisterApiControllers(assembly);
        }

        public void RegisterExceptionFiltersForApiControllers(params Type[] filterTypes)
        {
            _dependencyContainer.RegisterExceptionFiltersForApiControllers(filterTypes);
        }

        public void RegisterModule(IModule module)
        {
            _dependencyContainer.RegisterModule(module);
        }

        public void RegisterHandlebarsHelper(IHandlebarsHelper helper)
        {
            ViewEngine.RegisterHelper(helper);
        }

        public void RegisterHandlebarsHelper(IHandlebarsBlockHelper helper)
        {
            ViewEngine.RegisterHelper(helper);
        }
    }
}
