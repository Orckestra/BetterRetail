using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Fasterflect;
using Orckestra.Composer.Logging;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Services.Lookup;
using Orckestra.Overture.ServiceModel;

namespace Orckestra.Composer.ViewModels
{
    public sealed class ViewModelMapper : IViewModelMapper
    {
        private static ILog Log = LogProvider.GetCurrentClassLogger();

        private readonly IViewModelMetadataRegistry _metadataRegistry;
        private readonly IViewModelPropertyFormatter _viewModelPropertyFormatter;
        private readonly ILookupService _lookupService;
        private readonly ILocalizationProvider _localizationProvider;

        //todo: dependency on the lookup service?
        public ViewModelMapper(IViewModelMetadataRegistry metadataRegistry, IViewModelPropertyFormatter viewModelPropertyFormatter, 
            ILookupService lookupService, ILocalizationProvider localizationProvider)
        {
            _metadataRegistry = metadataRegistry ?? throw new ArgumentNullException(nameof(metadataRegistry));
            _viewModelPropertyFormatter = viewModelPropertyFormatter ?? throw new ArgumentNullException(nameof(viewModelPropertyFormatter));
            _lookupService = lookupService ?? throw new ArgumentNullException(nameof(lookupService));
            _localizationProvider = localizationProvider ?? throw new ArgumentNullException(nameof(localizationProvider));
        }


        /// <summary>
        /// Produces a dictionary of key-value pairs based on the metadata of a <see cref="BaseViewModel"/>.
        /// </summary>
        /// <param name="vm">ViewModel for which to extract metadata from May not be null.</param>
        /// <returns>Dictionnary of key-value pairs based on the metadata of the given <see cref="vm"/>'s type.</returns>
        public IDictionary<string, object> ToDictionary(IBaseViewModel vm)
        {
            if (vm == null) { throw new ArgumentNullException(nameof(vm)); }

            return vm.ToDictionary();
        }

        /// <summary>
        /// Maps a source object to a an object of <see cref="BaseViewModel"/>.
        /// </summary>
        /// <typeparam name="TViewModel">Type of ViewModel</typeparam>
        /// <param name="source">Source object to map</param>
        /// <param name="culture">Culture when translating text.</param>
        /// <returns>ViewModel with mapped values.</returns>
        public TViewModel MapTo<TViewModel>(object source, string culture)
            where TViewModel : IBaseViewModel, new()
        {
            if (string.IsNullOrWhiteSpace(culture))
            {
                var errorMessage = "Culture cannot be null or empty or whitespace";
                var exception = new ArgumentException(errorMessage, "culture");

                Log.ErrorException(errorMessage, exception);                

                throw exception;
            }

            try
            {
                var cultureInfo = CultureInfo.GetCultureInfo(culture);
                return MapTo<TViewModel>(source, cultureInfo);
            }
            catch (CultureNotFoundException ex)
            {
                var errorMessage = string.Format("Culture not found: {0}", culture);
                Log.ErrorException(errorMessage, ex);                

                throw;
            }
        }

        /// <summary>
        /// Maps a source object to a an object of <see cref="BaseViewModel"/>.
        /// </summary>
        /// <typeparam name="TViewModel">Type of ViewModel</typeparam>
        /// <param name="source">Source object to map</param>
        /// <param name="culture">Culture when translating text.</param>
        /// <returns>ViewModel with mapped values.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public TViewModel MapTo<TViewModel>(object source, CultureInfo culture)
            where TViewModel : IBaseViewModel, new()
        {
            return  (TViewModel) MapTo(typeof(TViewModel), source, culture);
        }

        private object MapTo(Type viewModelType, object source, CultureInfo culture)
        {
            if (source == null)
            {
                return null;
            }

            if (!typeof(BaseViewModel).IsAssignableFrom(viewModelType))
            {
                throw new ArgumentException(string.Format("Type '{0}' must derive from BaseViewModel", viewModelType.FullName), "viewModelType");
            }

            var sourceType = source.GetType();
            var sourceTypeProperties = sourceType.GetProperties();
            var sourcePropertyBag = GetPropertyBagFromObject(source, sourceTypeProperties);
            var viewModelProperties = _metadataRegistry.GetViewModelMetadata(viewModelType);
            var viewModel = (BaseViewModel)Activator.CreateInstance(viewModelType);

            foreach (var viewModelProperty in viewModelProperties)
            {
                var sourceProperty = sourceTypeProperties.FirstOrDefault(p => p.Name == viewModelProperty.SourcePropertyName);
                var sourcePropertyMatches = sourceProperty != null;

                if (sourcePropertyMatches || IsViewModelPropertyInSourcePropertyBag(sourcePropertyBag, viewModelProperty))
                {
                    object viewModelPropertyValue = null;
                    if (sourcePropertyMatches)
                    {
                        viewModelPropertyValue = MapPropertyFromObject(source, sourceProperty, viewModelProperty, culture);
                    }
                    else if (IsViewModelPropertyInSourcePropertyBag(sourcePropertyBag, viewModelProperty))
                    {
                        viewModelPropertyValue = MapPropertyFromPropertyBag(sourcePropertyBag, viewModelProperty, culture);
                    }

                    if (viewModelProperty.LookupProperty && viewModelPropertyValue != null)
                    {
                        var param = new GetLookupDisplayNameParam
                        {
                            LookupType = viewModelProperty.LookupType,
                            LookupName = viewModelProperty.LookupName,
                            Value = viewModelPropertyValue.ToString(),
                            CultureInfo = culture,
                            Delimiter = viewModelProperty.LookupDelimiter
                        };
                        // calling blocking .Result because we don't want otmake the mapper async
                        viewModelPropertyValue = _lookupService.GetLookupDisplayNameAsync(param).Result; 
                    }
                    var formattedViewModelPropertyValue = LocalizeValue(viewModelPropertyValue, viewModelProperty, culture);
                    formattedViewModelPropertyValue = FormatValue(formattedViewModelPropertyValue, viewModelProperty, culture);
                    viewModelProperty.SetValue(viewModel, formattedViewModelPropertyValue);
                }
                else
                {
                    // Setting null in the case of value types will set its default value instead
                    // See https://msdn.microsoft.com/en-ca/library/xb5dd1f1(v=vs.110).aspx in Remarks section
                    viewModelProperty.SetValue(viewModel, null);
                }

            }
            return viewModel;
        }

        private object FormatValue(object value, IPropertyMetadata propertyMetadata, CultureInfo cultureInfo)
        {
            // check if VM propery can be formatted. If not, pass through
            if (propertyMetadata != null && propertyMetadata.FormattableProperty)
            {
                // format value
                return _viewModelPropertyFormatter.Format(value, propertyMetadata, cultureInfo);
            }
            return value;
        }
        private object LocalizeValue(object value, IPropertyMetadata propertyMetadata, CultureInfo cultureInfo)
        {
            if (value == null) { return null; }
            var strValue = value.ToString();

            // check if VM propery can be formatted. If not, pass through
            if (!string.IsNullOrWhiteSpace(strValue) && propertyMetadata != null && propertyMetadata.LocalizableEnumProperty)
            {
                // format value
                var localizedString = _localizationProvider.GetLocalizedString(new GetLocalizedParam
                {
                    Category = propertyMetadata.PropertyEnumLocalizationCategory,
                    Key = strValue,
                    CultureInfo = cultureInfo
                });
                return string.IsNullOrWhiteSpace(localizedString) && !propertyMetadata.PropertyEnumAllowEmptyValue 
                    ? value.ToString() 
                    : localizedString ?? string.Empty;
            }

            return value;
        }

        private object MapPropertyFromObject(object source, PropertyInfo sourceProperty, IPropertyMetadata viewModelProperty,
            CultureInfo culture)
        {
            return MapProperty(
                sourceProperty.Get(source), 
                sourceProperty.PropertyType, 
                viewModelProperty.PropertyType, 
                viewModelProperty.SourcePropertyName, 
                viewModelProperty.FormattableProperty || viewModelProperty.LocalizableEnumProperty, 
                culture);
        }

        private object MapPropertyFromPropertyBag(PropertyBag sourcePropertyBag, IPropertyMetadata viewModelProperty,
            CultureInfo culture)
        {
            var sourcePropertyBagValue = sourcePropertyBag[viewModelProperty.SourcePropertyName];

            if (sourcePropertyBagValue == null)
            {
                return null;
            }

            return MapProperty(
                sourcePropertyBagValue, 
                sourcePropertyBagValue.GetType(), 
                viewModelProperty.PropertyType, 
                viewModelProperty.SourcePropertyName, 
                viewModelProperty.FormattableProperty || viewModelProperty.LocalizableEnumProperty, 
                culture);
        }

        private object MapProperty(
            object sourceValue, 
            Type sourceType, 
            Type viewModelPropertyType, 
            string viewModelPropertyName,
            bool isFormattable,
            CultureInfo culture)
        {
            if (sourceValue == null)
            {
                return null;
            }

            object viewModelPropertyValue;
            if (CanBeLocalized(sourceType, viewModelPropertyType))
            {
                viewModelPropertyValue = GetLocalizablePropertyValue(sourceValue, culture);
            }
            else if (PropertiesAreList(sourceType, viewModelPropertyType))
            {
                viewModelPropertyValue = GetEnumerablePropertyValue((IEnumerable)sourceValue, viewModelPropertyType, viewModelPropertyName, culture);
            }
            else if (PropertiesAreArrays(sourceType, viewModelPropertyType))
            {
                viewModelPropertyValue = GetArrayPropertyValue((Array)sourceValue, viewModelPropertyType, viewModelPropertyName, culture);
            }
            else if (PropertyIsAViewModel(viewModelPropertyType))
            {
                viewModelPropertyValue = MapTo(viewModelPropertyType, sourceValue, culture);
            }
            else if (PropertiesAreCompatible(sourceType, viewModelPropertyType))
            {
                viewModelPropertyValue = sourceValue;
            }
            //TODO: remove && viewModelPropertyType.Name == "String" check. with [Formatting] attribute on int for example, it will fail silently but error should be thrown
                //maybe there should be check done during bootstrap, all [Formatting] properties should be String, otherwise throw at bootstrap
            else if (isFormattable && viewModelPropertyType.Name == "String")
            {
                viewModelPropertyValue = sourceValue;
            }
            else
            {
                throw new InvalidOperationException(
                    string.Format(
                        "Cannot map property '{0}', because source property type was '{1}' and viewmodel property type was '{2}'.",
                        viewModelPropertyName,
                        sourceType.FullName,
                        viewModelPropertyType.FullName));
            }
            return viewModelPropertyValue;
        }

        private object GetEnumerablePropertyValue(IEnumerable sourceEnumerable, Type viewModelPropertyType, string viewModelPropertyName, CultureInfo culture)
        {
            var viewModelItemType = viewModelPropertyType.GenericTypeArguments.First();

            var viewModelList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(viewModelItemType));
            foreach (var sourceItem in sourceEnumerable)
            {
                viewModelList.Add(MapProperty(sourceItem, sourceItem.GetType(), viewModelItemType, viewModelPropertyName, false, culture));
            }
            return viewModelList;
        }

        private object GetArrayPropertyValue(Array sourceArray, Type viewModelPropertyType, string viewModelPropertyName, CultureInfo culture)
        {
            var viewModelItemType = viewModelPropertyType.GetElementType();

            var viewModelArray = Array.CreateInstance(viewModelItemType, sourceArray.Length);
            for (int i = 0; i < sourceArray.Length; i++)
            {
                var sourceItem = sourceArray.GetElement(i);
                viewModelArray.SetValue(MapProperty(sourceItem, sourceItem.GetType(), viewModelItemType, viewModelPropertyName, false, culture), i);
            }
            return viewModelArray;
        }

        private bool PropertiesAreList(Type sourceType, Type viewModelPropertyType)
        {
            return sourceType.InheritsOrImplements(typeof(List<>))
                   && viewModelPropertyType.InheritsOrImplements(typeof(List<>));
        }

        private bool PropertiesAreArrays(Type sourceType, Type viewModelPropertyType)
        {
            // Strings must not be treated as arrays here
            if (sourceType == typeof(string))
            {
                return false;
            }

            return sourceType.IsArray
                   && viewModelPropertyType.IsArray;
        }

        private static bool PropertiesAreCompatible(Type sourceType, Type viewModelPropertyType)
        {
            return viewModelPropertyType.IsAssignableFrom(sourceType);
        }

        private bool CanBeLocalized(Type sourceType, Type viewModelPropertyType)
        {
            return typeof(LocalizedString).IsAssignableFrom(sourceType) && viewModelPropertyType == typeof(string);
        }

        private bool PropertyIsAViewModel(Type viewModelPropertyType)
        {
            return typeof(BaseViewModel).IsAssignableFrom(viewModelPropertyType);
        }

        private static bool IsViewModelPropertyInSourcePropertyBag(PropertyBag sourcePropertyBag, IPropertyMetadata viewModelProperty)
        {
            return sourcePropertyBag != null && sourcePropertyBag.ContainsKey(viewModelProperty.SourcePropertyName);
        }

        private static string GetLocalizablePropertyValue(object sourceValue, CultureInfo culture)
        {
            var localizableString = (LocalizedString)sourceValue;
            return localizableString.GetLocalizedValue(culture.Name);
        }

        private static PropertyBag GetPropertyBagFromObject(object source, PropertyInfo[] sourceTypeProperties)
        {
            var propertyBagProperty = sourceTypeProperties.FirstOrDefault(p => p.Name == "PropertyBag" && p.PropertyType == typeof(PropertyBag));
            return propertyBagProperty == null ? null : (PropertyBag)propertyBagProperty.Get(source);
        }
    }
}