using Composite.Plugins.PageTemplates.MasterPages.Controls.Functions;
using Orckestra.Composer.Cart;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Grocery.Parameters;
using Orckestra.Composer.Grocery.Repositories;
using Orckestra.Composer.Grocery.Settings;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Cookie;
using Orckestra.Composer.Store.Parameters;
using Orckestra.Composer.Store.Repositories;
using Orckestra.ExperienceManagement.Configuration;
using Orckestra.Overture.ServiceModel.Orders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;
using StoreServiceModel = Orckestra.Overture.ServiceModel.Customers.Stores.Store;

namespace Orckestra.Composer.Grocery.Providers
{
    public class StoreAndFulfillmentSelectionProvider : IStoreAndFulfillmentSelectionProvider
    {
        public StoreAndFulfillmentSelectionProvider(
            ICookieAccessor<ComposerCookieDto> cookieAccessor,
            ICustomerRepository customerRepository,
            IGrocerySettings grocerySettings,
            IStoreRepository storeRepository,
            ICartMoveProvider cartMoveProvider,
            IScopeProvider scopeProvider,
            IFulfillmentLocationsRepository fulfillmentLocationsRepository,
            IFulfillmentMethodRepository fulfillmentMethodRepository,
            ICartRepository cartRepository,
            IWishListRepository wishlistRepository,
            ITimeSlotRepository timeSlotRepository,
            ISiteConfiguration siteConfiguration,
            IWebsiteContext websiteContext)
        {
            CookieAccessor = cookieAccessor ?? throw new ArgumentNullException(nameof(cookieAccessor));
            CustomerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            GrocerySettings = grocerySettings ?? throw new ArgumentNullException(nameof(grocerySettings));
            StoreRepository = storeRepository ?? throw new ArgumentNullException(nameof(storeRepository));
            CartMoveProvider = cartMoveProvider ?? throw new ArgumentNullException(nameof(cartMoveProvider));
            ScopeProvider = scopeProvider ?? throw new ArgumentNullException(nameof(scopeProvider));
            FulfillmentLocationsRepository = fulfillmentLocationsRepository ?? throw new ArgumentNullException(nameof(fulfillmentLocationsRepository));
            FulfillmentMethodRepository = fulfillmentMethodRepository ?? throw new ArgumentNullException(nameof(fulfillmentMethodRepository));
            CartRepository = cartRepository ?? throw new ArgumentNullException(nameof(cartRepository));
            WishlistRepository = wishlistRepository ?? throw new ArgumentNullException(nameof(wishlistRepository));
            TimeSlotRepository = timeSlotRepository ?? throw new ArgumentNullException(nameof(timeSlotRepository));
            SiteConfiguration = siteConfiguration ?? throw new ArgumentNullException(nameof(siteConfiguration));
            WebsiteContext = websiteContext ?? throw new ArgumentNullException(nameof(websiteContext));
        }
        protected ICookieAccessor<ComposerCookieDto> CookieAccessor { get; private set; }
        protected ICustomerRepository CustomerRepository { get; private set; }
        protected IGrocerySettings GrocerySettings { get; private set; }
        protected IStoreRepository StoreRepository { get; private set; }
        public ICartMoveProvider CartMoveProvider { get; private set; }
        public IScopeProvider ScopeProvider { get; private set; }
        public IFulfillmentLocationsRepository FulfillmentLocationsRepository { get; private set; }
        public ICartRepository CartRepository { get; private set; }

        public IWishListRepository WishlistRepository { get; set; }
        public ITimeSlotRepository TimeSlotRepository { get; }
        public IFulfillmentMethodRepository FulfillmentMethodRepository { get; private set; }
        public ISiteConfiguration SiteConfiguration { get; set; }
        public IWebsiteContext WebsiteContext { get; set; }

        /// <summary>
        ///  Enables browsing without a store if a store isn't selected yet.
        /// </summary>
        public void EnableBrowsingWithoutStoreSelection()
        {
            var cookieData = new ExtendedCookieData(CookieAccessor.Read());
            if (string.IsNullOrEmpty(cookieData.SelectedStoreNumber))
            {
                cookieData.BrowseWithoutStore = true;
                CookieAccessor.Write(cookieData.Cookie);
            };
        }

        public async Task<bool> CustomerShouldSelectStore(GetSelectedFulfillmentParam param)
        {
            if (param.CultureInfo == null) throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)));
            if (param.IsAuthenticated && param.CustomerId == Guid.Empty)
                throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)));

            var cookieData = new ExtendedCookieData(CookieAccessor.Read());
            if (cookieData.BrowseWithoutStore)
            {
                return false;
            }

            string storeNumber = cookieData.SelectedStoreNumber;
            if (!string.IsNullOrEmpty(storeNumber))
            {
                return false;
            }

            if (param.IsAuthenticated)
            {
                var customer = await CustomerRepository.GetCustomerByIdAsync(new GetCustomerByIdParam
                {
                    CustomerId = param.CustomerId,
                    CultureInfo = param.CultureInfo,
                    Scope = ScopeProvider.DefaultScope
                }).ConfigureAwait(false);

                if (customer != null && customer.PreferredStoreId != Guid.Empty)
                {
                    return false;
                }
            };

            return true;
        }

        public async Task<StoreServiceModel> GetSelectedStoreAsync(GetSelectedFulfillmentParam param)
        {
            if (param == null) throw new ArgumentNullException(nameof(param));
            if (param.CultureInfo == null) throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)));
            if (param.IsAuthenticated && param.CustomerId == Guid.Empty)
                throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)));

            var cookieData = new ExtendedCookieData(CookieAccessor.Read());

            if (cookieData.BrowseWithoutStore
                && !param.TryGetFromDefaultSettings)
            {
                return null;
            }

            string storeNumber = cookieData.SelectedStoreNumber;

            if (string.IsNullOrEmpty(storeNumber))
            {
                if (param.IsAuthenticated)
                {
                    var preferredStore = await GetCustomerPreferredStoreAsync(param.CustomerId, param.CultureInfo).ConfigureAwait(false);
                    if (preferredStore != null) return preferredStore;
                }

                if (param.TryGetFromDefaultSettings)
                {
                    storeNumber = GrocerySettings.DefaultStore;
                }

                if (string.IsNullOrEmpty(storeNumber))
                {
                    return null;
                }
            }

            var store = await StoreRepository.GetStoreByNumberAsync(new GetStoreByNumberParam
            {
                CultureInfo = param.CultureInfo,
                IncludeAddresses = true,
                IncludeSchedules = true,
                Scope = ScopeProvider.DefaultScope,
                StoreNumber = storeNumber
            }).ConfigureAwait(false);

            return store;
        }

        private async Task<StoreServiceModel> GetCustomerPreferredStoreAsync(Guid customerId, CultureInfo culture)
        {
            var customer = await CustomerRepository.GetCustomerByIdAsync(new GetCustomerByIdParam
            {
                CustomerId = customerId,
                CultureInfo = culture,
                Scope = ScopeProvider.DefaultScope
            }).ConfigureAwait(false);

            if (customer == null || customer.PreferredStoreId == Guid.Empty) return null;

            return await StoreRepository.GetStoreAsync(new GetStoreParam
            {
                Id = customer.PreferredStoreId,
                CultureInfo = culture,
                Scope = ScopeProvider.DefaultScope,
                IncludeAddresses = true,
                IncludeSchedules = true
            }).ConfigureAwait(false);
        }

        public async Task<StoreServiceModel> SetSelectedStoreAndFulfillmentMethodTypeAsync(SetSelectedFulfillmentParam param)
        {
            if (param == null) throw new ArgumentNullException(nameof(param));
            if (param.StoreId == Guid.Empty) throw new ArgumentException(GetMessageOfEmpty(nameof(param.StoreId)));
            if (param.CultureInfo == null) throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)));

            if (param.IsAuthenticated)
            {
                if (param.CustomerId == Guid.Empty) throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)));
            }

            Task<StoreServiceModel> currentStoreTask = GetSelectedStoreAsync(new GetSelectedFulfillmentParam
            {
                TryGetFromDefaultSettings = true,
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                IsAuthenticated = param.IsAuthenticated
            });

            Task<StoreServiceModel> newStoreTask = StoreRepository.GetStoreAsync(new GetStoreParam
            {
                CultureInfo = param.CultureInfo,
                Id = param.StoreId,
                Scope = ScopeProvider.DefaultScope
            });

            var storesResult = await Task.WhenAll(currentStoreTask, newStoreTask).ConfigureAwait(false);
            StoreServiceModel currentStore = storesResult[0], newStore = storesResult[1];

            await ChangeSelectedStoreAsync(currentStore, newStore, param);

            return newStore;
        }

        protected virtual async Task<ProcessedCart> ChangeSelectedStoreAsync(StoreServiceModel currentStore, StoreServiceModel newStore, SetSelectedFulfillmentParam param)
        {
            var cookie = CookieAccessor.Read();
            cookie.Scope = newStore.ScopeId;
            var cookieData = new ExtendedCookieData(cookie);

            if (param.IsAuthenticated && param.UpdatePreferredStore)
            {
                await UpdatePreferredStoreAsync(param.CustomerId, newStore.Number).ConfigureAwait(false);
            }

            var processedCart = await CartMoveProvider.MoveCart(new MoveCartParam
            {
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                NewStore = newStore,
                ScopeFrom = currentStore?.ScopeId ?? ScopeProvider.DefaultScope,
                ScopeTo = newStore.ScopeId,
                InventoryLocationId = newStore.FulfillmentLocation?.InventoryLocationId,
                FulfillementMethodType = param.FulfillmentMethodType.HasValue ? param.FulfillmentMethodType : cookieData.FulfillmentMethodType
            }).ConfigureAwait(false);

            if (processedCart != null)
            {
                var shipment = processedCart.Shipments.FirstOrDefault();
                cookieData.FulfillmentMethodType = shipment.FulfillmentMethod?.FulfillmentMethodType;
            }

            cookieData.SelectedStoreNumber = newStore.Number;
            cookieData.BrowseWithoutStore = false;
            cookieData.TimeSlotReservationId = default;

            CookieAccessor.Write(cookieData.Cookie);

            if (param.IsAuthenticated && newStore.FulfillmentLocation != null)
            {
                await UpdateWishListWithNewFulfillmentLocation(newStore.FulfillmentLocation.Id, param).ConfigureAwait(false);
            }

            return processedCart;
        }

        public virtual async Task<StoreServiceModel> SetSelectedFulfillmentByOrderAsync(Order order)
        {
            if (order == null) { throw new ArgumentNullException(nameof(order)); }

            var shipment = order.Cart.Shipments.FirstOrDefault();

            if (shipment == null) { throw new ArgumentException($"Order #{order.OrderNumber} has no shipments"); }

            var fulfillmentLocationId = shipment.FulfillmentLocationId;

            var store = await StoreRepository.GetStoreAsync(new GetStoreParam()
            {
                Id = fulfillmentLocationId,
                Scope = order.ScopeId
            }).ConfigureAwait(false);

            if (store == null)
            {
                throw new ArgumentException($"Store for order #{order.OrderNumber} and fulfillment location id {fulfillmentLocationId} does not exist");
            }

            var cookie = CookieAccessor.Read();
            cookie.Scope = order.ScopeId;
            var cookieData = new ExtendedCookieData(cookie);
            cookieData.FulfillmentMethodType = shipment.FulfillmentMethod?.FulfillmentMethodType;
            cookieData.SelectedStoreNumber = store.Number;
            Guid.TryParse(shipment.FulfillmentScheduleReservationNumber, out Guid timeSlotReservationId);
            cookieData.TimeSlotReservationId = timeSlotReservationId;

            CookieAccessor.Write(cookieData.Cookie);

            return store;
        }

        protected virtual async Task UpdateWishListWithNewFulfillmentLocation(Guid fulfillmentLocationId, SetSelectedStoreParam param)
        {
            var wishList = await WishlistRepository.GetWishListAsync(new GetCartParam
            {
                Scope = ScopeProvider.DefaultScope,
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                CartName = CartConfiguration.WishlistCartName,
                ExecuteWorkflow = false
            }).ConfigureAwait(false);

            var wishListShipment = wishList?.Shipments?.FirstOrDefault();

            if (wishListShipment != null && wishListShipment.FulfillmentLocationId != fulfillmentLocationId)
            {
                await CartRepository.UpdateShipmentAsync(new UpdateShipmentParam
                {
                    CartName = CartConfiguration.WishlistCartName,
                    CultureInfo = param.CultureInfo,
                    FulfillmentLocationId = fulfillmentLocationId,
                    CustomerId = wishList.CustomerId,
                    FulfillmentMethodName = wishListShipment.FulfillmentMethod?.Name,
                    FulfillmentScheduleMode = wishListShipment.FulfillmentScheduleMode,
                    FulfillmentScheduledTimeBeginDate = wishListShipment.FulfillmentScheduledTimeBeginDate,
                    FulfillmentScheduledTimeEndDate = wishListShipment.FulfillmentScheduledTimeEndDate,
                    PropertyBag = wishListShipment.PropertyBag,
                    Id = wishListShipment.Id,
                    ScopeId = wishList.ScopeId,
                    ShippingAddress = wishListShipment.Address,
                    ShippingProviderId = wishListShipment.FulfillmentMethod == null ? Guid.Empty : wishListShipment.FulfillmentMethod.ShippingProviderId
                }).ConfigureAwait(false);
            }
        }

        public virtual async Task<ProcessedCart> SetSelectedTimeSlotAsync(SetSelectedTimeSlotParam param)
        {
            if (param == null) throw new ArgumentNullException(nameof(param));
            if (param.SlotId == default) throw new ArgumentException(GetMessageOfNull(nameof(param.SlotId)));

            var cookieData = new ExtendedCookieData(CookieAccessor.Read());
            if (cookieData.TimeSlotReservationId != default || param.TimeSlotReservationId != default)
            {
                var timeSlotReservationId = cookieData.TimeSlotReservationId == default ? param.TimeSlotReservationId : cookieData.TimeSlotReservationId;
                await RemoveTimeSlotReservation(param.FulfillmentLocationId, timeSlotReservationId, param.Scope).ConfigureAwait(false);

                cookieData.TimeSlotReservationId = default;
                CookieAccessor.Write(cookieData.Cookie);
            }
            try
            {
                var cart = await TimeSlotRepository.ReserveTimeSlotAsync(new ReserveTimeSlotParam
                {
                    Scope = param.Scope,
                    SlotId = param.SlotId,
                    CartName = param.CartName,
                    CustomerId = param.CustomerId,
                    ReservationDate = param.Day,
                    ExpiryTime = GrocerySettings.ReservationExpirationTimeSpan,
                    ExpiryWarningTime = GrocerySettings.ReservationExpirationWarningTimeSpan,
                    CultureInfo = param.CultureInfo,
                    ShipmentId = param.ShipmentId
                }).ConfigureAwait(false);

                var shipment = cart?.Shipments.FirstOrDefault();
                if (shipment == null || !Guid.TryParse(shipment.FulfillmentScheduleReservationNumber, out Guid timeSlotReservationId) || !shipment.FulfillmentScheduledTimeBeginDate.HasValue) return null;

                cookieData.TimeSlotReservationId = timeSlotReservationId;
                cookieData.SelectedDay = shipment.FulfillmentScheduledTimeBeginDate.Value.Date;
                cookieData.SelectedTime = shipment.FulfillmentScheduledTimeBeginDate.Value.TimeOfDay;
                CookieAccessor.Write(cookieData.Cookie);

                return cart;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }
                throw;
            }
        }

        private Task RemoveTimeSlotReservation(Guid fulfillmentLocationId, Guid timeSlotReservationId, string scope)
        {
            return TimeSlotRepository.DeleteFulfillmentLocationTimeSlotReservationByIdAsync(new BaseFulfillmentLocationTimeSlotReservationParam()
            {
                SlotReservationId = timeSlotReservationId,
                Scope = scope,
                FulfillmentLocationId = fulfillmentLocationId
            });
        }

        public virtual async Task<TimeSlotReservation> GetFulfillmentLocationTimeSlotReservationByIdAsync(BaseFulfillmentLocationTimeSlotReservationParam param)
        {
            return await TimeSlotRepository.GetFulfillmentLocationTimeSlotReservationByIdAsync(param).ConfigureAwait(false);
        }

        public virtual async Task<(TimeSlotReservation TimeSlotReservation, TimeSlot TimeSlot)?> GetSelectedTimeSlotAsync(GetSelectedTimeSlotParam param)
        {
            if (param == null) throw new ArgumentNullException(nameof(param));
            if (param.StoreId == default) throw new ArgumentException(GetMessageOfNull(nameof(param.StoreId)));
            if (string.IsNullOrWhiteSpace(param.Scope)) throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param));

            var cookieData = new ExtendedCookieData(CookieAccessor.Read());
            var timeSlotReservationId = cookieData.TimeSlotReservationId;
            if (timeSlotReservationId == default) return null;

            var getTimeSlotParam = new BaseFulfillmentLocationTimeSlotReservationParam
            {
                SlotReservationId = timeSlotReservationId,
                Scope = param.Scope,
                FulfillmentLocationId = param.StoreId
            };

            var timeSlotReservation = await TimeSlotRepository.GetFulfillmentLocationTimeSlotReservationByIdAsync(getTimeSlotParam).ConfigureAwait(false);

            if (timeSlotReservation == null) return null;
            else if (timeSlotReservation.ReservationStatus == TimeslotReservationStatus.Expired)
            {
                return (timeSlotReservation, null);
            }

            var fulfillmentMethodType = cookieData.FulfillmentMethodType;
            var timeSlot = await TimeSlotRepository.GetFulfillmentLocationTimeSlotByIdAsync(new GetFulfillmentLocationTimeSlotByIdParam
            {
                FulfillmentLocationId = param.StoreId,
                FulfillmentMethodType = fulfillmentMethodType ?? default,
                Scope = param.Scope,
                SlotId = timeSlotReservation.FulfillmentLocationTimeSlotId

            }).ConfigureAwait(false);
            if (timeSlot == null) return null;

            return (timeSlotReservation, timeSlot);
        }

        public async Task<List<DayAvailability>> CalculateScheduleAvailabilitySlotsAsync(CalculateScheduleAvailabilitySlotsParam param)
        {
            if (param == null) throw new ArgumentNullException(nameof(param));

            param.StartDate = DateTime.Now;
            param.EndDate = param.StartDate.AddDays(GrocerySettings.TimeSlotsDaysAmount - 1);

            var result = await TimeSlotRepository.CalculateScheduleAvailabilitySlotsAsync(param).ConfigureAwait(false);
            return result.ScheduleResults;
        }

        public virtual Task<FulfillmentMethodType> GetSelectedFulfillmentMethodTypeAsync()
        {
            var cookieData = new ExtendedCookieData(CookieAccessor.Read());

            return Task.FromResult(cookieData.FulfillmentMethodType ?? FulfillmentMethodType.PickUp);
        }

        public virtual Task UpdatePreferredStoreAsync(Guid customerId, string storeNumber)
        {
            return UpdatePreferredStore(customerId, Guid.Empty, storeNumber);
        }

        protected virtual Task UpdatePreferredStore(Guid customerId, Guid storeId, string storeNumber)
        {
            if (customerId == Guid.Empty) throw new ArgumentException(GetMessageOfEmpty(nameof(customerId)), nameof(Param));
            if (storeId == Guid.Empty && string.IsNullOrWhiteSpace(storeNumber)) throw new ArgumentException($"At least `{nameof(storeId)}` or `{nameof(storeNumber)}` required.");

            return CustomerRepository.UpdateUserPreferredStoreAsync(new UpdateUserPreferredStoreParam
            {
                CustomerId = customerId,
                ScopeId = ScopeProvider.DefaultScope,
                StoreId = storeId,
                StoreNumber = storeNumber
            });
        }

        public virtual void ClearSelection()
        {
            var cookieData = new ExtendedCookieData(CookieAccessor.Read())
            {
                BrowseWithoutStore = default,
                SelectedDay = default,
                SelectedStoreNumber = default,
                TimeSlotReservationId = default,
                FulfillmentMethodType = default
            };
            CookieAccessor.Write(cookieData.Cookie);
        }

        public virtual void ClearTimeSlotSelection()
        {
            var cookieData = new ExtendedCookieData(CookieAccessor.Read())
            {
                SelectedDay = default,
                TimeSlotReservationId = default,
            };

            CookieAccessor.Write(cookieData.Cookie);
        }

        /// <summary>
        /// Fill Cart with Fulfilment Selection or Recover selection by Customer
        /// Use case: when Order completed, the customer cart is refreshed, so we need to fill data from current fulfilment selection
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual Task RecoverSelection(RecoverSelectionDataParam param)
        {
            if (param == null) throw new ArgumentException(GetMessageOfNull(nameof(param)));
            if (param.CultureInfo == null) throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)));
            if (param.CustomerId == Guid.Empty) throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)));

            var cookieData = new ExtendedCookieData(CookieAccessor.Read());
            var selectedStore = cookieData.SelectedStoreNumber;
            if (string.IsNullOrWhiteSpace(selectedStore))
            {
                return RecoverFulfillmentSelectionByCustomer(param);
            }
            else
            {
                return RecoverСustomerCartFromSelection(param);
            }
        }

        /// <summary>
        /// Set fulfillment selection by Store Number
        /// Use Case: restore for previous store selection after the edit order mode
        /// </summary>
        /// <param name="storeNumber"></param>
        /// <returns></returns>
        public virtual async Task<StoreServiceModel> SetFullfilmentSelectionByStore(SetFulfillmentSelectionByStoreParam param)
        {
            if (param == null) throw new ArgumentNullException(GetMessageOfNull(nameof(param)));
            if (string.IsNullOrEmpty(param.StoreNumber)) throw new ArgumentNullException(GetMessageOfNull(nameof(param.StoreNumber)));

            var store = await StoreRepository.GetStoreByNumberAsync(new GetStoreByNumberParam
            {
                StoreNumber = param.StoreNumber,
                CultureInfo = param.CultureInfo,
                Scope = ScopeProvider.DefaultScope,
                IncludeAddresses = false,
                IncludeSchedules = true
            }).ConfigureAwait(false);

            if (store == null) throw new ArgumentException($"Store #{param.StoreNumber} doesn't exist");

            await WriteStoreCartDataToCookie(store, param.CustomerId, param.IsAuthenticated, param.CultureInfo).ConfigureAwait(false);

            return store;
        }

        /// <summary>
        /// Recover fulfillment selection from Customer preferred store data
        /// This method is used when Guest without selected fulfillment, decide to login.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        protected virtual async Task RecoverFulfillmentSelectionByCustomer(RecoverSelectionDataParam param)
        {
            if (param == null) throw new ArgumentException(GetMessageOfNull(nameof(param)));
            if (param.CultureInfo == null) throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)));
            if (param.CustomerId == Guid.Empty) throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)));

            /// When Customer Log In we need to recover cookies with actual values from the user cart
            var customer = await CustomerRepository.GetCustomerByIdAsync(new GetCustomerByIdParam
            {
                CustomerId = param.CustomerId,
                CultureInfo = param.CultureInfo,
                Scope = ScopeProvider.DefaultScope
            }).ConfigureAwait(false);

            if (customer != null && customer.PreferredStoreId != Guid.Empty)
            {
                var preferredStore = await StoreRepository.GetStoreAsync(new GetStoreParam
                {
                    Id = customer.PreferredStoreId,
                    CultureInfo = param.CultureInfo,
                    Scope = ScopeProvider.DefaultScope,
                    IncludeAddresses = true,
                    IncludeSchedules = true
                }).ConfigureAwait(false);

                // When Guest, without selected fulfillment, but added items to the cart in default configured store scope, login
                // we need to merge items from guest default scope to user preferred scope
                if (param.GuestScope != null && param.GuestScope != preferredStore.ScopeId)
                {
                    await CartMoveProvider.MoveCart(new MoveCartParam
                    {
                        CultureInfo = param.CultureInfo,
                        CustomerId = param.CustomerId,
                        NewStore = preferredStore,
                        ScopeFrom = param.GuestScope,
                        ScopeTo = preferredStore.ScopeId,
                        InventoryLocationId = preferredStore.FulfillmentLocation?.InventoryLocationId,
                        MoveFulfillment = false,// we just need to move cart items
                    }).ConfigureAwait(false);
                }

                await WriteStoreCartDataToCookie(preferredStore, param.CustomerId, param.IsAuthenticated, param.CultureInfo).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Get Cart for Store and write Cart data to fulfillemnt selection
        /// </summary>
        /// <param name="store"></param>
        /// <param name="customerId"></param>
        /// <param name="isAuthenticated"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        private async Task WriteStoreCartDataToCookie(StoreServiceModel store, Guid customerId, bool isAuthenticated, CultureInfo cultureInfo)
        {
            var cart = await CartRepository.GetCartAsync(new GetCartParam
            {
                CultureInfo = cultureInfo,
                CustomerId = customerId,
                Scope = store.ScopeId ?? ScopeProvider.DefaultScope,
                CartName = CartConfiguration.ShoppingCartName
            }).ConfigureAwait(false);

            var shipment = cart.Shipments?.FirstOrDefault() ?? throw new InvalidOperationException("No shipment was found in the cart.");

            if (shipment.FulfillmentLocationId == Guid.Empty)
            {
                // the method ChangeSelectedStoreAsync will just update the current cart with fulfilment location id from store and set fulfillment selection in cookies 
                await ChangeSelectedStoreAsync(store, store, new SetSelectedFulfillmentParam()
                {
                    CultureInfo = cultureInfo,
                    CustomerId = customerId,
                    StoreId = store.Id,
                    IsAuthenticated = isAuthenticated,
                    UpdatePreferredStore = false
                }).ConfigureAwait(false);
            }
            else
            {
                Guid.TryParse(shipment.FulfillmentScheduleReservationNumber, out Guid TimeSlotReservationId);
                var cookieData = new ExtendedCookieData(CookieAccessor.Read())
                {
                    SelectedDay = shipment.FulfillmentScheduleReservationDate,
                    TimeSlotReservationId = TimeSlotReservationId,
                    FulfillmentMethodType = shipment.FulfillmentMethod?.FulfillmentMethodType,
                    BrowseWithoutStore = false,
                    SelectedStoreNumber = store.Number,
                };

                CookieAccessor.Write(cookieData.Cookie);
            }
        }

        protected virtual async Task RecoverСustomerCartFromSelection(RecoverSelectionDataParam param)
        {
            if (param == null) throw new ArgumentException(GetMessageOfNull(nameof(param)));
            if (param.CultureInfo == null) throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)));
            if (param.CustomerId == Guid.Empty) throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)));

            var cookieData = new ExtendedCookieData(CookieAccessor.Read());

            var preferredStore = await GetCustomerPreferredStoreAsync(param.CustomerId, param.CultureInfo).ConfigureAwait(false);

            var currentStore = await GetSelectedStoreAsync(new GetSelectedFulfillmentParam
            {
                TryGetFromDefaultSettings = true,
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                IsAuthenticated = param.IsAuthenticated
            }).ConfigureAwait(false);

            //Get current reserved timeslot to set later for new recovered cart
            Guid timeSlotReservationId = cookieData.TimeSlotReservationId;
            DateTime selectedDay = cookieData.SelectedDay ?? default;
            TimeSlotReservation timeSlotReservation = null;
            if (timeSlotReservationId != default)
            {
                var getTimeSlotParam = new BaseFulfillmentLocationTimeSlotReservationParam
                {
                    SlotReservationId = timeSlotReservationId,
                    Scope = currentStore.ScopeId,
                    FulfillmentLocationId = currentStore.Id
                };

                timeSlotReservation = await TimeSlotRepository.GetFulfillmentLocationTimeSlotReservationByIdAsync(getTimeSlotParam).ConfigureAwait(false);
            }

            //Update selected store and recover cart from cookies
            var processedCart = await ChangeSelectedStoreAsync(preferredStore ?? currentStore, currentStore, new SetSelectedFulfillmentParam()
            {
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                StoreId = currentStore.Id,
                IsAuthenticated = param.IsAuthenticated
            }).ConfigureAwait(false);

            //For the new Сart set current reservation
            var shipping = processedCart?.Shipments.FirstOrDefault();
            if (timeSlotReservation != null && processedCart != null && shipping != null && timeSlotReservation.ReservationStatus != TimeslotReservationStatus.Expired)
            {
                await SetSelectedTimeSlotAsync(new SetSelectedTimeSlotParam()
                {
                    SlotId = timeSlotReservation.FulfillmentLocationTimeSlotId,
                    TimeSlotReservationId = timeSlotReservation.Id,
                    Day = selectedDay,
                    CultureInfo = param.CultureInfo,
                    Scope = processedCart.ScopeId,
                    CustomerId = processedCart.CustomerId,
                    CartName = processedCart.Name,
                    ShipmentId = shipping.Id,
                    FulfillmentLocationId = shipping.FulfillmentLocationId
                }).ConfigureAwait(false);
            }
        }

        public string GetSelectedStoreNumber()
        {
            return new ExtendedCookieData(CookieAccessor.Read()).SelectedStoreNumber;
        }
    }
}