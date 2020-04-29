using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Enums;
using Orckestra.Composer.Factory;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Product.Parameters;
using Orckestra.Composer.Product.Services;
using Orckestra.Composer.Product.ViewModels;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Lookup;
using Orckestra.Composer.Utils;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Metadata;
using Orckestra.Overture.ServiceModel.Products;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Product.Factory
{
    public class ProductViewModelFactory : IProductViewModelFactory
    {
        protected IViewModelMapper ViewModelMapper { get; }
        protected IProductRepository ProductRepository { get; }
        protected IDamProvider DamProvider { get; }
        protected ILocalizationProvider LocalizationProvider { get; }
        protected ILookupService LookupService { get; }
        protected IProductUrlProvider ProductUrlProvider { get; }
        protected IScopeViewService ScopeViewService { get; }
        protected IRecurringOrdersRepository RecurringOrdersRepository { get; }
        protected IRecurringOrderProgramViewModelFactory RecurringOrderProgramViewModelFactory { get; }
        protected IRecurringOrdersSettings RecurringOrdersSettings { get; private set; }
        protected IProductSpecificationsViewService ProductSpecificationsViewService { get; private set; }
        protected IMyAccountUrlProvider MyAccountUrlProvider { get; private set; }

        public ProductViewModelFactory(
            IViewModelMapper viewModelMapper,
            IProductRepository productRepository,
            IDamProvider damProvider,
            ILocalizationProvider localizationProvider,
            ILookupService lookupService,
            IProductUrlProvider productUrlProvider,
            IScopeViewService scopeViewService,
            IRecurringOrdersRepository recurringOrdersRepository,
            IRecurringOrderProgramViewModelFactory recurringOrderProgramViewModelFactory,
            IRecurringOrdersSettings recurringOrdersSettings,
            IProductSpecificationsViewService productSpecificationsViewService,
            IMyAccountUrlProvider myAccountUrlProvider)
        {
            ViewModelMapper = viewModelMapper ?? throw new ArgumentNullException(nameof(viewModelMapper));
            ProductRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            DamProvider = damProvider ?? throw new ArgumentNullException(nameof(damProvider));
            LocalizationProvider = localizationProvider ?? throw new ArgumentNullException(nameof(localizationProvider));
            LookupService = lookupService ?? throw new ArgumentNullException(nameof(lookupService));
            ProductUrlProvider = productUrlProvider ?? throw new ArgumentNullException(nameof(productUrlProvider));
            ScopeViewService = scopeViewService ?? throw new ArgumentNullException(nameof(scopeViewService));
            RecurringOrdersRepository = recurringOrdersRepository ?? throw new ArgumentNullException(nameof(recurringOrdersRepository));
            RecurringOrderProgramViewModelFactory = recurringOrderProgramViewModelFactory ?? throw new ArgumentNullException(nameof(recurringOrderProgramViewModelFactory));
            RecurringOrdersSettings = recurringOrdersSettings;
            ProductSpecificationsViewService = productSpecificationsViewService ?? throw new ArgumentNullException(nameof(productSpecificationsViewService));
            MyAccountUrlProvider = myAccountUrlProvider ?? throw new ArgumentNullException(nameof(myAccountUrlProvider));
        }

        public virtual async Task<ProductViewModel> GetProductViewModel(GetProductParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (param.ProductId == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.ProductId)), nameof(param)); }
            if (param.Scope == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.Scope)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.BaseUrl)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.BaseUrl)), nameof(param)); }

            var product = await ProductRepository.GetProductAsync(param).ConfigureAwait(false);

            if (product == null) { return null; }

            var productDefinition = await ProductRepository.GetProductDefinitionAsync(new GetProductDefinitionParam
            {
                Name = product.DefinitionName,
                CultureInfo = param.CultureInfo});

            if (productDefinition == null) { return null; }

            //TODO: Use the GetLookupDisplayName
            var productLookups = await LookupService.GetLookupsAsync(LookupType.Product);
            var productDetailImages = await GetProductImages(product, product.Variants);

            var currency = await ScopeViewService.GetScopeCurrencyAsync(new GetScopeCurrencyParam
            {
                CultureInfo = param.CultureInfo,
                Scope = param.Scope});

            var productViewModel = CreateViewModel(new CreateProductDetailViewModelParam
            {
                Product = product,
                ProductDefinition = productDefinition,
                ProductLookups = productLookups,
                ProductDetailImages = productDetailImages,
                CultureInfo = param.CultureInfo,
                VariantId = param.VariantId,
                BaseUrl = param.BaseUrl,
                Currency = currency,
            });

            productViewModel = await SetViewModelRecurringOrdersRelatedProperties(param, productViewModel, product);

            return productViewModel;
        }

        protected virtual async Task<ProductViewModel> SetViewModelRecurringOrdersRelatedProperties(GetProductParam param, ProductViewModel vm, Overture.ServiceModel.Products.Product product)
        {
            if (param == null) throw new ArgumentNullException(nameof(param));
            if (vm == null) throw new ArgumentNullException(nameof(vm));

            var recurringOrdersEnabled = RecurringOrdersSettings.Enabled;

            var recurringOrderProgramName = product.PropertyBag.GetValueOrDefault<string>(Constants.ProductAttributes.RecurringOrderProgramName);

            if (string.IsNullOrWhiteSpace(recurringOrderProgramName))
            {
                return vm;
            }

            vm.RecurringOrderProgramName = recurringOrderProgramName;
            vm.Context["RecurringOrderProgramName"] = recurringOrderProgramName;

            var program = await RecurringOrdersRepository.GetRecurringOrderProgram(param.Scope, recurringOrderProgramName).ConfigureAwait(false);

            if (program == null)
            {
                return vm;
            }
           
            vm.IsRecurringOrderEligible = recurringOrdersEnabled;
            vm.Context["IsRecurringOrderEligible"] = recurringOrdersEnabled;

            if (recurringOrdersEnabled)
            {
                var recurringOrderProgramViewModel = RecurringOrderProgramViewModelFactory.CreateRecurringOrderProgramViewModel(program, param.CultureInfo);

                vm.RecurringOrderFrequencies = recurringOrderProgramViewModel.Frequencies;
                vm.Context["RecurringOrderFrequencies"] = recurringOrderProgramViewModel.Frequencies;
            }

            return vm;
        }

        protected virtual ProductViewModel CreateViewModel(CreateProductDetailViewModelParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.Product == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.Product)), nameof(param)); }
            if (param.ProductDefinition == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.ProductDefinition)), nameof(param)); }
            if (param.ProductLookups == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.ProductLookups)), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (string.IsNullOrEmpty(param.BaseUrl)) { throw new ArgumentException(GetMessageOfNullEmpty(nameof(param.BaseUrl)), nameof(param)); }

            var productDetailViewModel = ViewModelMapper.MapTo<ProductViewModel>(param.Product, param.CultureInfo);

            InitializeProductImages(param.Product.Id, param.ProductDetailImages, param.CultureInfo, productDetailViewModel);

            var productDisplayName = productDetailViewModel.DisplayName ?? string.Empty;

            var allVariantsVm = GetVariantViewModels(
                param.Product.Variants, 
                param.ProductDefinition.VariantProperties, 
                productDisplayName, 
                param.CultureInfo,
                vvm => InitializeVariantImages(param.Product.Id, param.ProductDetailImages, param.CultureInfo, vvm),
                vvm => InitializeVariantSpecificaton(param.Product, param.ProductDefinition, vvm)
            ).ToList();

            productDetailViewModel.Variants = allVariantsVm;
            var selectedVariantVm = GetSelectedVariantViewModel(param.VariantId, allVariantsVm);

            MergeSelectedVariantVmToProductVm(selectedVariantVm, productDetailViewModel);

            var selectedKvas = GetSelectedKvas(selectedVariantVm, param);
            productDetailViewModel.Context["selectedKvas"] = selectedKvas;

            //todo: create factory? the code to build variants is huge
            // get all the key variant attributes possibilities.  For instance, if the product has colors and size.  This method
            // will produce two collections (colors, sizes) and for each collection, it will contain all the possible values.
            productDetailViewModel.KeyVariantAttributeItems = CreateKvaItems(new GenerateKvaItemsParam
            {
                ProductLookups = param.ProductLookups,
                Product = param.Product,
                SelectedKvas = selectedKvas,
                ProductDefinition = param.ProductDefinition,
                CultureInfo = param.CultureInfo,
                ProductVariants = allVariantsVm
            });

            productDetailViewModel.Price = param.Product.ListPrice;

            productDetailViewModel.ProductDetailUrl = ProductUrlProvider.GetProductUrl(new GetProductUrlParam
            {                
                CultureInfo = param.CultureInfo,
                ProductId = param.Product.Id,
                VariantId = param.VariantId,
                SKU = param.Product.Sku,
                ProductName = productDisplayName
            });

            productDetailViewModel.CreateAccountUrl = MyAccountUrlProvider.GetCreateAccountUrl(new BaseUrlParameter
            {
                CultureInfo = param.CultureInfo,
                ReturnUrl = productDetailViewModel.ProductDetailUrl
            });

            SetViewModelContext(productDetailViewModel, selectedVariantVm, allVariantsVm);

            // When the product lookup is loaded, only the keys of the lookup are assigned to the product view model.  This method
            // compare the product view model bag item keys with the lookups name and assign the right localized lookup values to the view model.
            AssignLookupValuesToProductPropertyBag(param.ProductDefinition, param.ProductLookups, param.CultureInfo, productDetailViewModel);

            productDetailViewModel.Description = FixHtml(productDetailViewModel.Description);

            productDetailViewModel.DefinitionName = param.ProductDefinition.Name;

            if (ProductConfiguration.IsQuantityDisplayed &&
                ProductConfiguration.MinQuantity > 0 &&
                ProductConfiguration.MaxQuantity >= ProductConfiguration.MinQuantity)
            {
                productDetailViewModel.Quantity = new ProductQuantityViewModel
                {
                    Min = ProductConfiguration.MinQuantity,
                    Max = ProductConfiguration.MaxQuantity,
                    Value = ProductConfiguration.MinQuantity
                };
            }

            productDetailViewModel.Currency = param.Currency;
            productDetailViewModel.Specifications = ProductSpecificationsViewService.GetProductSpecificationsViewModel(new GetProductSpecificationsParam
            {
                Product = param.Product,
                ProductDefinition = param.ProductDefinition
            });

            return productDetailViewModel;
        }

        protected virtual string FixHtml(string html)
        {
            if (html == null) { return null; }

            html = Regex.Replace(html, "<br>", "<br/>", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, "&", "&#38;", RegexOptions.IgnoreCase);

            return html;
        }

        /// <summary>
        /// Initializes image fields for a given variant.
        /// </summary>
        /// <param name="productId">ID of the product.</param>
        /// <param name="productImages">Available product images.</param>
        /// <param name="cultureInfo">Culture info.</param>
        /// <param name="productViewModel">ViewModel to be impacted.</param>
        protected virtual void InitializeProductImages(
            string productId,
            IEnumerable<AllProductImages> productImages,
            CultureInfo cultureInfo,
            ProductViewModel productViewModel)
        {
            var images = BuildImages(productId, null, productViewModel.DisplayName, productImages, cultureInfo).ToList();
            var selectedImage = images.Find(i => i.Selected) ?? images.FirstOrDefault();

            productViewModel.Images = images;
            productViewModel.SelectedImage = selectedImage;
            productViewModel.FallbackImageUrl = selectedImage?.FallbackImageUrl ?? string.Empty;
        }

        protected virtual IEnumerable<VariantViewModel> GetVariantViewModels(
            IEnumerable<Variant> variants,
            IList<ProductPropertyDefinition> variantProperties,
            string displayName,
            CultureInfo cultureInfo,
            Action<VariantViewModel> imageSetter,
            Action<VariantViewModel> specificationSetter)
        {
            if (variants == null) { yield break; }

            var kvaPropertieNames = variantProperties
                .Where(vp => vp.IsKeyVariant)
                .Select(vp => vp.PropertyName)
                .ToList();

            //TODO: Move this sorting in virtual method?
            var validVariants = variants
                .Where(v => v.Active.GetValueOrDefault())
                .OrderBy(v => v.SequenceNumber);

            foreach (var variant in validVariants)
            {
                var variantVm = ViewModelMapper.MapTo<VariantViewModel>(variant, cultureInfo);
                
                if(string.IsNullOrEmpty(variantVm.DisplayName))
                {
                    variantVm.DisplayName = displayName;
                }
                
                variantVm.Kvas = variant.PropertyBag
                    .Join(kvaPropertieNames, bagEntry => bagEntry.Key,
                        kvaPropertyName => kvaPropertyName,
                        (bagEntry, kvaPropertyName) => bagEntry)
                    .ToDictionary(bagEntry => bagEntry.Key, bagEntry => bagEntry.Value);

                imageSetter.Invoke(variantVm);
                specificationSetter.Invoke(variantVm);

                yield return variantVm;
            }
        }

        /// <summary>
        /// Initializes image fields for a given variant.
        /// </summary>
        /// <param name="productId">ID of the product.</param>
        /// <param name="productImages">Available product images.</param>
        /// <param name="cultureInfo">Culture info.</param>
        /// <param name="variantViewModel">ViewModel to be impacted.</param>
        protected virtual void InitializeVariantImages(
            string productId,
            IEnumerable<AllProductImages> productImages,
            CultureInfo cultureInfo,
            VariantViewModel variantViewModel)
        {
            var images = BuildImages(productId, variantViewModel.Id, variantViewModel.DisplayName, productImages, cultureInfo).ToList();
            var selectedImage = images.Find(i => i.Selected) ?? images.FirstOrDefault();

            variantViewModel.Images = images;
            variantViewModel.SelectedImage = selectedImage;
            variantViewModel.FallbackImageUrl = selectedImage != null ? selectedImage.FallbackImageUrl : string.Empty;
        }

        /// <summary>
        /// Initializes specifications for a given variant.
        /// </summary>
        /// <param name="productId">ID of the product.</param>
        /// <param name="productImages">Available product images.</param>
        /// <param name="cultureInfo">Culture info.</param>
        /// <param name="variantViewModel">ViewModel to be impacted.</param>
        protected virtual void InitializeVariantSpecificaton(
            Overture.ServiceModel.Products.Product product,
            ProductDefinition productDefinition,
            VariantViewModel variantViewModel)
        {
            variantViewModel.Specifications = ProductSpecificationsViewService.GetProductSpecificationsViewModel(new GetProductSpecificationsParam
            {
                VariantId = variantViewModel.Id,
                Product = product,
                ProductDefinition = productDefinition
            });
        }

        /// <summary>
        /// Return the Requested Variant, or the first available if requested cannot be found
        /// </summary>
        /// <param name="selectedVariantId"></param>
        /// <param name="variants"></param>
        /// <returns></returns>
        protected virtual VariantViewModel GetSelectedVariantViewModel(
            string selectedVariantId,
            IList<VariantViewModel> variants)
        {
            if (variants == null) { return null; }

            var selectedVariant = variants.FirstOrDefault(v =>
                string.Equals(v.Id, selectedVariantId, StringComparison.InvariantCultureIgnoreCase))
                ?? variants.FirstOrDefault();

            return selectedVariant;
        }

        /// <summary>
        /// Return the initial state of the KVA selector
        /// The default implementation initialize the KVA with the selected Variant;
        /// A common override is to initialize the KVA only if the selected Variant match the requested Variant
        /// 
        /// </summary>
        /// <param name="selectedVariant"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected virtual Dictionary<string, object> GetSelectedKvas(
            VariantViewModel selectedVariant,
            CreateProductDetailViewModelParam param)
        {
            return selectedVariant == null ? new Dictionary<string, object>() : selectedVariant.Kvas;
        }

        protected virtual void MergeSelectedVariantVmToProductVm(
            VariantViewModel selectedVariantVm,
            ProductViewModel productViewModel)
        {
            if (selectedVariantVm == null)
            {
                productViewModel.Sku = productViewModel.Sku;
            }
            else
            {
                productViewModel.SelectedVariantId = selectedVariantVm.Id;
                productViewModel.Sku = selectedVariantVm.Sku;
                productViewModel.DisplayName = selectedVariantVm.DisplayName;
                productViewModel.ListPrice = selectedVariantVm.ListPrice;
                productViewModel.Images = selectedVariantVm.Images;
                productViewModel.SelectedImage = selectedVariantVm.SelectedImage;
                productViewModel.FallbackImageUrl = selectedVariantVm.FallbackImageUrl;
            }
        }

        protected virtual void SetViewModelContext(
            ProductViewModel productViewModel,
            VariantViewModel selectedVariantVm,
            IEnumerable<VariantViewModel> allVariantsVm)
        {
            //TODO: Move this to final mapping step?
            productViewModel.Context["productId"] = productViewModel.ProductId;

            if (selectedVariantVm != null)
            {
                productViewModel.Context["allVariants"] = allVariantsVm.Select(v => new { v.ListPrice, v.Id, v.Kvas, v.Sku } );
                productViewModel.Context["selectedVariantId"] = selectedVariantVm.Id;
                productViewModel.Context["displayedVariantId"] = selectedVariantVm.Id;
            }

            //Some additionnal Context Required by JS

            productViewModel.Context["CategoryId"] = productViewModel.CategoryId;
            productViewModel.Context["Sku"] = productViewModel.Sku;
            productViewModel.Context["keyVariantAttributeItems"] = productViewModel.KeyVariantAttributeItems;
            productViewModel.Context["CreateAccountUrl"] = productViewModel.CreateAccountUrl;

            // Transfer custom properties that might have been added
            foreach (var property in productViewModel.Bag)
            {
                productViewModel.Context[property.Key] = property.Value;
            }
        }

        /// <summary>
        /// Assigns the localized lookup values to product property bag.
        /// </summary>
        /// <param name="productDefinition">The product definition</param>
        /// <param name="productLookups">The product lookups.</param>
        /// <param name="cultureInfo">The culture information.</param>
        /// <param name="baseViewModel">The product detail view model.</param>
        /// TODO: Remove this method and its call and replace it by attributes on the ViewModelMapper.
        protected virtual void AssignLookupValuesToProductPropertyBag(
            ProductDefinition productDefinition,
            List<Lookup> productLookups,
            CultureInfo cultureInfo,
            IBaseViewModel baseViewModel)
        {
            IDictionary<string, string> productBagLookupValueDictionary = new Dictionary<string, string>();

            foreach (var propertyBag in baseViewModel.Bag)
            {
                if (propertyBag.Value == null) { continue; }

                var propertyName = propertyBag.Key;
                var value = propertyBag.Value.ToString();

                var productLookup = productDefinition.PropertyGroups
                    .SelectMany(pg => pg.Properties)
                    .Where(pp => pp.PropertyName == propertyName && pp.DataType == PropertyDataType.Lookup)
                    .Join(productLookups, ppd => ppd.LookupDefinition.LookupName, pl => pl.LookupName, (ppd, pl) => pl)
                    .FirstOrDefault();

                if (productLookup == null) { continue; }

                var lookupValue = productLookup.Values.Find(l => l.Value == value);

                if (lookupValue != null && lookupValue.DisplayName.ContainsKey(cultureInfo.Name))
                {
                    productBagLookupValueDictionary.Add(propertyBag.Key, lookupValue.DisplayName[cultureInfo.Name]);
                }
            }

            foreach (var productBagLookupValue in productBagLookupValueDictionary)
            {
                baseViewModel.Bag[productBagLookupValue.Key] = productBagLookupValue.Value;
            }
        }

        /// <summary>
        /// Generates the kva items that will be used to display the product properties values.  Basically, this
        /// method generates a flat list of key value properties and their possible values. 
        /// </summary>
        /// <param name="kvaParam">The GenerateKvaItemsParam</param>
        protected virtual List<KeyVariantAttributeItem> CreateKvaItems(GenerateKvaItemsParam kvaParam)
        {
            var kvas = new List<KeyVariantAttributeItem>();

            if (kvaParam.Product.Variants == null) { return kvas; }

            var productFormatter = new ProductFormatter(LocalizationProvider, LookupService);

            var properties = kvaParam.ProductDefinition.VariantProperties
                .Where(v => v.IsKeyVariant)
                .OrderBy(v => v.KeyVariantOrder)
                .ThenBy(v => v.DisplayOrder)
                .ToList();

            foreach (var property in properties)
            {
                //Get Values
                var items = kvaParam.Product.Variants
                    .Where(v => v.Active.GetValueOrDefault())
                    .SelectMany(v => v.PropertyBag.Select(pb => new { Variant = v, PB = pb }))
                    .Where(o => o.PB.Key == property.PropertyName)
                    .GroupBy(o => o.PB.Value)
                    .Select(g => new KeyVariantAttributeItemValue
                    {
                        Title = productFormatter.FormatValue(property, g.Key, kvaParam.CultureInfo)
                                ?? (g.Key ?? string.Empty).ToString(),
                        Value = g.Key,
                        Selected = false,
                        Disabled = false,
                        RelatedVariantIds = g.Select(o => o.Variant.Id).ToList()
                    })
                    .ToList();

                //Sort
                if (property.DataType == PropertyDataType.Boolean ||
                    property.DataType == PropertyDataType.Text)
                {
                    //Localised Alphabetic Order
                    items = items.OrderBy(i => i.Title)
                        .ThenBy(i => i.Value)
                        .ToList();
                }
                else if (property.DataType == PropertyDataType.Lookup)
                {
                    var lookupValues = kvaParam.ProductLookups
                                .Where(l => l.LookupName == property.LookupDefinition.LookupName)
                                .Select(l => l.Values)
                                .FirstOrDefault() ?? new List<LookupValue>();

                    //Weight Defined SortOrder (with fallback to Alphabetic)
                    items = items.Join(lookupValues, i => i.Value, lv => lv.Value, (i, lv) => new
                    {
                        Item = i,
                        LookupValue = lv
                    })
                        .OrderBy(o => o.LookupValue.SortOrder)
                        .ThenBy(o => o.Item.Title)
                        .ThenBy(o => o.Item.Value)
                        .Select(o => o.Item)
                        .ToList();
                }
                else
                {
                    //Semantic Order (Numerical or Chronological)
                    items = items.OrderBy(i => i.Value).ToList();
                }

                //Bind Images from LookupValues
                if (property.LookupDefinition != null)
                {
                    string lookupName = property.LookupDefinition.LookupName;
                    items.ForEach(item =>
                        item.ImageUrl = GetLookupImageUrl(lookupName, item.Value)
                    );
                }

                //BindSelected
                if (kvaParam.SelectedKvas.TryGetValue(property.PropertyName, out object selectedValue))
                {
                    var item = items.FirstOrDefault(i => i.Value.Equals(selectedValue));
                    if (item != null)
                    {
                        item.Selected = true;
                        item.Disabled = false;
                    }
                }

                //Stock
                kvas.Add(new KeyVariantAttributeItem
                {
                    DisplayName = property.DisplayName.GetLocalizedValue(kvaParam.CultureInfo.Name) ?? property.PropertyName,
                    PropertyName = property.PropertyName,
                    PropertyDataType = property.DataType.ToString("g"),
                    Values = items
                });
            }

            kvas = DisableMissingKvas(kvaParam, kvas);

            return EnableKvasInStock(kvaParam, kvas); 
        }

        private List<KeyVariantAttributeItem> DisableMissingKvas(GenerateKvaItemsParam kvaParam, List<KeyVariantAttributeItem> kvas)
        {
            foreach (var selectedKva in kvaParam.SelectedKvas)
            {
                var existingKvasOtherThanSelected = kvaParam.ProductVariants
                    //All product variants that have the same KVA as our selected one
                    .Where(productVariant => productVariant.Kvas[selectedKva.Key].Equals(selectedKva.Value))
                    .SelectMany(productVariant => productVariant.Kvas) //Flatten our product variants to their KVA values
                                                                       //Ignore the KVAs retrieved that are the same type as our selected one (i.e. if our selected is Color, ignore all Color KVAs and keep the other ones)
                    .Where(productVariantKva => !productVariantKva.Key.Equals(selectedKva.Key))
                    .ToList();
                var allPossibleKvasOtherThanSelected = kvas
                    .Where(kva => !kva.PropertyName.Equals(selectedKva.Key))
                    .SelectMany(v => v.Values);
                var notExistingKva = allPossibleKvasOtherThanSelected.Where(x => !existingKvasOtherThanSelected.Any(ekva => ekva.Value.Equals(x.Value)));
                foreach (var kva in notExistingKva)
                {
                    kva.Disabled = true;
                };
            };
            return kvas;
        }

        protected virtual List<KeyVariantAttributeItem> EnableKvasInStock(GenerateKvaItemsParam kvaParam, List<KeyVariantAttributeItem> kvas)
        {            
            //Enable available kvas
            foreach (var kva in kvaParam.SelectedKvas)
            {
                //get all enabled variants for the first level. i.e.: for color "black" this list would contain all sizes available for black in the first loop
                //and the second loop all colors for the size 5
                var enabledList = kvaParam.ProductVariants.Where(x => x.Kvas[kva.Key].Equals(kva.Value)).SelectMany(x => x.Kvas).Where(x => !x.Key.Equals(kva.Key)).ToList();

                foreach (var enabledItem in enabledList)
                {
                    var property = kvas.First(x => x.PropertyName.Equals(enabledItem.Key));
                    if (property.Values.Any(x => x.Value.ToString().Equals(enabledItem.Value.ToString())))
                    {
                        property.Values.First(x => x.Value.ToString().Equals(enabledItem.Value.ToString())).Disabled = false;
                    }
                };

            };

            return kvas;
        }

        /// <summary>
        /// Get the product images associate with a productId and a variantId
        /// </summary>
        /// <param name="product"></param>
        /// <param name="variants"></param>
        /// <returns></returns>
        protected virtual async Task<List<AllProductImages>> GetProductImages(
            Overture.ServiceModel.Products.Product product,
            IList<Variant> variants)
        {
            var param = new GetAllProductImagesParam
            {
                ImageSize = ProductConfiguration.ImageSize,
                ThumbnailImageSize = ProductConfiguration.ThumbnailImageSize,
                ProductZoomImageSize = ProductConfiguration.ProductZoomImageSize,
                ProductId = product.Id,
                PropertyBag = product.PropertyBag,
                ProductDefinitionName = product.DefinitionName,
                Variants = variants == null ? new List<Variant>() : variants.ToList(),
                MediaSet = product.MediaSet,
                VariantMediaSet = product.VariantMediaSet
            };

            return await DamProvider.GetAllProductImagesAsync(param).ConfigureAwait(false);
        }

        /// <summary>
        /// Build an enumerable of images applicable to the current Product / Variant.
        /// </summary>
        /// <param name="productId">Id of the product.</param>
        /// <param name="variantId">Id of the Variant.</param>
        /// <param name="productImages">Images of all products being mapped.</param>
        /// <param name="cultureInfo">Culture Info.</param>
        /// <returns></returns>
        protected virtual IEnumerable<ProductDetailImageViewModel> BuildImages(
            string productId,
            string variantId,
            string defaultAlt,
            IEnumerable<AllProductImages> productImages,
            CultureInfo cultureInfo)
        {
            if (productImages == null) { return Enumerable.Empty<ProductDetailImageViewModel>(); }

            var images = productImages
                .Where(pi => string.Equals(pi.ProductId, productId, StringComparison.InvariantCultureIgnoreCase))
                .Where(pi => string.Equals(pi.VariantId, variantId, StringComparison.InvariantCultureIgnoreCase))
                .OrderBy(pi => pi.SequenceNumber)
                .Select(pi => {
                    var image = ViewModelMapper.MapTo<ProductDetailImageViewModel>(pi, cultureInfo);
                    if (string.IsNullOrEmpty(image.Alt))
                        image.Alt = defaultAlt;
                    return image;
                });

            var productDetailImageViewModels = SetFirstImageSelected(images);

            return productDetailImageViewModels;
        }

        protected static IEnumerable<ProductDetailImageViewModel> SetFirstImageSelected(
          IEnumerable<ProductDetailImageViewModel> imageViewModels)
        {
            var list = imageViewModels.ToList();
            var firstImage = list.FirstOrDefault();

            if (firstImage != null)
            {
                firstImage.Selected = true;
            }
            return list;
        }

        /// <summary>
        /// Find an image associated to the lookup value (used by FGL)
        /// </summary>
        /// <param name="lookupName">The lookupName as configured in overture</param>
        /// <param name="value">The value of the entry in that lookup</param>
        /// <returns>An Image Url, or null if none found</returns>
        protected virtual string GetLookupImageUrl(string lookupName, object value)
        {
            return null;
        }
    }
}