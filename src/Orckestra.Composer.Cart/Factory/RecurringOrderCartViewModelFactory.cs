using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Orckestra.Composer.Cart.Helper;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.RecurringOrders;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;


namespace Orckestra.Composer.Cart.Factory
{
    public class RecurringOrderCartViewModelFactory : IRecurringOrderCartViewModelFactory
    {
        protected ICartViewModelFactory CartViewModelFactory { get; private set; }
        protected IViewModelMapper ViewModelMapper { get; private set; }
        protected IComposerContext ComposerContext { get; private set; }
        protected IRecurringCartUrlProvider RecurringCartUrlProvider { get; private set; }
        protected IRecurringScheduleUrlProvider RecurringScheduleUrlProvider { get; private set; }
        protected ILineItemViewModelFactory LineItemViewModelFactory { get; private set; }


        public RecurringOrderCartViewModelFactory(
            ICartViewModelFactory cartViewModelFactory,
            IViewModelMapper viewModelMapper,
            IComposerContext composerContext,
            IRecurringCartUrlProvider recurringCartUrlProvider,
            IRecurringScheduleUrlProvider recurringScheduleUrlProvider,
            ILineItemViewModelFactory lineItemViewModelFactory)
        {
            CartViewModelFactory = cartViewModelFactory ?? throw new ArgumentNullException(nameof(cartViewModelFactory));
            ViewModelMapper = viewModelMapper ?? throw new ArgumentNullException(nameof(viewModelMapper));
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            RecurringCartUrlProvider = recurringCartUrlProvider ?? throw new ArgumentNullException(nameof(recurringCartUrlProvider));
            RecurringScheduleUrlProvider = recurringScheduleUrlProvider ?? throw new ArgumentNullException(nameof(recurringScheduleUrlProvider));
            LineItemViewModelFactory = lineItemViewModelFactory ?? throw new ArgumentNullException(nameof(lineItemViewModelFactory));
        }

        public virtual CartViewModel CreateRecurringOrderCartViewModel(CreateRecurringOrderCartViewModelParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (param.ProductImageInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.ProductImageInfo)), nameof(param)); }
            if (param.ProductImageInfo.ImageUrls == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.ProductImageInfo.ImageUrls)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.BaseUrl)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.BaseUrl)), nameof(param)); }

            var vm = CartViewModelFactory.CreateCartViewModel(new CreateCartViewModelParam
            {
                Cart = param.Cart,
                CultureInfo = param.CultureInfo,
                ProductImageInfo = param.ProductImageInfo,
                BaseUrl = param.BaseUrl,
                PaymentMethodDisplayNames = param.PaymentMethodDisplayNames,
                IncludeInvalidCouponsMessages = param.IncludeInvalidCouponsMessages
            });

            var roCartVm = vm.AsExtensionModel<IRecurringOrderCartViewModel>();

            FillNextOcurrence(roCartVm, param.Cart, param.CultureInfo);
            MapRecurringOrderLineitemFrequencyName(vm, param.CultureInfo, param.RecurringOrderPrograms);
            FillRecurringScheduleUrl(roCartVm, param.CultureInfo);

            roCartVm.Name = param.Cart.Name;
            vm.Context["Name"] = roCartVm.Name;           

            return vm;
        }

        protected virtual void FillRecurringScheduleUrl(IRecurringOrderCartViewModel roCartVm, CultureInfo cultureInfo)
        {
            var url = RecurringScheduleUrlProvider.GetRecurringScheduleUrl(new GetRecurringScheduleUrlParam
            {
                CultureInfo = cultureInfo
            });

            roCartVm.RecurringScheduleUrl = url;
        }

        protected virtual void FillNextOcurrence(IRecurringOrderCartViewModel vm, Overture.ServiceModel.Orders.Cart cart, CultureInfo cultureInfo)
        {
            vm.NextOccurence = GetNextOccurenceDate(cart.Shipments.First());
            vm.FormatedNextOccurence = GetFormattedNextOccurenceDate(vm.NextOccurence, cultureInfo);
            vm.NextOccurenceValue = GetNextOccurenceDateValue(vm.NextOccurence, cultureInfo);
        }

        protected virtual string GetNextOccurenceDateValue(DateTime nextOccurence, CultureInfo cultureInfo)
        {
            return nextOccurence == DateTime.MinValue
                                ? string.Empty
                                : nextOccurence.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
        }

        protected virtual void FillNextOcurrence(LightRecurringOrderCartViewModel vm, Overture.ServiceModel.Orders.Cart cart, CultureInfo cultureInfo)
        {
            vm.NextOccurence = GetNextOccurenceDate(cart.Shipments.First());
            vm.FormatedNextOccurence = GetFormattedNextOccurenceDate(vm.NextOccurence, cultureInfo);
        }

        protected static DateTime GetNextOccurenceDate(Shipment shipment)
        {
            return shipment.FulfillmentScheduledTimeBeginDate ?? DateTime.MinValue;
        }

        protected static string GetFormattedNextOccurenceDate(DateTime date, CultureInfo culture)
        {
            return date == DateTime.MinValue
                    ? string.Empty
                    : string.Format(culture, "{0:D}", date);
        }

        protected virtual void MapRecurringOrderLineitemFrequencyName(CartViewModel recurringOrderCartViewModel, CultureInfo culture, List<RecurringOrderProgram> recurringOrderPrograms)
        {
            if (recurringOrderCartViewModel.LineItemDetailViewModels == null) { return; }

            foreach (var lineitem in recurringOrderCartViewModel.LineItemDetailViewModels)
            {
                if (RecurringOrderCartHelper.IsRecurringOrderLineItemValid(lineitem))
                {
                    var program = recurringOrderPrograms.Find(p => string.Equals(p.RecurringOrderProgramName, lineitem.RecurringOrderProgramName, StringComparison.OrdinalIgnoreCase));

                    if (program != null)
                    {
                        var frequency = program.Frequencies.Find(f => string.Equals(f.RecurringOrderFrequencyName, lineitem.RecurringOrderFrequencyName, StringComparison.OrdinalIgnoreCase));

                        if (frequency != null)
                        {
                            var localization = frequency.Localizations.Find(l => string.Equals(l.CultureIso, culture.Name, StringComparison.OrdinalIgnoreCase));
                            lineitem.RecurringOrderFrequencyDisplayName = localization != null ? localization.DisplayName : frequency.RecurringOrderFrequencyName;
                        }
                    }
                }
            }
        }

        public virtual LightRecurringOrderCartViewModel CreateLightRecurringOrderCartViewModel(CreateLightRecurringOrderCartViewModelParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (param.ProductImageInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.ProductImageInfo)), nameof(param)); }
            if (param.ProductImageInfo.ImageUrls == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.ProductImageInfo.ImageUrls)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.BaseUrl)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.BaseUrl)), nameof(param)); }
            
            var vm = ViewModelMapper.MapTo<LightRecurringOrderCartViewModel>(param.Cart, param.CultureInfo);
            
            vm.LineItemDetailViewModels = LineItemViewModelFactory.CreateLightViewModel(new CreateLightListOfLineItemDetailViewModelParam
            {
                Cart = param.Cart,
                LineItems = param.Cart.GetLineItems(),
                CultureInfo = param.CultureInfo,
                ImageInfo = param.ProductImageInfo,
                BaseUrl = param.BaseUrl
            }).ToList();

            FillNextOcurrence(vm, param.Cart, param.CultureInfo);
            vm.CartDetailUrl = GetRecurringCartDetailUrl(param.CultureInfo, param.Cart.Name);

            // Reverse the items order in the Cart so the last added item will be the first in the list
            if (vm.LineItemDetailViewModels != null)
            {
                vm.LineItemDetailViewModels.Reverse();
            }

            vm.IsAuthenticated = ComposerContext.IsAuthenticated;

            return vm;   
        }

        protected virtual string GetRecurringCartDetailUrl(CultureInfo cultureInfo, string cartName)
        {
            _ = RecurringCartUrlProvider.GetRecurringCartsUrl(new GetRecurringCartsUrlParam
            {
                CultureInfo = cultureInfo
            });

            return RecurringCartUrlProvider.GetRecurringCartDetailsUrl(new GetRecurringCartDetailsUrlParam
            {
                CultureInfo = cultureInfo,
               // ReturnUrl = recurringCartsPageUrl,
                RecurringCartName = cartName
            });
        }
    }
}