using Composite.Plugins.PageTemplates.MasterPages.Controls.Functions;
using Orckestra.Composer.Cart;
using Orckestra.Composer.Cart.Factory;
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

                        if (preferredStore != null) return preferredStore;
                    }
                };

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

        public async Task SetSelectedStoreAsync(SetSelectedStoreParam param)
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
        }

        protected virtual async Task<ProcessedCart> ChangeSelectedStoreAsync(StoreServiceModel currentStore, StoreServiceModel newStore, SetSelectedStoreParam param)
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
                FulfillementMethodType = cookieData.FulfillmentMethodType
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
                await TimeSlotRepository.DeleteFulfillmentLocationTimeSlotReservationByIdAsync(new BaseFulfillmentLocationTimeSlotReservationParam()
                {
                    SlotReservationId = cookieData.TimeSlotReservationId == default ? param.TimeSlotReservationId : cookieData.TimeSlotReservationId,
                    Scope = param.Scope,
                    FulfillmentLocationId = param.FulfillmentLocationId
                }).ConfigureAwait(false);

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
                if (!Guid.TryParse(shipment?.FulfillmentScheduleReservationNumber, out Guid timeSlotReservationId)) return null;

                cookieData.TimeSlotReservationId = timeSlotReservationId;
                cookieData.SelectedDay = param.Day;
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

        public virtual async Task SetSelectedFulfillmentMethodTypeAsync(SetSelectedFulfillmentMethodTypeParam param)
        {
            if (param == null) throw new ArgumentNullException(nameof(param));

            var cart = await CartRepository.GetCartAsync(new GetCartParam
            {
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                Scope = param.Scope,
                CartName = param.CartName
            }).ConfigureAwait(false);

            var shipment = cart.Shipments.FirstOrDefault();
            if (param.FulfillmentMethodType == shipment?.FulfillmentMethod?.FulfillmentMethodType)
            {
                return;
            }

            if (Guid.TryParse(shipment?.FulfillmentScheduleReservationNumber, out Guid timeSlotReservationId)) {
                await TimeSlotRepository.DeleteFulfillmentLocationTimeSlotReservationByIdAsync(new BaseFulfillmentLocationTimeSlotReservationParam()
                {
                    SlotReservationId = timeSlotReservationId,
                    Scope = param.Scope,
                    FulfillmentLocationId = shipment.FulfillmentLocationId
                }).ConfigureAwait(false);
            };

            var fulfillmentMethods = await FulfillmentMethodRepository.GetCalculatedFulfillmentMethods(new GetShippingMethodsParam
            {
                CartName = cart.Name,
                CustomerId = cart.CustomerId,
                CultureInfo = new CultureInfo(cart.CultureName),
                Scope = cart.ScopeId
            }).ConfigureAwait(false);

            var newMethod = fulfillmentMethods.FirstOrDefault(method => method.FulfillmentMethodType == param.FulfillmentMethodType);

            var updateShippinParam = new UpdateShipmentParam
            {
                CartName = cart.Name,
                CultureInfo = new CultureInfo(cart.CultureName),
                FulfillmentLocationId = shipment.FulfillmentLocationId,
                CustomerId = cart.CustomerId,
                FulfillmentMethodName = newMethod?.Name,
                FulfillmentScheduleMode = shipment.FulfillmentScheduleMode,
                FulfillmentScheduledTimeBeginDate = null,
                FulfillmentScheduledTimeEndDate = null,
                PropertyBag = shipment.PropertyBag,
                Id = shipment.Id,
                ScopeId = cart.ScopeId,
                PickUpLocationId = null,
                ShippingAddress = null,
                ShippingProviderId = newMethod?.ShippingProviderId ?? default
            };

            var updatedCart = await CartRepository.UpdateShipmentAsync(updateShippinParam).ConfigureAwait(false);

            var cookieData = new ExtendedCookieData(CookieAccessor.Read())
            {
                FulfillmentMethodType = param.FulfillmentMethodType,
                SelectedStoreNumber = default,
                TimeSlotReservationId = default,
                SelectedDay = default
             };

            CookieAccessor.Write(cookieData.Cookie);
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

        public virtual Task RecoverSelection(RecoverSelectionDataParam param)
        {
            if (param == null) throw new ArgumentException(GetMessageOfNull(nameof(param)));
            if (param.CultureInfo == null) throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)));
            if (param.CustomerId == Guid.Empty) throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)));

            var cookieData = new ExtendedCookieData(CookieAccessor.Read());
            var selectedStore = cookieData.SelectedStoreNumber;
            if (string.IsNullOrWhiteSpace(selectedStore))
            {
                return RecoverSelectionForCustomer(param);
            }
            else
            {
                return RecoverСustomerCartFromSelection(param);
            }
        }

        protected virtual async Task RecoverSelectionForCustomer(RecoverSelectionDataParam param)
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

                var cart = await CartRepository.GetCartAsync(new GetCartParam
                {
                    CultureInfo = param.CultureInfo,
                    CustomerId = param.CustomerId,
                    Scope = preferredStore?.ScopeId ?? ScopeProvider.DefaultScope,
                    CartName = param.CartName
                }).ConfigureAwait(false);

                var shipment = cart.Shipments?.FirstOrDefault() ?? throw new InvalidOperationException("No shipment was found in the cart.");

                if (shipment.FulfillmentLocationId == Guid.Empty)
                {
                    await ChangeSelectedStoreAsync(preferredStore, preferredStore, new SetSelectedStoreParam()
                    {
                        CultureInfo = param.CultureInfo,
                        CustomerId = param.CustomerId,
                        StoreId = preferredStore.Id,
                        IsAuthenticated = param.IsAuthenticated,
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
                        SelectedStoreNumber = preferredStore?.Number,
                    };

                    CookieAccessor.Write(cookieData.Cookie);
                }
            }
        }

        protected virtual async Task RecoverСustomerCartFromSelection(RecoverSelectionDataParam param)
        {
            if (param == null) throw new ArgumentException(GetMessageOfNull(nameof(param)));
            if (param.CultureInfo == null) throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)));
            if (param.CustomerId == Guid.Empty) throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)));

            var cookieData = new ExtendedCookieData(CookieAccessor.Read());
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
            var processedCart = await ChangeSelectedStoreAsync(currentStore, currentStore, new SetSelectedStoreParam()
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