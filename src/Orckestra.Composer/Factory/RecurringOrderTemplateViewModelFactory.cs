using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Country;
using Orckestra.Composer.Extensions;
using Orckestra.Composer.Helper;
using Orckestra.Composer.Parameters;
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
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Products;
using Orckestra.Overture.ServiceModel.Requests.Customers;
using Orckestra.Overture.ServiceModel.Requests.Metadata;
using Orckestra.Overture.ServiceModel.Requests.RecurringOrders;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Factory
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
        protected IProductUrlProvider ProductUrlProvider { get; private set; }
        protected IProductPriceViewService ProductPriceViewService { get; private set; }
        protected IOvertureClient OvertureClient { get; private set; }
        protected IRecurringScheduleUrlProvider RecurringScheduleUrlProvider { get; private set; }
        protected IRecurringOrderProgramViewModelFactory RecurringOrderProgramViewModelFactory { get; private set; }
        protected IRecurringOrdersRepository RecurringOrderRepository { get; private set; }


        public RecurringOrderTemplateViewModelFactory(
            IOvertureClient overtureClient,
            ILocalizationProvider localizationProvider,
            IViewModelMapper viewModelMapper,
            ICountryService countryService,
            IComposerContext composerContext,
            IRecurringOrdersRepository recurringOrdersRepository,
            IAddressRepository addressRepository,
            IProductUrlProvider productUrlProvider,
            IProductPriceViewService productPriceViewService,
            IRecurringScheduleUrlProvider recurringScheduleUrlProvider,
            IRecurringOrderProgramViewModelFactory recurringOrderProgramViewModelFactory,
            IRecurringOrdersRepository recurringOrderRepository)
        {
            LocalizationProvider = localizationProvider ?? throw new ArgumentNullException(nameof(localizationProvider));
            ViewModelMapper = viewModelMapper ?? throw new ArgumentNullException(nameof(viewModelMapper));
            CountryService = countryService ?? throw new ArgumentNullException(nameof(countryService));
            ComposerContext = composerContext;
            RecurringOrdersRepository = recurringOrdersRepository ?? throw new ArgumentNullException(nameof(recurringOrdersRepository));
            AddressRepository = addressRepository ?? throw new ArgumentNullException(nameof(addressRepository));
            ProductUrlProvider = productUrlProvider ?? throw new ArgumentNullException(nameof(productUrlProvider));
            ProductPriceViewService = productPriceViewService ?? throw new ArgumentNullException(nameof(productPriceViewService));
            OvertureClient = overtureClient ?? throw new ArgumentNullException(nameof(overtureClient));
            RecurringScheduleUrlProvider = recurringScheduleUrlProvider ?? throw new ArgumentNullException(nameof(recurringScheduleUrlProvider));
            RecurringOrderProgramViewModelFactory = recurringOrderProgramViewModelFactory ?? throw new ArgumentNullException(nameof(recurringOrderProgramViewModelFactory));
            RecurringOrderRepository = recurringOrderRepository ?? throw new ArgumentNullException(nameof(recurringOrderRepository));
        }

        public virtual async Task<RecurringOrderTemplatesViewModel> CreateRecurringOrderTemplatesViewModel(CreateRecurringOrderTemplatesViewModelParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (param.ProductImageInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.ProductImageInfo)), nameof(param)); }
            if (param.ProductImageInfo.ImageUrls == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.ProductImageInfo.ImageUrls)), nameof(param)); }
            if (param.ScopeId == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.ScopeId)), nameof(param)); }

            var vm = new RecurringOrderTemplatesViewModel
            {
                RecurringOrderTemplateViewModelList = await CreateTemplateGroupedShippingAddress(new CreateTemplateGroupedShippingAddressParam
                {
                    ListOfRecurringOrderLineItems = param.ListOfRecurringOrderLineItems,
                    CultureInfo = param.CultureInfo,
                    ProductImageInfo = param.ProductImageInfo,
                    BaseUrl = param.BaseUrl,
                    CustomerId = param.CustomerId,
                    ScopeId = param.ScopeId
                }).ConfigureAwait(false)
            };

            foreach (var template in vm.RecurringOrderTemplateViewModelList)
            {
                await MapRecurringOrderLineitemFrequencyName(template, param.CultureInfo);
            }

            return vm;
        }

        public virtual async Task<RecurringOrderTemplateViewModel> CreateRecurringOrderTemplateDetailsViewModel(CreateRecurringOrderTemplateDetailsViewModelParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (param.ProductImageInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.ProductImageInfo)), nameof(param)); }
            if (param.ProductImageInfo.ImageUrls == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.ProductImageInfo.ImageUrls)), nameof(param)); }
            if (param.ScopeId == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.ScopeId)), nameof(param)); }
            if (param.RecurringOrderLineItem == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.RecurringOrderLineItem)), nameof(param)); }

            var vm = new RecurringOrderTemplateViewModel();
            
            var imgDictionary = LineItemHelper.BuildImageDictionaryFor(param.ProductImageInfo.ImageUrls);

            var recurringScheduleUrl = RecurringScheduleUrlProvider.GetRecurringScheduleUrl(new GetRecurringScheduleUrlParam
            {
                CultureInfo = param.CultureInfo
            });
            
            var lineItemViewModel = await MapToTemplateLineItemViewModel(new MapToTemplateLineItemViewModelParam
            {
                RecurringOrderlineItem = param.RecurringOrderLineItem,
                CultureInfo = param.CultureInfo,
                ImageDictionnary = imgDictionary,
                BaseUrl = param.BaseUrl,
                RecurringScheduleUrl = recurringScheduleUrl
            }).ConfigureAwait(false);

            if (lineItemViewModel != null)
            {
                lineItemViewModel.ShippingAddressId = param.RecurringOrderLineItem.ShippingAddressId;
                vm.RecurringOrderTemplateLineItemViewModels.Add(lineItemViewModel);
            }

            return vm;
        }


        public virtual async Task<List<RecurringOrderTemplateViewModel>> CreateTemplateGroupedShippingAddress(CreateTemplateGroupedShippingAddressParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.ListOfRecurringOrderLineItems == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.ListOfRecurringOrderLineItems)), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (param.ProductImageInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.ProductImageInfo)), nameof(param)); }
            if (param.ProductImageInfo.ImageUrls == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.ProductImageInfo.ImageUrls)), nameof(param)); }
            if (param.ScopeId == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.ScopeId)), nameof(param)); }

            var groups = param.ListOfRecurringOrderLineItems.RecurringOrderLineItems.GroupBy(grp => grp.ShippingAddressId);

            var imgDictionary = LineItemHelper.BuildImageDictionaryFor(param.ProductImageInfo.ImageUrls);

            var itemList = new List<RecurringOrderTemplateViewModel>();

            var recurringScheduleUrl = RecurringScheduleUrlProvider.GetRecurringScheduleUrl(new GetRecurringScheduleUrlParam
            {
                CultureInfo = param.CultureInfo,
            });  

            foreach (var group in groups)
            {
                var templateViewModel = new RecurringOrderTemplateViewModel
                {
                    ShippingAddress = await MapShippingAddress(group.Key, param.CultureInfo).ConfigureAwait(false)
                };

                var tasks = group.Select(g => MapToTemplateLineItemViewModel(new MapToTemplateLineItemViewModelParam
                {
                    RecurringOrderlineItem =  g,
                    CultureInfo = param.CultureInfo,
                    ImageDictionnary =  imgDictionary,
                    BaseUrl = param.BaseUrl,
                    RecurringScheduleUrl = recurringScheduleUrl
                }));

                var templateLineItems = await Task.WhenAll(tasks);

                //Filter null to not have an error when rendering the page
                templateViewModel.RecurringOrderTemplateLineItemViewModels.AddRange(templateLineItems.Where(t => t != null).ToList());
                
                itemList.Add(templateViewModel);
            }

            return itemList;
        }

        //TODO: rename to GetCustomerAsync if used, directly return customer without await
        protected virtual async Task<Customer> GetCustomer(Guid customerId, string scopeId)
        {
            var getCustomerRequest = new GetCustomerRequest
            {
                CustomerId = customerId,
                ScopeId = scopeId
            };

            var customer = await OvertureClient.SendAsync(getCustomerRequest).ConfigureAwait(false);

            return customer;
        }

        public virtual async Task<RecurringOrderTemplateLineItemViewModel> MapToTemplateLineItemViewModel(MapToTemplateLineItemViewModelParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.RecurringOrderlineItem == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.RecurringOrderlineItem)), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (param.BaseUrl == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.BaseUrl)), nameof(param)); }
            if (param.RecurringScheduleUrl == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.RecurringScheduleUrl)), nameof(param)); }

            var recrurringLineItem = param.RecurringOrderlineItem;

            var vm = ViewModelMapper.MapTo<RecurringOrderTemplateLineItemViewModel>(recrurringLineItem, param.CultureInfo);

            if (vm.IsValid == null)
            {
                vm.IsValid = true;
            }

            if (param.ImageDictionnary.TryGetValue(Tuple.Create(recrurringLineItem.ProductId, recrurringLineItem.VariantId), out ProductMainImage mainImage))
            {
                vm.ImageUrl = mainImage.ImageUrl;
                vm.FallbackImageUrl = mainImage.FallbackImageUrl;
            }

            var getProductRequest = new Overture.ServiceModel.Requests.Products.GetProductRequest
            {
                ProductId = recrurringLineItem.ProductId,
                ScopeId = recrurringLineItem.ScopeId,
                CultureName = param.CultureInfo.Name,
                IncludePriceLists = true,
                IncludeRelationships = false,
                IncludeVariants = true
            };

            var getProductResponse = await OvertureClient.SendAsync(getProductRequest).ConfigureAwait(false);

            if (getProductResponse == null || (getProductResponse != null && recrurringLineItem.VariantId != string.Empty
                && recrurringLineItem.VariantId != null 
                && getProductResponse.Variants.SingleOrDefault(v => v.Id == recrurringLineItem.VariantId) == null))
            {
                var deleteRecurringLineItem = new DeleteRecurringOrderLineItemsRequest
                {
                    CustomerId = recrurringLineItem.CustomerId,
                    RecurringOrderLineItemIds = new List<Guid> { recrurringLineItem.RecurringOrderLineItemId },
                    ScopeId = recrurringLineItem.ScopeId
                };
                await OvertureClient.SendAsync(deleteRecurringLineItem);

                return await Task.FromResult<RecurringOrderTemplateLineItemViewModel>(null);
            }

            var variant = getProductResponse.Variants.SingleOrDefault(v => v.Id == recrurringLineItem.VariantId);

            vm.FormattedNextOccurence = vm.NextOccurence == DateTime.MinValue
                    ? string.Empty
                    : string.Format(param.CultureInfo, "{0:D}", vm.NextOccurence);

            vm.NextOccurenceValue = vm.NextOccurence == DateTime.MinValue
                    ? string.Empty
                    : vm.NextOccurence.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);

            vm.Id = recrurringLineItem.RecurringOrderLineItemId;
            vm.ProductSummary = new RecurringProductSummaryViewModel
            {
                DisplayName = ProductHelper.GetProductOrVariantDisplayName(getProductResponse, variant, param.CultureInfo)
            };

            var productsPricesVm = await ProductPriceViewService.CalculatePricesAsync(new GetProductsPriceParam
            {
                CultureInfo = param.CultureInfo,
                Scope = recrurringLineItem.ScopeId,
                ProductIds = new List<string>() { recrurringLineItem.ProductId }
            });
            
            var productPriceVm = productsPricesVm.ProductPrices.SingleOrDefault(p => p.ProductId == recrurringLineItem.ProductId);
            if (productPriceVm != null)
            {
                var variantPriceVm = productPriceVm.VariantPrices.SingleOrDefault(v => v.VariantId == recrurringLineItem.VariantId);
                if (variantPriceVm != null)
                {
                    vm.DefaultListPrice = variantPriceVm.DefaultListPrice;
                    vm.ListPrice = variantPriceVm.ListPrice;
                }
                else
                {
                    vm.DefaultListPrice = productPriceVm.DefaultListPrice;
                    vm.ListPrice = productPriceVm.ListPrice;
                }
            }
            vm.IsOnSale = string.CompareOrdinal(vm.DefaultListPrice, vm.ListPrice) != 0;

            var conv = decimal.TryParse(vm.ListPrice, NumberStyles.Currency, param.CultureInfo.NumberFormat, out decimal price);
            if (conv)
            {
                vm.TotalWithoutDiscount = LocalizationProvider.FormatPrice((decimal)vm.Quantity * price, param.CultureInfo);

                vm.Total = LocalizationProvider.FormatPrice((decimal)vm.Quantity * price, param.CultureInfo);
            }

            //Adding brand display name
            var brandLookup = await OvertureClient.SendAsync(new GetProductLookupRequest { LookupName = "Brand" });
            var brandId = getProductResponse.Brand;

            if (brandId != null)
            {
                string brandValue = Convert.ToString(brandId);
                vm.ProductSummary.Brand = brandLookup?.GetDisplayName(brandValue, param.CultureInfo.Name) ?? brandId;
            }

            var list = await ProductHelper.GetKeyVariantAttributes(getProductResponse, variant, param.CultureInfo, OvertureClient);
            if (list != null && list.Count > 0)
            {
                vm.KeyVariantAttributesList = list.ToList();
            }

            vm.ShippingMethodName = recrurringLineItem.FulfillmentMethodName;

            vm.ProductUrl = ProductUrlProvider.GetProductUrl(new GetProductUrlParam
            {
                CultureInfo = param.CultureInfo,
                VariantId = recrurringLineItem.VariantId,
                ProductId = recrurringLineItem.ProductId,
                ProductName = vm.ProductSummary.DisplayName,
                SKU = recrurringLineItem.Sku
            });

            var recurringScheduleEditUrl = RecurringScheduleUrlProvider.GetRecurringScheduleDetailsUrl(new GetRecurringScheduleDetailsUrlParam
            {
                CultureInfo = param.CultureInfo,
                RecurringScheduleId = vm.Id.ToString()
            });
            
            vm.EditUrl = recurringScheduleEditUrl;
            vm.ScheduleUrl = param.RecurringScheduleUrl;

            var program = await RecurringOrderRepository.GetRecurringOrderProgram(recrurringLineItem.ScopeId, recrurringLineItem.RecurringOrderProgramName);
            var programViewModel = RecurringOrderProgramViewModelFactory.CreateRecurringOrderProgramViewModel(program, param.CultureInfo);
            vm.RecurringOrderProgramFrequencies = programViewModel?.Frequencies;

            return vm;
        }

        protected virtual string GetProductOrVariantListPrice(Orckestra.Overture.ServiceModel.Products.Product product, Variant variant, CultureInfo culture)
        {
            return variant != null
                ? LocalizationProvider.FormatPrice(variant.ListPrice.Value, culture)
                : LocalizationProvider.FormatPrice(product.ListPrice.Value, culture);
        }

        //TODO: rename to MapShippingAddressAsync if used
        protected virtual async Task<RecurringOrderTemplateAddressViewModel> MapShippingAddress(Guid shippingAddressId, CultureInfo culture)
        {
            var address = await AddressRepository.GetAddressByIdAsync(shippingAddressId).ConfigureAwait(false);

            return GetAddressViewModel(address, culture);
        }

        /// <summary>
        /// Map the address of the client
        /// </summary>
        /// <param name="address"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public virtual RecurringOrderTemplateAddressViewModel GetAddressViewModel(Address address, CultureInfo cultureInfo)
        {
            if (address == null) { return new RecurringOrderTemplateAddressViewModel(); }

            var addressViewModel = ViewModelMapper.MapTo<RecurringOrderTemplateAddressViewModel>(address, cultureInfo);

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
                            var frequency = program.Frequencies.Find(f => string.Equals(f.RecurringOrderFrequencyName, lineitem.RecurringOrderFrequencyName, StringComparison.OrdinalIgnoreCase));

                            if (frequency != null)
                            {
                                var localization = frequency.Localizations.Find(l => string.Equals(l.CultureIso, culture.Name, StringComparison.OrdinalIgnoreCase));

                                lineitem.RecurringOrderFrequencyDisplayName = localization != null ? localization.DisplayName : frequency.RecurringOrderFrequencyName;
                            }
                        }
                        var programViewModel = RecurringOrderProgramViewModelFactory.CreateRecurringOrderProgramViewModel(program, culture);
                        lineitem.RecurringOrderProgramFrequencies = programViewModel?.Frequencies;
                    }
                }
            }
        }


        public virtual RecurringOrderShippingMethodViewModel GetShippingMethodViewModel(FulfillmentMethodInfo fulfillmentMethodInfo, CultureInfo cultureInfo)
        {
            if (fulfillmentMethodInfo == null) { return null; }

            var shippingMethodViewModel = ViewModelMapper.MapTo<RecurringOrderShippingMethodViewModel>(fulfillmentMethodInfo, cultureInfo);

            return shippingMethodViewModel;
        }
    }
}