using System;
using System.Globalization;
using Orckestra.Composer.Enums;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Services.Lookup;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Metadata;

namespace Orckestra.Composer.Product.Services
{
    public class ProductFormatter
    {
        private const string PropertyFormattingResource = "ProductSpecificationsPropertyFormatting";
        private const string BasePropertyTypeLookupResourceKey = "BasePropertyTypeLookup";
        private const string BasePropertyTypeNumberResourceKey = "BasePropertyTypeNumber";
        private const string BasePropertyTypeTextResourceKey = "BasePropertyTypeText";
        private const string BasePropertyTypeDateTimeResourceKey = "BasePropertyTypeDateTime";
        private const string BasePropertyTypeDecimalResourceKey = "BasePropertyTypeDecimal";
        private const string BasePropertyTypeCurrencyResourceKey = "BasePropertyTypeCurrency";
        private const string BasePropertyTypeBooleanTrueResourceKey = "BasePropertyTypeBooleanTrue";
        private const string BasePropertyTypeBooleanFalseResourceKey = "BasePropertyTypeBooleanFalse";

        private readonly ILocalizationProvider _localizationProvider;
        private readonly ILookupService _lookupService;

        public ProductFormatter(ILocalizationProvider localizationProvider, ILookupService lookupService)
        {
            _localizationProvider = localizationProvider ?? throw new ArgumentNullException(nameof(localizationProvider));
            _lookupService = lookupService ?? throw new ArgumentNullException(nameof(lookupService));
        }

        public virtual string FormatValue(ProductPropertyDefinition property, object value, CultureInfo cultureInfo)
        {
            string valueText;
            switch (property.DataType)
            {
                case PropertyDataType.Boolean:
                    if (value is bool boolValue)
                    {
                        if (boolValue)
                        {
                            valueText = FormatValueByType(value, string.Format("{0}True", property.PropertyName),
                                BasePropertyTypeBooleanTrueResourceKey, cultureInfo);
                        }
                        else
                        {
                            valueText = FormatValueByType(value, string.Format("{0}False", property.PropertyName),
                                BasePropertyTypeBooleanFalseResourceKey, cultureInfo);
                        }
                    }
                    else
                    {
                        valueText = FormatValueByType(value, property.PropertyName, BasePropertyTypeTextResourceKey,
                            cultureInfo);
                    }
                    break;
                case PropertyDataType.Currency:
                    valueText = FormatValueByType(value, property.PropertyName, BasePropertyTypeCurrencyResourceKey, cultureInfo);
                    break;
                case PropertyDataType.Decimal:
                    valueText = FormatValueByType(value, property.PropertyName, BasePropertyTypeDecimalResourceKey, cultureInfo);
                    break;
                case PropertyDataType.Number:
                    valueText = FormatValueByType(value, property.PropertyName, BasePropertyTypeNumberResourceKey, cultureInfo);
                    break;
                case PropertyDataType.DateTime:
                    if (value is DateTime dateTimeValue)
                    {
                        valueText = dateTimeValue.ToShortDateString();
                    }
                    else
                    {
                        valueText = value.ToString();
                    }
                    valueText = FormatValueByType(valueText, property.PropertyName, BasePropertyTypeDateTimeResourceKey, cultureInfo);
                    break;
                case PropertyDataType.Text:
                    if (property.Localizable && value is LocalizedString localizedStringValue)
                    {
                        valueText = FormatValueByType(localizedStringValue.GetLocalizedValue(cultureInfo.Name),
                            property.PropertyName, BasePropertyTypeTextResourceKey, cultureInfo);
                    }
                    else
                    {
                        valueText = FormatValueByType(value, property.PropertyName, BasePropertyTypeTextResourceKey,
                               cultureInfo);
                    }
                    break;
                case PropertyDataType.Lookup:
                    var param = new GetLookupDisplayNameParam
                    {
                        CultureInfo = cultureInfo,
                        // since this isn't ViewModel mapping, we con't have access to the delimiter declared as an Attribute
                        Delimiter = SpecificationConfiguration.MultiValueLookupSeparator + " ",
                        LookupType = LookupType.Product,
                        LookupName = property.LookupDefinition.LookupName,
                        Value = value.ToString()
                    };
                    // call this synchronously to avoid async calls up the stack
                    // the request is cached, so this shouldn't be a performance burden
                    valueText = _lookupService.GetLookupDisplayNameAsync(param).Result;

                    valueText = FormatValueByType(valueText, property.PropertyName, BasePropertyTypeLookupResourceKey, cultureInfo);
                    break;
                default:
                    valueText = FormatValueByType(value, property.PropertyName, BasePropertyTypeTextResourceKey, cultureInfo);
                    break;
            }
            return valueText;
        }

        private string FormatValueByType(object value, string propertyName, string baseTypeFormatting, CultureInfo cultureInfo)
        {
            var formatting = _localizationProvider.GetLocalizedString(new GetLocalizedParam 
            {
                Category    = PropertyFormattingResource, 
                Key         = propertyName, 
                CultureInfo = cultureInfo
            });

            if (formatting == null)
            {
                formatting = _localizationProvider.GetLocalizedString(new GetLocalizedParam
                {
                    Category    = PropertyFormattingResource, 
                    Key         = baseTypeFormatting, 
                    CultureInfo = cultureInfo
                });
            }
            if (formatting == null)
            {
                formatting = "{0}";
            }
            return string.Format(formatting, value);
        }
    }
}