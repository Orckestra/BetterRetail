using System;
using System.Globalization;
using Orckestra.Composer.Cart.Factory;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Country;
using Orckestra.Composer.Grocery.ViewModels;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Composer.Services;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Grocery.Factory
{
    public class GroceryCartViewModelFactory : CartViewModelFactory
    {
        public GroceryCartViewModelFactory(
            ILocalizationProvider localizationProvider,
            IViewModelMapper viewModelMapper,
            IFulfillmentMethodRepository fulfillmentMethodRepository,
            ICountryService countryService,
            IComposerContext composerContext,
            ITaxViewModelFactory taxViewModelFactory,
            ILineItemViewModelFactory lineItemViewModelFactory,
            IRewardViewModelFactory rewardViewModelFactory,
            ICartUrlProvider cartUrlProvider,
            ITimeSlotViewModelFactory timeSlotViewModelFactory)
            :
            base(
                localizationProvider,
                viewModelMapper,
                fulfillmentMethodRepository,
                countryService,
                composerContext,
                taxViewModelFactory,
                lineItemViewModelFactory,
                rewardViewModelFactory,
                cartUrlProvider)
        {
            TimeSlotViewModelFactory = timeSlotViewModelFactory ?? throw new ArgumentNullException(nameof(timeSlotViewModelFactory));
        }

        public ITimeSlotViewModelFactory TimeSlotViewModelFactory { get; }

        protected override void MapOneShipment(Shipment shipment, CultureInfo cultureInfo, ProductImageInfo imageInfo,
            string baseUrl, CartViewModel cartVm, Overture.ServiceModel.Orders.Cart cart)
        {
            base.MapOneShipment(shipment, cultureInfo, imageInfo, baseUrl, cartVm, cart);
            var extendedVM = cartVm.AsExtensionModel<IGroceryCartViewModel>();
            extendedVM.TimeSlotReservation = TimeSlotViewModelFactory.GetTimeSlotReservationViewModel(shipment);
        }
    }
}