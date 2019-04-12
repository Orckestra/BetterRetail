using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Providers.LineItemValidation;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Cart.Repositories.Order;
using System.Threading.Tasks;
using Orckestra.Composer.Services;
using Orckestra.Composer.Cart.Helper;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Utils;
using Orckestra.Composer.Helper;

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

        public LineItemViewModelFactory(IViewModelMapper viewModelMapper,
            ILocalizationProvider localizationProvider,
            IProductUrlProvider productUrlProvider,
            IRewardViewModelFactory rewardViewModelFactory,
            ILineItemValidationProvider lineItemValidationProvider,
            IRecurringOrdersRepository recurringOrderRepository,
            IComposerContext composerContext)
        {
            if (localizationProvider == null) { throw new ArgumentNullException("localizationProvider"); }
            if (viewModelMapper == null) { throw new ArgumentNullException("viewModelMapper"); }
            if (productUrlProvider == null) { throw new ArgumentNullException("productUrlProvider"); }
            if (rewardViewModelFactory == null) { throw new ArgumentNullException("rewardViewModelFactory"); }
            if (lineItemValidationProvider == null) { throw new ArgumentNullException("lineItemValidationProvider"); }
            if (recurringOrderRepository == null) { throw new ArgumentNullException("recurringOrderRepository"); }

            ViewModelMapper = viewModelMapper;
            LocalizationProvider = localizationProvider;
            ProductUrlProvider = productUrlProvider;
            RewardViewModelFactory = rewardViewModelFactory;
            LineItemValidationProvider = lineItemValidationProvider;
            RecurringOrderRepository = recurringOrderRepository;
            ComposerContext = composerContext;
        }

        /// <summary>
        /// Gets a list of LineItemDetailViewModel from a list of overture LineItem objects.
        /// </summary>
        public virtual IEnumerable<LineItemDetailViewModel> CreateViewModel(CreateListOfLineItemDetailViewModelParam param)
        {
            if (param.LineItems == null)
            {
                yield break;
            }

            var imgDictionary = LineItemHelper.BuildImageDictionaryFor(param.ImageInfo.ImageUrls);

            var processedCart = param.Cart as ProcessedCart;
            var preMapAction = processedCart == null
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
                          && (int) (lineItem.CurrentPrice.Value*100) < (int) (lineItem.DefaultPrice.Value*100);
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

            ProductMainImage mainImage;
            if (param.ImageDictionary.TryGetValue(Tuple.Create(lineItem.ProductId, lineItem.VariantId), out mainImage))
            {
                vm.ImageUrl = mainImage.ImageUrl;
                vm.FallbackImageUrl = mainImage.FallbackImageUrl;
            }

            vm.ProductUrl = ProductUrlProvider.GetProductUrl(new GetProductUrlParam
            {
                CultureInfo = param.CultureInfo,                
                VariantId = lineItem.VariantId,
                ProductId = lineItem.ProductId,
                ProductName = lineItem.ProductSummary.DisplayName
            });

            vm.AdditionalFees = MapLineItemAdditionalFeeViewModel(lineItem, param.CultureInfo).ToList();

            //Because the whole class is not async, we call a .Result here
            vm.RecurringOrderFrequencyDisplayName = GetRecurringOrderFrequencyDisplayName(lineItem, param.CultureInfo).Result;
            
            return vm;
        }

        public async virtual Task<string> GetRecurringOrderFrequencyDisplayName(LineItem lineItem, CultureInfo cultureInfo)
        {
            if (lineItem == null) { return string.Empty; }

            var scope = ComposerContext.Scope;
            var recurringProgramName = lineItem.RecurringOrderProgramName;

            if(string.IsNullOrEmpty(recurringProgramName)) { return string.Empty; }

            var program = await RecurringOrderRepository.GetRecurringOrderProgram(scope, recurringProgramName).ConfigureAwaitWithCulture(false);

            if (RecurringOrderCartHelper.IsRecurringOrderLineItemValid(lineItem))
            {               
                if (program != null)
                {
                    var frequency = program.Frequencies.FirstOrDefault(f => string.Equals(f.RecurringOrderFrequencyName, lineItem.RecurringOrderFrequencyName, StringComparison.OrdinalIgnoreCase));

                    if (frequency != null)
                    {
                        var localization = frequency.Localizations.FirstOrDefault(l => string.Equals(l.CultureIso, cultureInfo.Name, StringComparison.OrdinalIgnoreCase));

                        if (localization != null)
                            return localization.DisplayName;
                        else
                            return frequency.RecurringOrderFrequencyName;
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
            if (lineItem.AdditionalFees == null)
            {
                yield break;
            }

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
    }
}
