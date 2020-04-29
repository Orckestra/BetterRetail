using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Helper;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Providers.LineItemValidation;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Factory;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.RecurringOrders;

namespace Orckestra.Composer.Cart.Factory
{
    public class LineItemViewModelFactory : ILineItemViewModelFactory
    {
        protected IViewModelMapper ViewModelMapper { get; private set; }
        protected ILocalizationProvider LocalizationProvider { get; private set; }
        protected IProductUrlProvider ProductUrlProvider { get; private set; }
        protected IRewardViewModelFactory RewardViewModelFactory { get; private set; }
        protected ILineItemValidationProvider LineItemValidationProvider { get; private set; }
        protected IRecurringOrdersRepository RecurringOrderRepository { get; private set; }
        protected IComposerContext ComposerContext { get; private set; }
        protected IRecurringOrderProgramViewModelFactory RecurringOrderProgramViewModelFactory { get; private set; }
        protected IRecurringOrdersSettings RecurringOrdersSettings { get; private set; }

        public LineItemViewModelFactory(IViewModelMapper viewModelMapper,
            ILocalizationProvider localizationProvider,
            IProductUrlProvider productUrlProvider,
            IRewardViewModelFactory rewardViewModelFactory,
            ILineItemValidationProvider lineItemValidationProvider,
            IRecurringOrdersRepository recurringOrderRepository,
            IComposerContext composerContext,
            IRecurringOrderProgramViewModelFactory recurringOrderProgramViewModelFactory,
            IRecurringOrdersSettings recurringOrdersSettings)
        {
            ViewModelMapper = viewModelMapper ?? throw new ArgumentNullException(nameof(viewModelMapper));
            LocalizationProvider = localizationProvider ?? throw new ArgumentNullException(nameof(localizationProvider));
            ProductUrlProvider = productUrlProvider ?? throw new ArgumentNullException(nameof(productUrlProvider));
            RewardViewModelFactory = rewardViewModelFactory ?? throw new ArgumentNullException(nameof(rewardViewModelFactory));
            LineItemValidationProvider = lineItemValidationProvider ?? throw new ArgumentNullException(nameof(lineItemValidationProvider));
            RecurringOrderRepository = recurringOrderRepository ?? throw new ArgumentNullException(nameof(recurringOrderRepository));
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            RecurringOrderProgramViewModelFactory = recurringOrderProgramViewModelFactory ?? throw new ArgumentNullException(nameof(recurringOrderProgramViewModelFactory));
            RecurringOrdersSettings = recurringOrdersSettings ?? throw new ArgumentNullException(nameof(recurringOrdersSettings));
        }

        /// <summary>
        /// Gets a list of LineItemDetailViewModel from a list of overture LineItem objects.
        /// </summary>
        public virtual IEnumerable<LineItemDetailViewModel> CreateViewModel(CreateListOfLineItemDetailViewModelParam param)
        {
            if (param.LineItems == null) { yield break; }

            var imgDictionary = LineItemHelper.BuildImageDictionaryFor(param.ImageInfo.ImageUrls);

            var preMapAction = !(param.Cart is ProcessedCart processedCart)
                ? new Action<LineItem>(li => { })
                : li => LineItemValidationProvider.ValidateLineItem(processedCart, li);

            foreach (var lineItem in param.LineItems)
            {
                var vm = GetLineItemDetailViewModel(new CreateLineItemDetailViewModelParam
                {
                    PreMapAction = preMapAction,
                    LineItem = lineItem,
                    CultureInfo = param.CultureInfo,
                    ImageDictionary = imgDictionary,
                    BaseUrl = param.BaseUrl
                });

                yield return vm;
            }
        }

        protected virtual LineItemDetailViewModel GetLineItemDetailViewModel(CreateLineItemDetailViewModelParam param)
        {
            param.PreMapAction.Invoke(param.LineItem);
            var lineItem = param.LineItem;

            var vm = ViewModelMapper.MapTo<LineItemDetailViewModel>(lineItem, param.CultureInfo);

            if (vm.IsValid == null)
            {
                vm.IsValid = true;
            }

            vm.Rewards = RewardViewModelFactory.CreateViewModel(lineItem.Rewards, param.CultureInfo, RewardLevel.LineItem).ToList();
            vm.IsOnSale = lineItem.CurrentPrice.HasValue && lineItem.DefaultPrice.HasValue
                          && (int)(lineItem.CurrentPrice.Value * 100) < (int)(lineItem.DefaultPrice.Value * 100);
            vm.IsPriceDiscounted = lineItem.DiscountAmount.GetValueOrDefault(0) > 0;

            decimal lineItemsSavingSale = Math.Abs(decimal.Multiply(
                decimal.Subtract(
                    lineItem.CurrentPrice.GetValueOrDefault(0),
                    lineItem.DefaultPrice.GetValueOrDefault(0)),
                Convert.ToDecimal(lineItem.Quantity)));

            decimal lineItemsSavingTotal = decimal.Add(lineItem.DiscountAmount.GetValueOrDefault(0), lineItemsSavingSale);

            vm.SavingsTotal = lineItemsSavingTotal.Equals(0) ? string.Empty : LocalizationProvider.FormatPrice(lineItemsSavingTotal, param.CultureInfo);

            vm.KeyVariantAttributesList = GetKeyVariantAttributes(new GetKeyVariantAttributesParam {
                KvaValues = lineItem.KvaValues,
                KvaDisplayValues = lineItem.KvaDisplayValues
            }).ToList();

            if (param.ImageDictionary.TryGetValue(Tuple.Create(lineItem.ProductId, lineItem.VariantId), out ProductMainImage mainImage))
            {
                vm.ImageUrl = mainImage.ImageUrl;
                vm.FallbackImageUrl = mainImage.FallbackImageUrl;
            }

            vm.ProductUrl = ProductUrlProvider.GetProductUrl(new GetProductUrlParam
            {
                CultureInfo = param.CultureInfo,
                VariantId = lineItem.VariantId,
                ProductId = lineItem.ProductId,
                ProductName = lineItem.ProductSummary.DisplayName,
                SKU = lineItem.Sku
            });

            vm.AdditionalFees = MapLineItemAdditionalFeeViewModel(lineItem, param.CultureInfo).ToList();

            //Because the whole class is not async, we call a .Result here
            _ = MapRecurringOrderFrequencies(vm, lineItem, param.CultureInfo).Result;

            return vm;
        }

        public async virtual Task<bool> MapRecurringOrderFrequencies(LineItemDetailViewModel vm, LineItem lineItem, CultureInfo cultureInfo)
        {
            if (lineItem == null) { return false; }

            var scope = ComposerContext.Scope;
            var recurringProgramName = lineItem.RecurringOrderProgramName;

            if (string.IsNullOrEmpty(recurringProgramName) || !RecurringOrdersSettings.Enabled) { return false; }

            var program = await RecurringOrderRepository.GetRecurringOrderProgram(scope, recurringProgramName).ConfigureAwait(false);

            vm.RecurringOrderFrequencyDisplayName = GetRecurringOrderFrequencyDisplayName(program, lineItem, cultureInfo);

            var programViewModel = RecurringOrderProgramViewModelFactory.CreateRecurringOrderProgramViewModel(program, cultureInfo);
            vm.RecurringOrderProgramFrequencies = programViewModel?.Frequencies;

            return true;
        }

        protected virtual string GetRecurringOrderFrequencyDisplayName(RecurringOrderProgram program, LineItem lineItem, CultureInfo cultureInfo)
        {
            if (RecurringOrderCartHelper.IsRecurringOrderLineItemValid(lineItem))
            {
                if (program != null)
                {
                    var frequency = program.Frequencies
                        .Find(f => string.Equals(f.RecurringOrderFrequencyName, lineItem.RecurringOrderFrequencyName, StringComparison.OrdinalIgnoreCase));

                    if (frequency != null)
                    {
                        var localization = frequency.Localizations.Find(l => string.Equals(l.CultureIso, cultureInfo.Name, StringComparison.OrdinalIgnoreCase));
                        return localization != null ? localization.DisplayName : frequency.RecurringOrderFrequencyName;
                    }
                }
            }
            return string.Empty;
        }
        
        /// <summary>
        /// Gets the KeyVariant attributes from a line item.
        /// </summary>
        public virtual IEnumerable<KeyVariantAttributes> GetKeyVariantAttributes(GetKeyVariantAttributesParam param)
        {
            if (param.KvaDisplayValues == null) { yield break; }

            foreach (var pair in param.KvaValues.OrderBy(p => p.Key))
            {
                var displayValue = param.KvaDisplayValues[pair.Key];

                yield return new KeyVariantAttributes()
                {
                    Key = pair.Key,
                    Value = displayValue.ToString(),
                    OriginalValue = pair.Value.ToString()
                };
            }
        }

        protected virtual IEnumerable<AdditionalFeeViewModel> MapLineItemAdditionalFeeViewModel(LineItem lineItem, CultureInfo cultureInfo)
        {
            if (lineItem.AdditionalFees == null) { yield break; }

            foreach (var lineItemAdditionalFee in lineItem.AdditionalFees)
            {
                var additionalFeeViewModel = ViewModelMapper.MapTo<AdditionalFeeViewModel>(lineItemAdditionalFee, cultureInfo);

                switch (lineItemAdditionalFee.CalculationRule)
                {
                    case AdditionalFeeCalculationRule.PerUnit:
                        additionalFeeViewModel.TotalAmount = lineItemAdditionalFee.Amount * (decimal)lineItem.Quantity;
                        break;
                    case AdditionalFeeCalculationRule.PerLineItem:
                        additionalFeeViewModel.TotalAmount = lineItemAdditionalFee.Amount;
                        break;
                }

                yield return additionalFeeViewModel;
            }
        }

        public virtual IEnumerable<LightLineItemDetailViewModel> CreateLightViewModel(CreateLightListOfLineItemDetailViewModelParam param)
        {
            if (param.LineItems == null) { yield break; }

            var imgDictionary = LineItemHelper.BuildImageDictionaryFor(param.ImageInfo.ImageUrls);

            var preMapAction = !(param.Cart is ProcessedCart processedCart)
                ? new Action<LineItem>(li => { })
                : li => LineItemValidationProvider.ValidateLineItem(processedCart, li);

            foreach (var lineItem in param.LineItems)
            {
                var vm = GetLightLineItemDetailViewModel(new CreateLineItemDetailViewModelParam
                {
                    PreMapAction = preMapAction,
                    LineItem = lineItem,
                    CultureInfo = param.CultureInfo,
                    ImageDictionary = imgDictionary,
                    BaseUrl = param.BaseUrl
                });

                yield return vm;
            }
        }

        protected virtual LightLineItemDetailViewModel GetLightLineItemDetailViewModel(CreateLineItemDetailViewModelParam param)
        {
            param.PreMapAction.Invoke(param.LineItem);
            var lineItem = param.LineItem;

            var vm = ViewModelMapper.MapTo<LightLineItemDetailViewModel>(lineItem, param.CultureInfo);

            if (vm.IsValid == null)
            {
                vm.IsValid = true;
            }

            if (param.ImageDictionary.TryGetValue(Tuple.Create(lineItem.ProductId, lineItem.VariantId), out ProductMainImage mainImage))
            {
                vm.ImageUrl = mainImage.ImageUrl;
                vm.FallbackImageUrl = mainImage.FallbackImageUrl;
            }

            vm.ProductUrl = ProductUrlProvider.GetProductUrl(new GetProductUrlParam
            {
                CultureInfo = param.CultureInfo,
                VariantId = lineItem.VariantId,
                ProductId = lineItem.ProductId,
                ProductName = lineItem.ProductSummary.DisplayName,
                SKU = lineItem.Sku
            });
            
            return vm;
        }
    }
}