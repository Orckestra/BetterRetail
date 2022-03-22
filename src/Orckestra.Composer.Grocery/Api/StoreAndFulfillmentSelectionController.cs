using Orckestra.Composer.Cart;
using Orckestra.Composer.Grocery.Parameters;
using Orckestra.Composer.Grocery.Providers;
using Orckestra.Composer.Grocery.Requests;
using Orckestra.Composer.Grocery.Services;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.Composer.WebAPIFilters;
using Orckestra.Overture.ServiceModel.Orders;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace Orckestra.Composer.Grocery.Api
{
    /// <summary>
    /// API provides functionality to get or set an active store for a certain customer
    /// </summary>
    [ValidateLanguage]
    [JQueryOnlyFilter]
    [ValidateModelState]
    public class StoreAndFulfillmentSelectionController : ApiController
    {
        public StoreAndFulfillmentSelectionController(IComposerContext composerContext,
            IStoreAndFulfillmentSelectionViewService storeAndFulfillmentSelectionViewService,
            IStoreAndFulfillmentSelectionProvider storeAndFulfillmentSelectionProvider)
        {
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            StoreAndFulfillmentSelectionViewService = storeAndFulfillmentSelectionViewService ?? throw new ArgumentNullException(nameof(storeAndFulfillmentSelectionViewService));
            StoreAndFulfillmentSelectionProvider = storeAndFulfillmentSelectionProvider ?? throw new ArgumentNullException(nameof(storeAndFulfillmentSelectionProvider));

        }

        protected IComposerContext ComposerContext { get; private set; }
        protected IStoreAndFulfillmentSelectionViewService StoreAndFulfillmentSelectionViewService { get; private set; }
        protected IStoreAndFulfillmentSelectionProvider StoreAndFulfillmentSelectionProvider { get; private set; }

        /// <summary>
        /// Get an active store of a certain customer.
        /// In the first line, the active store is looking in cookies. 
        /// In the second line, in customer preferences (if authorized).
        /// In the third line, in Grocery settings of Commerce Settings of a website.
        /// </summary>
        /// <returns></returns>
        [ActionName("getSelectedFulfillment")]
        [HttpGet]
        [ValidateModelState]
        public virtual async Task<IHttpActionResult> GetSelectedFulfillment()
        {
            var viewService = await StoreAndFulfillmentSelectionViewService.GetSelectedFulfillmentAsync(new GetSelectedFulfillmentParam
            {
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString(),
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = ComposerContext.CustomerId,
                IsAuthenticated = ComposerContext.IsAuthenticated,
                Scope = ComposerContext.Scope,
            });

            return Ok(viewService);
        }

        /// <summary>
        /// Set up an active store and fulfilment method type for a certain customer.
        /// This store will be used during an interaction of the customer with a website.
        /// For example, adding products to a cart or searching for products.
        /// During changing of current active store current cart of the customer will be moved to 
        /// the scope of a new store
        /// </summary>
        /// <param name="request">Container with parameters, contains FulfillmentMethodType string and store GUID</param>
        /// <returns>Http action result of processing operation</returns>
        [ActionName("setFulfillment")]
        [HttpPost]
        [ValidateModelState]
        public virtual async Task<IHttpActionResult> SetSelectedFulfillment(SetSelectedFulfillmentRequest request)
        {
            if (!Enum.TryParse(request.FulfillmentMethodType, out FulfillmentMethodType fulfillmentMethodType))
            {
                throw new ArgumentException($"Cannot parse {nameof(request.FulfillmentMethodType)} with value '{request.FulfillmentMethodType}'", nameof(request));
            }

            if (!Guid.TryParse(request.StoreId, out Guid storeGuidId))
            {
                throw new ArgumentException($"Cannot parse {nameof(request.StoreId)} with value '{request.StoreId}'", nameof(request));
            }

            var fulfillment = await StoreAndFulfillmentSelectionViewService.SetSelectedFulfillmentAsync(new SetSelectedFulfillmentParam
            {
                FulfillmentMethodType = fulfillmentMethodType,
                StoreId = storeGuidId,
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = ComposerContext.CustomerId,
                CartName = CartConfiguration.ShoppingCartName,
                IsAuthenticated = ComposerContext.IsAuthenticated,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString()
            });

            return Ok(fulfillment);
        }

        /// <summary>
        /// Set selected time slot
        /// </summary>
        /// <param name="request">Container with parameters, contains time slot GUID</param>
        /// <returns>Http action result of processing operation</returns>
        [ActionName("setSelectedTimeslot")]
        [HttpPost]
        [ValidateModelState]
        public virtual async Task<IHttpActionResult> SetSelectedTimeSlot(SetSelectedTimeSlotRequest request)
        {
            if (!Guid.TryParse(request.SlotId, out Guid guidId))
            {
                throw new ArgumentException($"Cannot parse {nameof(request.SlotId)} with value '{request.SlotId}'", nameof(request));
            }

            if (!DateTime.TryParse(request.Date, out DateTime date))
            {
                throw new ArgumentException($"Cannot parse {nameof(request.Date)} with value '{request.Date}'", nameof(request));
            }

            if (!Guid.TryParse(request.ShipmentId, out Guid shipmentId))
            {
                throw new ArgumentException($"Cannot parse {nameof(request.ShipmentId)} with value '{request.ShipmentId}'", nameof(request));
            }

            if (!Guid.TryParse(request.StoreId, out Guid fulfillmentLocationId))
            {
                throw new ArgumentException($"Cannot parse {nameof(request.StoreId)} with value '{request.StoreId}'", nameof(request));
            }

            var result = await StoreAndFulfillmentSelectionViewService.SetSelectedTimeSlotAsync(new SetSelectedTimeSlotParam
            {
                SlotId = guidId,
                Day = date.ToUniversalTime(),
                CultureInfo = ComposerContext.CultureInfo,
                Scope = ComposerContext.Scope,
                CustomerId = ComposerContext.CustomerId,
                CartName = CartConfiguration.ShoppingCartName,
                ShipmentId = shipmentId,
                FulfillmentLocationId = fulfillmentLocationId,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString()
            });

            return Ok(result);
        }

        /// <summary>
        /// Get time slots availability
        /// </summary>
        /// <param name="request">Container with parameters, contains fulfillmentMethodType <see cref="string"/> and store <see cref="Guid"/></param>
        /// <returns>Http action result of processing operation</returns>
        [ActionName("gettimeslots")]
        [HttpPost]
        [ValidateModelState]
        public virtual async Task<IHttpActionResult> GetTimeSlotsAvailability(GetTimeSlotsAvailabilityRequest request)
        {
            if (!Guid.TryParse(request.StoreId, out Guid fulfillmentLocationId))
            {
                throw new ArgumentException($"Cannot parse {nameof(request.StoreId)} with value '{request.StoreId}'", nameof(request));
            }

            if (!Enum.TryParse(request.FulfillmentMethodTypeString, out FulfillmentMethodType fulfillmentMethodType))
            {
                throw new ArgumentException($"Cannot parse {nameof(request.FulfillmentMethodTypeString)} with value '{request.FulfillmentMethodTypeString}'", nameof(request));
            }

            if (!Guid.TryParse(request.ShipmentId, out Guid shipmentId))
            {
                throw new ArgumentException($"Cannot parse {nameof(request.ShipmentId)} with value '{request.ShipmentId}'", nameof(request));
            }

            var param = new CalculateScheduleAvailabilitySlotsParam()
            {
                CultureInfo = ComposerContext.CultureInfo,
                Scope = ComposerContext.Scope,
                FulfillmentLocationId = fulfillmentLocationId,
                FulfillmentType = fulfillmentMethodType,
                ShipmentId = shipmentId
            };

            var result = await StoreAndFulfillmentSelectionViewService.CalculateScheduleAvailabilitySlotsAsync(param);

            return Ok(result);
        }

        /// <summary>
        /// Disable forcing user to select store and allow website browsing without store
        /// </summary>
        /// <returns></returns>
        [ActionName("disableforcingtoselect")]
        [HttpPost]
        [ValidateModelState]
        public virtual void DisableForcingToSelectStore()
        {
            StoreAndFulfillmentSelectionProvider.EnableBrowsingWithoutStoreSelection();
        }

        [ActionName("setOrderFulfillment")]
        [HttpPost]
        [ValidateModelState]
        public virtual async Task<IHttpActionResult> SetOrderFulfillment([FromBody]string orderNumber)
        {
            if (string.IsNullOrEmpty(orderNumber)) throw new ArgumentNullException(nameof(orderNumber));

            var viewService = await StoreAndFulfillmentSelectionViewService.SetOrderFulfillmentAsync(new SetOrderFulfillmentParam
            {
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString(),
                CultureInfo = ComposerContext.CultureInfo,
                Scope = ComposerContext.Scope,
                OrderNumber = orderNumber
            }).ConfigureAwait(false);

            return Ok(viewService);
        }

        [ActionName("setStoreFulfillment")]
        [HttpPost]
        public virtual async Task<IHttpActionResult> SetStoreFulfillment([FromBody] string storeNumber)
        {
            if (string.IsNullOrEmpty(storeNumber)) throw new ArgumentNullException(nameof(storeNumber));

            await StoreAndFulfillmentSelectionProvider.SetFullfilmentSelectionByStore(new SetFulfillmentSelectionByStoreParam()
            {
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = ComposerContext.CustomerId,
                StoreNumber = storeNumber,
                IsAuthenticated = ComposerContext.IsAuthenticated
            }).ConfigureAwait(false);

            return Ok(true);
        }

        [ActionName("recoverFulfillment")]
        [HttpGet]
        public virtual async Task<IHttpActionResult> RecoverFulfillment()
        {
            await StoreAndFulfillmentSelectionProvider.RecoverSelection(new RecoverSelectionDataParam()
            {
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = ComposerContext.CustomerId,
                CartName= CartConfiguration.ShoppingCartName,
                IsAuthenticated = ComposerContext.IsAuthenticated
            }).ConfigureAwait(false);

            return Ok(true);
        }
    }
}