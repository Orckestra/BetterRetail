using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Overture;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Metadata;
using Orckestra.Overture.ServiceModel.Products;
using Orckestra.Overture.ServiceModel.Requests.Metadata;
using Orckestra.Overture.ServiceModel.Requests.Products;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.Helper
{
    public class ProductHelper
    {


        public static async Task<List<KeyVariantAttributes>> GetKeyVariantAttributes(Orckestra.Overture.ServiceModel.Products.Product product, Variant variant, CultureInfo culture, IOvertureClient client)
        {
            if (variant == null)
                return null;

            var request = new GetProductDefinitionRequest()
            {
                CultureName = culture.Name,
                Name = product.DefinitionName,
            };

            var productDef = client.Send(request);

            var lookups = await GetLookups(productDef, client).ConfigureAwaitWithCulture(false);

            if (variant.PropertyBag == null) return null;

            var keyVariantAttributes = productDef.VariantProperties.Where(x => x.IsKeyVariant)
                                                        .OrderBy(x => x.KeyVariantOrder)
                                                        .ToList();

            if (!keyVariantAttributes.Any()) return null;

            var list = new List<KeyVariantAttributes>();

            foreach (var keyVariantAttribute in keyVariantAttributes)
            {
                object kvaValue;
                if (keyVariantAttribute.DataType.Equals(PropertyDataType.Lookup))
                {
                    var lookup =
                        lookups.SingleOrDefault(l => l.LookupName == keyVariantAttribute.LookupDefinition.LookupName);
                    kvaValue = GetLocalizedKvaDisplayValueFromLookup(lookup, culture.Name, variant, keyVariantAttribute);
                }
                else
                {
                    kvaValue = GetLocalizedKvaDisplayValueFromValue(culture.Name, variant, keyVariantAttribute);
                }

                if (kvaValue != null)
                {
                    //var displayValue = kvaValue.Value;
                    list.Add(new KeyVariantAttributes()
                    {
                        Key = keyVariantAttribute.PropertyName,
                        Value = "TODO",//LineItemHelper.GetVariantValue(kvaValue.ToString(), keyVariantAttribute.PropertyName),
                        OriginalValue = variant.PropertyBag[keyVariantAttribute.PropertyName].ToString()
                    });
                }
            }

            return list;
        }

        private static object GetLocalizedKvaDisplayValueFromValue(string cultureName, Variant variant, ProductPropertyDefinition keyVariantAttribute)
        {
            var value = variant.PropertyBag[keyVariantAttribute.PropertyName];
            var localized = value as LocalizedString;

            if (localized == null)
            {
                return value;
            }

            var localizedValue = localized.GetLocalizedValue(cultureName);

            if (!string.IsNullOrWhiteSpace(localizedValue))
            {
                return localizedValue;
            }

            return variant.PropertyBag[keyVariantAttribute.PropertyName];
        }

        private static string GetLocalizedKvaDisplayValueFromLookup(Lookup lookup, string cultureName, Variant variant, ProductPropertyDefinition keyVariantAttribute)
        {

            if (lookup == null)
            {
                return variant.PropertyBag[keyVariantAttribute.PropertyName] as string;
            }

            var firstOrDefault =
                lookup.Values.FirstOrDefault(x => x.Value.Equals(variant.PropertyBag[keyVariantAttribute.PropertyName]));

            if (firstOrDefault == null || firstOrDefault.DisplayName == null)
            {
                return variant.PropertyBag[keyVariantAttribute.PropertyName] as string;
            }

            var localizedValue = firstOrDefault.DisplayName.GetLocalizedValue(cultureName);

            return !string.IsNullOrWhiteSpace(localizedValue) ? localizedValue : firstOrDefault.Value;
        }

        private static async Task<List<Lookup>> GetLookups(ProductDefinition productDef, IOvertureClient client)
        {
            var kvaLookupAttributes = productDef.VariantProperties.Where(vp => vp.IsKeyVariant && vp.DataType == PropertyDataType.Lookup)
                                                         .Select(vp => vp.LookupDefinition.LookupName);

            var results = new List<Lookup>();
            var lookupTasks = new List<Task<Lookup>>();

            foreach (var name in kvaLookupAttributes.Distinct())
            {
                var request = new GetProductLookupRequest
                {
                    LookupName = name
                };

                lookupTasks.Add(client.SendAsync(request));
            }

            var lookupResults = await Task.WhenAll(lookupTasks).ConfigureAwaitWithCulture(false);
            foreach (var lookup in lookupResults)
            {
                if (lookup != null)
                {
                    results.Add(lookup);
                }
            }

            return results;
        }
    }
}
