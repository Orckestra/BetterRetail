using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fasterflect;

namespace Orckestra.Composer.ViewModels
{
    public sealed class ViewModelMetadataRegistry : IViewModelMetadataRegistry
    {
        public static IViewModelMetadataRegistry Current { get; set; }

        private readonly Dictionary<Type, List<IPropertyMetadata>> _metadata;

        public ViewModelMetadataRegistry()
        {
            _metadata = new Dictionary<Type, List<IPropertyMetadata>>();
        }

        /// <summary>
        /// Gets an enumeration of metadata of View models.
        /// </summary>
        /// <param name="viewModelType">Type for which to retrieve metadata for.</param>
        /// <returns>Enumeration of metadata, or an empty enumerable if type was not discovered when host ran.</returns>
        public IEnumerable<IPropertyMetadata> GetViewModelMetadata(Type viewModelType)
        {
            if (HasMetadataFor(viewModelType))
            {
                var metadata = _metadata[viewModelType];
                return metadata;
            }

            throw new ArgumentException(string.Format("{0} has no metadata", viewModelType), "viewModelType");
        }

        public bool HasMetadataFor(Type viewModelType)
        {
            return _metadata.ContainsKey(viewModelType);
        }

        /// <summary>
        /// Gets the eligible properties for metadata.
        /// </summary>
        /// <param name="t">Type from which to extract properties.</param>
        /// <returns></returns>
        private IEnumerable<PropertyInfo> GetProperties(Type t)
        {
            var pInfos = t.Properties(Flags.InstancePublic).Where(p => !p.HasAttribute<MetadataIgnoreAttribute>());
            return pInfos;
        }

        public void LoadViewModelMetadataInAssemblyOf<T>()
        {
            LoadViewModelMetadataInAssemblyOf(typeof(T).Assembly);
        }
        
        /// <summary>
        /// Loads metadata from the specified assembly.
        /// </summary>
        /// <param name="assembly">Assembly from which the metadata should be loaded.</param>
        public void LoadViewModelMetadataInAssemblyOf(Assembly assembly)
        {
            var types = assembly.Types(Flags.Public);
            var iExtensionOfType = typeof(IExtensionOf<>);

            foreach (var type in types)
            {
                if (typeof(BaseViewModel).IsAssignableFrom(type) && !_metadata.ContainsKey(type))
                {
                    RegisterViewModel(type);
                }

                var extendedType = type.GetInterfaces()
                    .Where(i => i.IsGenericType &&
                                iExtensionOfType.IsAssignableFrom(i.GetGenericTypeDefinition()))
                    .Select(i => i.GetGenericArguments()[0])
                    .SingleOrDefault();

                if (extendedType != null)
                {
                    if (!_metadata.ContainsKey(extendedType))
                    {
                        RegisterViewModel(extendedType);
                    }
                    RegisterViewModelMetadata(type, extendedType);
                }
            }
        }

        /// <summary>
        /// Creates metadata for the given ViewModel type. If the metadata already exists, an <see cref="InvalidOperationException"/> is thrown.
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <returns>List of <see cref="IPropertyMetadata"/> for the specified <see cref="viewModelType"/>.</returns>
        private List<IPropertyMetadata> CreateMetadataFor(Type viewModelType)
        {
            bool metaAlreadyExist = _metadata.TryGetValue(viewModelType, out List<IPropertyMetadata> metadataList);

            if (metaAlreadyExist)
            {
                throw new InvalidOperationException(string.Format("The ViewModel '{0}' has already been registered.", viewModelType.AssemblyQualifiedName));
            }

            metadataList = new List<IPropertyMetadata>();
            _metadata.Add(viewModelType, metadataList);

            return metadataList;
        }


        /// <summary>
        /// Registers Metadata for classes extending <see cref="BaseViewModel"/>.
        /// </summary>
        /// <param name="viewModelType">Type to register.</param>
        private void RegisterViewModel(Type viewModelType)
        {
            if (viewModelType == null) { throw new ArgumentNullException(nameof(viewModelType)); }

            var properties = GetProperties(viewModelType);
            var metadataList = CreateMetadataFor(viewModelType);

            foreach (var pInfo in properties)
            {
                var pMeta = new InstancePropertyMetadata(pInfo);
                metadataList.Add(pMeta);
            }
        }

        /// <summary>
        /// Registers metadata from Metadata classes.
        /// </summary>
        /// <param name="metaType">Type extending <paramref name="vmType"/></param>
        /// <param name="vmType">Type <paramref name="metaType"/> is extending.</param>
        private void RegisterViewModelMetadata(Type metaType, Type vmType)
        {
            //var metaAttr = metaType.GetCustomAttribute<ViewModelMetadataAttribute>();
            //var vmType = metaAttr.ViewModelType;

            var metadataList = GetMetadataFor(vmType, metaType);
            var metaProperties = GetProperties(metaType);

            foreach (var pInfo in metaProperties)
            {
                HandleExistingMetadata(pInfo, metaType, vmType, metadataList);

                var pMeta = new BagPropertyMetadata(pInfo);
                metadataList.Add(pMeta);
            }
        }


        /// <summary>
        /// Gets the metadata for the given ViewModel type. If the metadata does not exist, an <see cref="InvalidMetadataException"/> is thrown.
        /// </summary>
        /// <param name="viewModelType">Type of the ViewModel for which the metadata is registered.</param>
        /// <param name="metadataType">Type of the Metadata for which this method is invoked.</param>
        /// <returns>List of <see cref="IPropertyMetadata"/> for the specified <see cref="viewModelType"/>.</returns>
        private List<IPropertyMetadata> GetMetadataFor(Type viewModelType, Type metadataType)
        {
            var couldRetrieveMetadata = _metadata.TryGetValue(viewModelType, out List<IPropertyMetadata> metadata);

            if (!couldRetrieveMetadata)
            {
                throw new InvalidMetadataException(viewModelType, metadataType, "Could not retrieve the ViewModel for which this metadata has been registered. Have you registered it?");
            }

            return metadata;
        }

        /// <summary>
        /// Checks if the property about the be registered already exists, and if so, throws appropriate exception.
        /// </summary>
        /// <param name="pInfo">PropertyInfo which will provide metadata.</param>
        /// <param name="metadataType">Type of the metadata invoking this method.</param>
        /// <param name="viewModelType">Type of the ViewModel associated with the <see cref="metadataList"/>.</param>
        /// <param name="metadataList">List of existing metadata for the right type.</param>
        private void HandleExistingMetadata(PropertyInfo pInfo, Type metadataType, Type viewModelType, IEnumerable<IPropertyMetadata> metadataList)
        {
            var pMeta = metadataList.FirstOrDefault(m => m.PropertyName == pInfo.Name);
            if (pMeta != null)
            {
                var isPropertyMeta = pMeta is InstancePropertyMetadata;

                if (isPropertyMeta)
                {
                    throw new InvalidMetadataException(viewModelType, metadataType,
                        string.Format("The property '{0}' already exists in the ViewModel class. " +
                                      "Metadata cannot override view model properties.", pInfo.Name));
                }
                else
                {
                    throw new InvalidMetadataException(viewModelType, metadataType,
                        string.Format("The property '{0}' has already been registered by another metadata class for the same ViewModel.",
                            pInfo.Name));
                }
            }
        }


    }
}