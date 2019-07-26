using Orckestra.Composer.Country;
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
using Orckestra.Overture.ServiceModel.Metadata;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Products;
using Orckestra.Overture.ServiceModel.RecurringOrders;
using Orckestra.Overture.ServiceModel.Requests.Customers;
using Orckestra.Overture.ServiceModel.Requests.Products;
using Orckestra.Overture.ServiceModel.Requests.RecurringOrders;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if (overtureClient == null) { throw new ArgumentNullException(nameof(overtureClient)); }
            if (localizationProvider == null) { throw new ArgumentNullException(nameof(localizationProvider)); }
            if (viewModelMapper == null) { throw new ArgumentNullException(nameof(viewModelMapper)); }
            if (countryService == null) { throw new ArgumentNullException(nameof(countryService)); }
            if (recurringOrdersRepository == null) { throw new ArgumentNullException(nameof(recurringOrdersRepository)); }
            if (addressRepository == null) { throw new ArgumentNullException(nameof(addressRepository)); }
            if (productUrlProvider == null) { throw new ArgumentNullException(nameof(productUrlProvider)); }
            if (productPriceViewService == null) { throw new ArgumentNullException(nameof(productPriceViewService)); }
            if (recurringScheduleUrlProvider == null) { throw new ArgumentNullException(nameof(recurringScheduleUrlProvider)); }
            if (recurringOrderProgramViewModelFactory == null) { throw new ArgumentNullException(nameof(recurringOrderProgramViewModelFactory)); }
            if (recurringOrderRepository == null) { throw new ArgumentNullException(nameof(recurringOrderRepository)); }

            LocalizationProvider = localizationProvider;
            ViewModelMapper = viewModelMapper;
            CountryService = countryService;
            ComposerContext = composerContext;
            RecurringOrdersRepository = recurringOrdersRepository;
            AddressRepository = addressRepository;
            ProductUrlProvider = productUrlProvider;
            ProductPriceViewService = productPriceViewService;
            OvertureClient = overtureClient;
            RecurringScheduleUrlProvider = recurringScheduleUrlProvider;
            RecurringOrderProgramViewModelFactory = recurringOrderProgramViewModelFactory;
            RecurringOrderRepository = recurringOrderRepository;
        }

        public async Task<RecurringOrderTemplatesViewModel> CreateRecurringOrderTemplatesViewModel(CreateRecurringOrderTemplatesViewModelParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentNullException(nameof(param.CultureInfo)); }
            if (param.ProductImageInfo == null) { throw new ArgumentNullException(nameof(param.ProductImageInfo)); }
            if (param.ProductImageInfo.ImageUrls == null) { throw new ArgumentNullException(nameof(param.ProductImageInfo.ImageUrls)); }
            if (param.ScopeId == null) { throw new ArgumentNullException(nameof(param.ScopeId)); }

            var vm = new RecurringOrderTemplatesViewModel();

            vm.RecurringOrderTemplateViewModelList = await CreateTemplateGroupedShippingAddress(new CreateTemplateGroupedShippingAddressParam { 
                ListOfRecurringOrderLineItems = param.ListOfRecurringOrderLineItems,
                CultureInfo = param.CultureInfo,
                ProductImageInfo = param.ProductImageInfo,
                BaseUrl = param.BaseUrl,
                CustomerId = param.CustomerId,
                ScopeId = param.ScopeId}).ConfigureAwaitWithCulture(false);


            foreach (var template in vm.RecurringOrderTemplateViewModelList)
            {
                await MapRecurringOrderLineitemFrequencyName(template, param.CultureInfo).ConfigureAwaitWithCulture(false);
            }

            return vm;
        }

        public async Task<RecurringOrderTemplateViewModel> CreateRecurringOrderTemplateDetailsViewModel(CreateRecurringOrderTemplateDetailsViewModelParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentNullException(nameof(param.CultureInfo)); }
            if (param.ProductImageInfo == null) { throw new ArgumentNullException(nameof(param.ProductImageInfo)); }
            if (param.ProductImageInfo.ImageUrls == null) { throw new ArgumentNullException(nameof(param.ProductImageInfo.ImageUrls)); }
            if (param.ScopeId == null) { throw new ArgumentNullException(nameof(param.ScopeId)); }
            if (param.RecurringOrderLineItem == null) { throw new ArgumentNullException(nameof(param.RecurringOrderLineItem)); }

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
            }).ConfigureAwaitWithCulture(false);

            if (lineItemViewModel != null)
            {
                lineItemViewModel.ShippingAddressId = param.RecurringOrderLineItem.ShippingAddressId;

                vm.RecurringOrderTemplateLineItemViewModels.Add(lineItemViewModel);
            }

            return vm;
        }


        public async Task<List<RecurringOrderTemplateViewModel>> CreateTemplateGroupedShippingAddress(CreateTemplateGroupedShippingAddressParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.ListOfRecurringOrderLineItems == null) { throw new ArgumentNullException(nameof(param.ListOfRecurringOrderLineItems)); }
            if (param.CultureInfo == null) { throw new ArgumentNullException(nameof(param.CultureInfo)); }
            if (param.ProductImageInfo == null) { throw new ArgumentNullException(nameof(param.ProductImageInfo)); }
            if (param.ProductImageInfo.ImageUrls == null) { throw new ArgumentNullException(nameof(param.ProductImageInfo.ImageUrls)); }
            if (param.ScopeId == null) { throw new ArgumentNullException(nameof(param.ScopeId)); }

            var groups = param.ListOfRecurringOrderLineItems.RecurringOrderLineItems.GroupBy(grp => grp.ShippingAddressId);

            var imgDictionary = LineItemHelper.BuildImageDictionaryFor(param.ProductImageInfo.ImageUrls);

            var itemList = new List<RecurringOrderTemplateViewModel>();


            var recurringScheduleUrl = RecurringScheduleUrlProvider.GetRecurringScheduleUrl(new GetRecurringScheduleUrlParam
            {
                CultureInfo = param.CultureInfo,
            });  

            foreach (var group in groups)
            {
                var templateViewModel = new RecurringOrderTemplateViewModel();

                templateViewModel.ShippingAddress = await MapShippingAddress(group.Key, param.CultureInfo).ConfigureAwaitWithCulture(false);
                 
                var tasks = group.Select(g => MapToTemplateLineItemViewModel(new MapToTemplateLineItemViewModelParam
                {
                    RecurringOrderlineItem =  g,
                    CultureInfo = param.CultureInfo,
                    ImageDictionnary =  imgDictionary,
                    BaseUrl = param.BaseUrl,
                    RecurringScheduleUrl = recurringScheduleUrl
                }));
                var templateLineItems = await Task.WhenAll(tasks).ConfigureAwaitWithCulture(false);

                //Filter null to not have an error when rendering the page
                templateViewModel.RecurringOrderTemplateLineItemViewModels.AddRange(templateLineItems.Where(t => t != null).ToList());
                
                itemList.Add(templateViewModel);
            }

            return itemList;
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



        public async Task<RecurringOrderTemplateLineItemViewModel> MapToTemplateLineItemViewModel(MapToTemplateLineItemViewModelParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.RecurringOrderlineItem == null) { throw new ArgumentNullException(nameof(param.RecurringOrderlineItem)); }
            if (param.CultureInfo == null) { throw new ArgumentNullException(nameof(param.CultureInfo)); }
            if (param.BaseUrl == null) { throw new ArgumentNullException(nameof(param.BaseUrl)); }
            if (param.RecurringScheduleUrl == null) { throw new ArgumentNullException(nameof(param.RecurringScheduleUrl)); }

            var recrurringLineItem = param.RecurringOrderlineItem;

            var vm = ViewModelMapper.MapTo<RecurringOrderTemplateLineItemViewModel>(recrurringLineItem, param.CultureInfo);

            if (vm.IsValid == null)
            {
                vm.IsValid = true;
            }

            ProductMainImage mainImage;
            if (param.ImageDictionnary.TryGetValue(Tuple.Create(recrurringLineItem.ProductId, recrurringLineItem.VariantId), out mainImage))
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
            var getProductResponse = OvertureClient.Send(getProductRequest);

            if (getProductResponse == null ||
                (getProductResponse != null && recrurringLineItem.VariantId != string.Empty
                && recrurringLineItem.VariantId != null && 
                getProductResponse.Variants.SingleOrDefault(v => v.Id == recrurringLineItem.VariantId) == null))
            {
                var deleteRecurringLineItem = new DeleteRecurringOrderLineItemsRequest
                {
                    CustomerId = recrurringLineItem.CustomerId,
                    RecurringOrderLineItemIds = new List<Guid> {
                        recrurringLineItem.RecurringOrderLineItemId,
                    },
                    ScopeId = recrurringLineItem.ScopeId
                };
                OvertureClient.Send(deleteRecurringLineItem);

                return await Task.FromResult<RecurringOrderTemplateLineItemViewModel>(null).ConfigureAwaitWithCulture(false);
            }

            var variant = getProductResponse.Variants.SingleOrDefault(v => v.Id == recrurringLineItem.VariantId);

            vm.FormattedNextOccurence = vm.NextOccurence == DateTime.MinValue
                    ? string.Empty
                    : string.Format(param.CultureInfo, "{0:D}", vm.NextOccurence);
            vm.NextOccurenceValue = vm.NextOccurence == DateTime.MinValue
                    ? string.Empty
                    : vm.NextOccurence.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);

            vm.Id = recrurringLineItem.RecurringOrderLineItemId;
            vm.ProductSummary = new RecurringProductSummaryViewModel();
            vm.ProductSummary.DisplayName = ProductHelper.GetProductOrVariantDisplayName(getProductResponse, variant, param.CultureInfo);
            
            var productsPricesVm = await ProductPriceViewService.CalculatePricesAsync(new GetProductsPriceParam()
            {
                CultureInfo = param.CultureInfo,
                Scope = recrurringLineItem.ScopeId,
                ProductIds = new List<string>() { recrurringLineItem.ProductId }
            }).ConfigureAwaitWithCulture(false);
            
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


            decimal price;
            var conv = decimal.TryParse(vm.ListPrice, NumberStyles.Currency, param.CultureInfo.NumberFormat, out price);
            if (conv)
            {
                vm.TotalWithoutDiscount = LocalizationProvider.FormatPrice((decimal)vm.Quantity * price, param.CultureInfo);

                vm.Total = LocalizationProvider.FormatPrice((decimal)vm.Quantity * price, param.CultureInfo);
            }

            vm.ProductSummary.Brand = getProductResponse.Brand;
            var list = await ProductHelper.GetKeyVariantAttributes(getProductResponse, variant, param.CultureInfo, OvertureClient).ConfigureAwaitWithCulture(false);
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
                ProductName = vm.ProductSummary.DisplayName
            });


            var recurringScheduleEditUrl = RecurringScheduleUrlProvider.GetRecurringScheduleDetailsUrl(new GetRecurringScheduleDetailsUrlParam
            {
                CultureInfo = param.CultureInfo,
                RecurringScheduleId = vm.Id.ToString()
            });
            
            vm.EditUrl = recurringScheduleEditUrl;
            vm.ScheduleUrl = param.RecurringScheduleUrl;

            var program = await RecurringOrderRepository.GetRecurringOrderProgram(recrurringLineItem.ScopeId, recrurringLineItem.RecurringOrderProgramName).ConfigureAwaitWithCulture(false);
            var programViewModel = RecurringOrderProgramViewModelFactory.CreateRecurringOrderProgramViewModel(program, param.CultureInfo);
            vm.RecurringOrderProgramFrequencies = programViewModel?.Frequencies;

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

        private async Task<RecurringOrderTemplateAddressViewModel> MapShippingAddress(Guid shippingAddressId, CultureInfo culture)
        {
            var address = await AddressRepository.GetAddressByIdAsync(shippingAddressId).ConfigureAwaitWithCulture(false);

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
            if (address == null)
            {
                return new RecurringOrderTemplateAddressViewModel();
            }

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
                        var programViewModel = RecurringOrderProgramViewModelFactory.CreateRecurringOrderProgramViewModel(program, culture);
                        lineitem.RecurringOrderProgramFrequencies = programViewModel?.Frequencies;
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
    }
}