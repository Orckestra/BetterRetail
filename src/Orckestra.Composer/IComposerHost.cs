using System;
using System.Collections.Generic;
using System.Reflection;
using Orckestra.Composer.Dependency;
using Orckestra.Composer.ViewEngine;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture;

namespace Orckestra.Composer
{
    public interface IComposerHost : IDependencyContainer
    {
        List<Type> RegisteredInterfaces { get; }
        void Init();

        void LoadPlugins();

        IViewModelMetadataRegistry MetadataRegistry { get; }

        /// <summary>
        /// The event raised after host initialization.
        /// </summary>
        event EventHandler Initialized;

        void RegisterControllers(Assembly assembly);
        void RegisterApiControllers(Assembly assembly);
        void RegisterExceptionFiltersForApiControllers(params Type[] filterTypes);
        void RegisterHandlebarsHelper(IHandlebarsHelper helper);
        void RegisterHandlebarsHelper(IHandlebarsBlockHelper helper);
    }
}