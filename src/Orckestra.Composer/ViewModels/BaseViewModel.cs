using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Fasterflect;
using Newtonsoft.Json;
using Orckestra.Composer.Providers;

namespace Orckestra.Composer.ViewModels
{
    /// <summary>
    /// Base class for all view models used throughout the Composer project.
    /// </summary>
    public abstract class BaseViewModel : IBaseViewModel
    {
        private IViewModelMetadataRegistry _viewModelMetadataRegistry;
        private readonly static ConcurrentDictionary<Type, List<ViewModelProperty>> _childViewModel = new ConcurrentDictionary<Type, List<ViewModelProperty>>();

        /// <summary>
        /// Bag holding properties that will be added dynamically to the view model.
        /// </summary>
        [MetadataIgnore]
        public Dictionary<string, object> Bag { get; private set; }

        /// <summary>
        /// Bag holding properties that will be added dynamically to the Json Context
        /// </summary>
        [MetadataIgnore]
        public Dictionary<string, object> Context { get; private set; }

        /// <summary>
        /// Json representation of the Context.
        /// </summary>
        //[MetadataIgnore]
        public string JsonContext
        {
            get
            {
                return JsonConvert.SerializeObject(Context, ComposerHost.Current == null ? new JsonSerializerSettings() : ComposerHost.Current.JsonSettings);
            }
        }

        protected BaseViewModel()
        {
            Bag = new Dictionary<string, object>();
            Context = new Dictionary<string, object>();
            _viewModelMetadataRegistry = ViewModelMetadataRegistry.Current;
        }

        /// <summary>
        /// Sets the IViewModelMetadataRegistry to be used by the BaseViewModel.
        /// </summary>
        /// <param name="viewModelMetadataRegistry"></param>
        public void SetViewModelMetadataRegistry(IViewModelMetadataRegistry viewModelMetadataRegistry)
        {
            _viewModelMetadataRegistry = viewModelMetadataRegistry;
        }


        /// <summary>
        /// Produces a dictionary of key-value pairs based on the metadata of the <see cref="BaseViewModel"/>.
        /// </summary>
        /// <returns>Dictionnary of key-value pairs based on the metadata</returns>
        public IDictionary<string, object> ToDictionary()
        {
            var metadata = _viewModelMetadataRegistry.GetViewModelMetadata(GetType());

            var dictionnary = metadata
                .ToDictionary(meta => meta.DisplayName, GetPropertyMetadataValue);

            var childViewModels = _childViewModel.GetOrAdd(GetType(), (key) => GetChildViewModel(key));

            foreach (var getter in childViewModels)
            {
                var viewModel = getter.Getter(this);
                object dict = null;
                if (viewModel != null)
                {

                    if (getter.IsList)
                    {
                        IList viewModels =
                            (IList) Activator.CreateInstance(typeof (List<>).MakeGenericType(typeof(Dictionary<string, object>)));
                        foreach (IBaseViewModel current in (IList) viewModel)
                        {
                            var currentDic = current.ToDictionary();
                            viewModels.Add(currentDic);
            }
                        dict = viewModels;
        }
                    else
        {
                        dict = ((IBaseViewModel) viewModel).ToDictionary();
                    }
                }
                dictionnary[getter.PropertyInfo.Name] = dict;
            }
            return dictionnary;
        }

        private List<ViewModelProperty> GetChildViewModel(Type type)
        {
            var getters = new List<ViewModelProperty>();
            foreach (var property in type.GetProperties())
            {
                var listType =
                    property.PropertyType.GetInterfaces()
                        .Concat(new[] { property.PropertyType })
                        .FirstOrDefault(
                            i => i.IsGenericType && typeof (IList<>).IsAssignableFrom(i.GetGenericTypeDefinition()));
                if (typeof (IBaseViewModel).IsAssignableFrom(property.PropertyType))
                {
                    var getter = new ViewModelProperty
                    {
                        Getter = property.DelegateForGetPropertyValue(Flags.InstancePublic),
                        PropertyInfo = property
                    };
                    getters.Add(getter);
                }
                else if (listType != null && typeof (IBaseViewModel).IsAssignableFrom(listType.GetGenericArguments()[0]))
                {
                    var getter = new ViewModelProperty
                    {
                        Getter = property.DelegateForGetPropertyValue(Flags.InstancePublic),
                        PropertyInfo = property,
                        ListModelType = listType.GetGenericArguments()[0],
                        IsList = true
                    };
                    getters.Add(getter);
                }
            }
            return getters;
        }

        private object GetPropertyMetadataValue(IPropertyMetadata propertyMetadata)
        {
            var value = propertyMetadata.GetValue(this);
            return value is BaseViewModel model ? model.ToDictionary() : value;
        }

        /// <summary>
        ///     Converts the current instance of view model to an extension version of this model of type <typeparamref name="T" />
        /// </summary>
        /// <typeparam name="T">Type of the extended version of this model</typeparam>
        /// <returns>The extended version of the view model</returns>
        /// <exception cref="InvalidOperationException">
        ///     Thrown if <typeparamref name="T" /> is not an extension of this current
        ///     type of view model
        /// </exception>
        public T AsExtensionModel<T>()
        {
            ValidateExtensionType<T>();
            return ExtensionTypeProxyFactory.Default.Create<T>(this);
        }


        /// <summary>
        ///     Validates that <typeparamref name="T" /> extends IExtensionOf&lt;X&gt; where X is the type of this instance.
        /// </summary>
        private void ValidateExtensionType<T>()
        {
            var myType = GetType();
            var extensionType = typeof(T);
            var isValidExtensionType = extensionType.GetInterfaces()
                .Any(t => t.IsGenericType &&
                          typeof(IExtensionOf<>).IsAssignableFrom(t.GetGenericTypeDefinition()) &&
                          t.GetGenericArguments()[0] == myType);

            if (!isValidExtensionType)
            {
                throw new InvalidOperationException(string.Format("{0} does not extend IExtensionOf<{1}>",
                    extensionType.Name, myType.Name));
            }
        }

        private class ViewModelProperty
        {
            public PropertyInfo PropertyInfo { get; set; }

            public MemberGetter Getter { get; set; }

            public bool IsList { get; set; }

            public Type ListModelType { get; set; }
        }
    }
}