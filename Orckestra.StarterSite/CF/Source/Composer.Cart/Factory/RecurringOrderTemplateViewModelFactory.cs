using Orckestra.Composer.Cart.Helper;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories.Order;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Country;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Product.Factory;
using Orckestra.Composer.Product.Parameters;
using Orckestra.Composer.Product.Requests;
using Orckestra.Composer.Product.Services;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Customers;
using Orckestra.Overture.ServiceModel.Metadata;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Products;
using Orckestra.Overture.ServiceModel.RecurringOrders;
using Orckestra.Overture.ServiceModel.Requests.Customers;
using Orckestra.Overture.ServiceModel.Requests.Products;
using Orckestra.Overture.ServiceModel.Requests.RecurringOrders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.Factory
{
    public class RecurringOrderTemplateViewModelFactory : IRecurringOrderTemplateViewModelFactory
    {

        private RetrieveCountryParam _countryParam;
        protected RetrieveCountryParam CountryParam
        {
            get
            {
                return _countryParam ?? (_countryParam = new RetrieveCountryParam
                {
                    CultureInfo = ComposerContext.CultureInfo,
                    IsoCode = ComposerContext.CountryCode
                });
            }
        }

        protected ILocalizationProvider LocalizationProvider { get; private set; }
        protected IViewModelMapper ViewModelMapper { get; private set; }
        protected ICountryService CountryService { get; private set; }
        protected IComposerContext ComposerContext { get; private set; }
        protected IRecurringOrdersRepository RecurringOrdersRepository { get; private set; }
        protected IAddressRepository AddressRepository { get; private set; }
        protected IProductViewModelFactory ProductViewModelFactory { get; private set; }
        protected IProductUrlProvider ProductUrlProvider { get; private set; }
        protected IProductPriceViewService ProductPriceViewService { get; private set; }
        protected IOvertureClient OvertureClient { get; private set; }

        public RecurringOrderTemplateViewModelFactory(
            IOvertureClient overtureClient,
            ILocalizationProvider localizationProvider,
            IViewModelMapper viewModelMapper,
            ICountryService countryService,
            IComposerContext composerContext,
            IRecurringOrdersRepository recurringOrdersRepository,
            IAddressRepository addressRepository,
            IProductViewModelFactory productViewModelFactory,
            IProductUrlProvider productUrlProvider,
            IProductPriceViewService productPriceViewService)
        {
            if (overtureClient == null) { throw new ArgumentNullException(nameof(overtureClient)); }
            if (localizationProvider == null) { throw new ArgumentNullException(nameof(localizationProvider)); }
            if (viewModelMapper == null) { throw new ArgumentNullException(nameof(viewModelMapper)); }
            if (countryService == null) { throw new ArgumentNullException(nameof(countryService)); }
            if (recurringOrdersRepository == null) { throw new ArgumentNullException(nameof(recurringOrdersRepository)); }
            if (addressRepository == null) { throw new ArgumentNullException(nameof(addressRepository)); }
            if (productViewModelFactory == null) { throw new ArgumentNullException(nameof(productViewModelFactory)); }
            if (productUrlProvider == null) { throw new ArgumentNullException(nameof(productUrlProvider)); }
            if (productPriceViewService == null) { throw new ArgumentNullException(nameof(productPriceViewService)); }

            LocalizationProvider = localizationProvider;
            ViewModelMapper = viewModelMapper;
            CountryService = countryService;
            ComposerContext = composerContext;
            RecurringOrdersRepository = recurringOrdersRepository;
            AddressRepository = addressRepository;
            ProductViewModelFactory = productViewModelFactory;
            ProductUrlProvider = productUrlProvider;
            ProductPriceViewService = productPriceViewService;
            OvertureClient = overtureClient;
        }

        public async Task<RecurringOrderTemplatesViewModel> CreateRecurringOrderTemplatesViewModel(CreateRecurringOrderTemplatesViewModelParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentNullException(nameof(param.CultureInfo)); }
            if (param.ProductImageInfo == null) { throw new ArgumentNullException(nameof(param.ProductImageInfo)); }
            if (param.ProductImageInfo.ImageUrls == null) { throw new ArgumentNullException(nameof(param.ProductImageInfo.ImageUrls)); }
            if (param.ScopeId == null) { throw new ArgumentNullException(nameof(param.ScopeId)); }

            var vm = new RecurringOrderTemplatesViewModel();

            await CreateTemplateGroupedShippingAddress(param.ListOfRecurringOrderLineItems, vm, param.CultureInfo, param.ProductImageInfo, param.BaseUrl, param.CustomerId, param.ScopeId).ConfigureAwaitWithCulture(false);


            foreach (var template in vm.RecurringOrderTemplateViewModelList)
            {
                await MapRecurringOrderLineitemFrequencyName(template, param.CultureInfo).ConfigureAwaitWithCulture(false);
            }

            return vm;
        }


        /// <summary>
        /// Quick Access lookup for images
        /// Group by Product then by VariantId
        /// </summary>
        /// <returns></returns>
        protected virtual IDictionary<Tuple<string, string>, ProductMainImage> BuildImageDictionaryFor(IList<ProductMainImage> images)
        {
            return images == null
                ? new Dictionary<Tuple<string, string>, ProductMainImage>()
                : images.GroupBy(image => Tuple.Create(image.ProductId, image.VariantId))
                .ToDictionary(img => img.Key, img => img.FirstOrDefault());
        }

        private async Task CreateTemplateGroupedShippingAddress(ListOfRecurringOrderLineItems list, RecurringOrderTemplatesViewModel vm, CultureInfo culture, ProductImageInfo imageInfo, string baseUrl, Guid customerId, string scopeId)
        {

            var groups = list.RecurringOrderLineItems.GroupBy(grp => grp.ShippingAddressId);

            var imgDictionary = BuildImageDictionaryFor(imageInfo.ImageUrls);

            var customer = await GetCustomer(customerId, scopeId).ConfigureAwaitWithCulture(false);

            foreach (var group in groups)
            {
                var templateViewModel = new RecurringOrderTemplateViewModel();

                templateViewModel.ShippingAddress = await MapShippingAddress(group.Key, culture).ConfigureAwaitWithCulture(false);

                var tasks = group.Select(g => MapToTemplateLineItemViewModel(g, culture, imgDictionary, baseUrl, customer));
                var templateLineItems = await Task.WhenAll(tasks).ConfigureAwaitWithCulture(false);

                //Filter null to not have an error when rendering the page
                templateViewModel.RecurringOrderTemplateLineItemViewModels.AddRange(templateLineItems.Where(t => t != null).ToList());

                vm.RecurringOrderTemplateViewModelList.Add(templateViewModel);
            }
        }

        private async Task<Customer> GetCustomer(Guid customerId, string scopeId)
        {
            var getCustomerRequest = new GetCustomerRequest
            {
                CustomerId = customerId,
                ScopeId = scopeId
            };

            var customer = await OvertureClient.SendAsync(getCustomerRequest).ConfigureAwaitWithCulture(false);

            return customer;
        }

       

        private async Task<RecurringOrderTemplateLineItemViewModel> MapToTemplateLineItemViewModel(RecurringOrderLineItem lineItem, CultureInfo culture, IDictionary<Tuple<string, string>, ProductMainImage> imgDictionary, string baseUrl, Customer customer)
        {
            var vm = ViewModelMapper.MapTo<RecurringOrderTemplateLineItemViewModel>(lineItem, culture);

            if (vm.IsValid == null)
            {
                vm.IsValid = true;
            }

            ProductMainImage mainImage;
            if (imgDictionary.TryGetValue(Tuple.Create(lineItem.ProductId, lineItem.VariantId), out mainImage))
            {
                vm.ImageUrl = mainImage.ImageUrl;
                vm.FallbackImageUrl = mainImage.FallbackImageUrl;
            }

            var getProductRequest = new Orckestra.Overture.ServiceModel.Requests.Products.GetProductRequest
            {
                ProductId = lineItem.ProductId,
                ScopeId = lineItem.ScopeId,
                CultureName = culture.Name,
                IncludePriceLists = true,
                IncludeRelationships = false,
                IncludeVariants = true
            };
            var getProductResponse = OvertureClient.Send(getProductRequest);

            if (getProductResponse == null ||
                (getProductResponse != null && lineItem.VariantId != string.Empty
                && lineItem.VariantId != null && getProductResponse.Variants.SingleOrDefault(v => v.Id == lineItem.VariantId) == null))
            {
                var deleteRecurringLineItem = new DeleteRecurringOrderLineItemsRequest
                {
                    CustomerId = lineItem.CustomerId,
                    RecurringOrderLineItemIds = new List<Guid> {
                        lineItem.RecurringOrderLineItemId,
                    },
                    ScopeId = lineItem.ScopeId
                };
                OvertureClient.Send(deleteRecurringLineItem);

                return await Task.FromResult<RecurringOrderTemplateLineItemViewModel>(null).ConfigureAwaitWithCulture(false);
            }

            var variant = getProductResponse.Variants.SingleOrDefault(v => v.Id == lineItem.VariantId);

            vm.FormattedNextOccurence = vm.NextOccurence == DateTime.MinValue
                    ? string.Empty
                    : string.Format(culture, "{0:D}", vm.NextOccurence);

            vm.Id = lineItem.RecurringOrderLineItemId;
            vm.ProductSummary = new CartProductSummaryViewModel();
            vm.ProductSummary.DisplayName = Composer.Utils.ProductHelper.GetProductOrVariantDisplayName(getProductResponse, variant, culture);


            var productsPricesVm = await ProductPriceViewService.CalculatePricesAsync(new GetProductsPriceParam()
            {
                CultureInfo = culture,
                Scope = lineItem.ScopeId,
                ProductIds = new List<string>() { lineItem.ProductId }
            }).ConfigureAwaitWithCulture(false);

            vm.DefaultListPrice = GetProductOrVariantListPrice(getProductResponse, variant, culture);
            vm.ListPrice = vm.DefaultListPrice;
            var productPriceVm = productsPricesVm.ProductPrices.SingleOrDefault(p => p.ProductId == lineItem.ProductId);
            if (productPriceVm != null)
            {
                var variantPriceVm = productPriceVm.VariantPrices.SingleOrDefault(v => v.VariantId == lineItem.VariantId);
                if (variantPriceVm != null)
                {
                    vm.ListPrice = variantPriceVm.ListPrice;
                    vm.IsOnSale = string.CompareOrdinal(variantPriceVm.ListPrice, vm.ListPrice) != 0;
                }
            }

            decimal price;
            var conv = decimal.TryParse(vm.ListPrice, NumberStyles.Currency, culture.NumberFormat, out price);
            if (conv)
            {
                vm.TotalWithoutDiscount = LocalizationProvider.FormatPrice((decimal)vm.Quantity * price, culture);

                vm.Total = LocalizationProvider.FormatPrice((decimal)vm.Quantity * price, culture);
            }

            vm.ProductSummary.Brand = getProductResponse.Brand;
            //var list = await GetKeyVariantAttributes(getProductResponse, variant, culture, OvertureClient).ConfigureAwaitWithCulture(false);
            //if (list != null && list.Count > 0)
            //{
            //    vm.KeyVariantAttributesList = list.ToList();
            //}

            vm.ShippingMethodName = lineItem.FulfillmentMethodName;

            vm.ProductUrl = ProductUrlProvider.GetProductUrl(new GetProductUrlParam
            {
                CultureInfo = culture,
                VariantId = lineItem.VariantId,
                ProductId = lineItem.ProductId,
                ProductName = vm.ProductSummary.DisplayName
            });

            return vm;
        }

        private string GetProductOrVariantListPrice(Orckestra.Overture.ServiceModel.Products.Product product, Variant variant, CultureInfo culture)
        {
            if (variant != null)
            {
                return LocalizationProvider.FormatPrice(variant.ListPrice.Value, culture);
            }
            else
            {
                return LocalizationProvider.FormatPrice(product.ListPrice.Value, culture);
            }
        }

        private async Task<AddressViewModel> MapShippingAddress(Guid shippingAddressId, CultureInfo culture)
        {
            var address = await AddressRepository.GetAddressByIdAsync(shippingAddressId).ConfigureAwaitWithCulture(false);

            return GetAddressViewModel(address, culture);
        }

        /// <summary>
        /// Map the address of the client
        /// </summary>
        public virtual AddressViewModel GetAddressViewModel(Address address, CultureInfo cultureInfo)
        {
            if (address == null)
            {
                return new AddressViewModel();
            }

            var addressViewModel = ViewModelMapper.MapTo<AddressViewModel>(address, cultureInfo);

            var regionName = CountryService.RetrieveRegionDisplayNameAsync(new RetrieveRegionDisplayNameParam
            {
                CultureInfo = cultureInfo,
                IsoCode = ComposerContext.CountryCode,
                RegionCode = address.RegionCode
            }).Result;

            addressViewModel.RegionName = regionName;
            addressViewModel.PhoneNumber = LocalizationProvider.FormatPhoneNumber(address.PhoneNumber, cultureInfo);

            return addressViewModel;
        }

        public async virtual Task MapRecurringOrderLineitemFrequencyName(RecurringOrderTemplateViewModel template, CultureInfo culture)
        {
            if (template.RecurringOrderTemplateLineItemViewModels == null) { return; }

            var uniqueProgramNames = template.RecurringOrderTemplateLineItemViewModels
                                            .Select(x => x.RecurringOrderProgramName)
                                            .Where(l => !string.IsNullOrWhiteSpace(l))
                                            .Distinct(StringComparer.OrdinalIgnoreCase)
                                            .ToList();

            if (uniqueProgramNames.Count > 0)
            {
                var tasks = uniqueProgramNames.Select(programName => RecurringOrdersRepository.GetRecurringOrderProgram(ComposerContext.Scope, programName));
                var programs = await Task.WhenAll(tasks).ConfigureAwait(false);

                foreach (var lineitem in template.RecurringOrderTemplateLineItemViewModels)
                {
                    if (RecurringOrderTemplateHelper.IsRecurringOrderLineItemValid(lineitem))
                    {
                        var program = programs.FirstOrDefault(p => string.Equals(p.RecurringOrderProgramName, lineitem.RecurringOrderProgramName, StringComparison.OrdinalIgnoreCase));

                        if (program != null)
                        {
                            var frequency = program.Frequencies.FirstOrDefault(f => string.Equals(f.RecurringOrderFrequencyName, lineitem.RecurringOrderFrequencyName, StringComparison.OrdinalIgnoreCase));

                            if (frequency != null)
                            {
                                var localization = frequency.Localizations.FirstOrDefault(l => string.Equals(l.CultureIso, culture.Name, StringComparison.OrdinalIgnoreCase));

                                if (localization != null)
                                    lineitem.RecurringOrderFrequencyDisplayName = localization.DisplayName;
                                else
                                    lineitem.RecurringOrderFrequencyDisplayName = frequency.RecurringOrderFrequencyName;
                            }
                        }
                    }
                }
            }
        }


        public virtual RecurringOrderShippingMethodViewModel GetShippingMethodViewModel(FulfillmentMethodInfo fulfillmentMethodInfo, CultureInfo cultureInfo)
        {
            if (fulfillmentMethodInfo == null)
            {
                return null;
            }

            var shippingMethodViewModel = ViewModelMapper.MapTo<RecurringOrderShippingMethodViewModel>(fulfillmentMethodInfo, cultureInfo);

            return shippingMethodViewModel;
        }

        public static  List<KeyVariantAttributes> GetKeyVariantAttributes(Orckestra.Overture.ServiceModel.Products.Product product, Variant variant, CultureInfo culture, IOvertureClient client)
        {
            if (variant == null)
                return null;

            var request = new GetProductDefinitionRequest()
            {
                CultureName = culture.Name,
                Name = product.DefinitionName,
            };

            var productDef = client.Send(request);

            //var lookups = await GetLookups(productDef, client).ConfigureAwaitWithCulture(false);

            //if (variant.PropertyBag == null) return null;

            //var keyVariantAttributes = productDef.VariantProperties.Where(x => x.IsKeyVariant)
            //                                            .OrderBy(x => x.KeyVariantOrder)
            //                                            .ToList();

            //if (!keyVariantAttributes.Any()) return null;

            var list = new List<KeyVariantAttributes>();

            //foreach (var keyVariantAttribute in keyVariantAttributes)
            //{
            //    object kvaValue;
            //    if (keyVariantAttribute.DataType.Equals(PropertyDataType.Lookup))
            //    {
            //        var lookup =
            //            lookups.SingleOrDefault(l => l.LookupName == keyVariantAttribute.LookupDefinition.LookupName);
            //        kvaValue = GetLocalizedKvaDisplayValueFromLookup(lookup, culture.Name, variant, keyVariantAttribute);
            //    }
            //    else
            //    {
            //        kvaValue = GetLocalizedKvaDisplayValueFromValue(culture.Name, variant, keyVariantAttribute);
            //    }

            //    if (kvaValue != null)
            //    {
            //        //var displayValue = kvaValue.Value;
            //        list.Add(new KeyVariantAttributes()
            //        {
            //            Key = keyVariantAttribute.PropertyName,
            //            Value = "TODO"//,LineItemHelper.GetVariantValue(kvaValue.ToString(), keyVariantAttribute.PropertyName),
            //            OriginalValue = variant.PropertyBag[keyVariantAttribute.PropertyName].ToString()
            //        });
            //    }
            //}

            return list;
        }
    }
}