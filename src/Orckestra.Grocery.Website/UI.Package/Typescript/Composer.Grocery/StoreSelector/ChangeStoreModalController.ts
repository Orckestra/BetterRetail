///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../FulfillmentService.ts' />
///<reference path='../../Composer.Store/Store/StoreService.ts' />
///<reference path='../../Composer.SingleCheckout/Enums/FulfillmentMethodTypes.ts' />
/// <reference path='../../Composer.Cart/CartSummary/CartService.ts' />
///<reference path='../FulfillmentEvents.ts' />
///<reference path='../FulfillmentHelper.ts' />


module Orckestra.Composer {
    'use strict';

    export class ChangeStoreModalController extends Controller {

        public VueChangeStoreModal: Vue;
        protected selectedStoreService: IFulfillmentService = FulfillmentService.instance();
        protected storeService: IStoreService = StoreService.instance();
        protected cartService = CartService.getInstance();
        protected shippingMethodService: ShippingMethodService = new ShippingMethodService();
        protected cache = CacheProvider.instance().defaultCache;

        public initialize() {
            super.initialize();
            this.initializeVueComponent();
        }

        private initializeVueComponent() {
            let self: ChangeStoreModalController = this;
            let commonFulfillmentOptions = FulfillmentHelper.getCommonSelectedFulfillmentStateOptions();
            this.VueChangeStoreModal = new Vue({
                el: '#changeStoreModal',
                data: {
                    ...commonFulfillmentOptions.data,
                    DaysOnPage: 5,
                    StartIndex: 0,
                    DayAvailabilityList: [],
                    BeginTime: 0,
                    EndTime: 0,
                    TimeSlotRows: [],
                    ShippingMethodTypes: undefined,
                    ShippingMethod: undefined,
                    PostalCode: undefined,
                    Location: undefined,
                    Stores: undefined,
                    CurrentShipmentId: undefined,
                    Mode: {
                        ChangeMode: 'store',
                        Loading: false
                    }
                },
                mounted() {
                    this.getCartPromise = () => self.cartService.getCart().then(cart => {
                        this.CurrentShipmentId = cart.CurrentShipmentId;
                        this.ShippingMethod = cart.ShippingMethod;
                    });
                    this.initPostalCodeSearchBox();
                    this.getCachedAddressAndLocation();

                    self.eventHub.subscribe('modal-opened', e => this.getCartPromise().then(() => this.onModalOpened(e.data)));
                },
                computed: {
                    IsLoading() {
                        return this.Mode.Loading;
                    },
                    ShowStoreList() {
                        return this.Stores && this.Stores.length && this.Mode.ChangeMode == 'store'
                    },
                    NoStores() {
                        return this.Stores && this.Stores.length === 0 && this.Location;
                    },
                    DisplayedDays() {
                        return this.DayAvailabilityList.slice(this.StartIndex, this.StartIndex + this.DaysOnPage);
                    },
                    StartDayLabel() {
                        if (!this.DayAvailabilityList.length > this.StartIndex) return;

                        return this.DayAvailabilityList[this.StartIndex].DisplayMonth;
                    },
                    disableNextDay() {
                        return this.DayAvailabilityList.length <= this.StartIndex + this.DaysOnPage;
                    },
                    disablePrevDay() {
                        return this.StartIndex <= 0;
                    },
                    ReservedSlotId() {
                        if (!this.SelectedFulfillment.TimeSlotReservation) return '';
                        let { ReservationDate: date, FulfillmentLocationTimeSlotId: id } = this.SelectedFulfillment.TimeSlotReservation;
                        return date + id;
                    },
                    SelectedStoreId() {
                        return this.SelectedFulfillment.Store ? this.SelectedFulfillment.Store.Id : undefined;
                    }
                },
                methods: {
                    initPostalCodeSearchBox() {
                        let opt: google.maps.places.AutocompleteOptions = { fields: ['geometry'] };
                        this.postalCodeSearchBox = new google.maps.places.Autocomplete(this.$refs.postalCodeInput, opt);

                        this.postalCodeSearchBox.addListener('place_changed', () => {

                            let place = this.postalCodeSearchBox.getPlace();
                            if (place && place.geometry) {
                                this.Location = place.geometry.location;
                                this.PostalCode = this.$refs.postalCodeInput.value;
                                self.eventHub.publish(FulfillmentEvents.LocationSelected, { data: this.Location });
                                self.cache.set(StoreLocatorService.SearchPointLocationCacheKey, this.Location);
                                self.cache.set(StoreLocatorService.SearchPointAddressCacheKey, this.PostalCode);
                            } else {
                                this.Location = undefined;
                                this.Stores = undefined;
                            }
                        });
                    },
                    getCachedAddressAndLocation() {
                        self.cache.get<any>(StoreLocatorService.SearchPointAddressCacheKey)
                            .then(cachedAddr => {
                                this.PostalCode = cachedAddr;
                                return self.cache.get<any>(StoreLocatorService.SearchPointLocationCacheKey);
                            })
                            .then(location => { 
                                this.Location = location; 
                            })
                            .fail(() => this.PostalCode = '');
                    },
                    onModalOpened(data) {
                        this.getSelectedStoreInfo().then(() => {
                            if (!data) return;

                            this.Mode.ChangeMode = data.ChangeMode;
                            if (this.Mode.ChangeMode == 'method') {
                                self.shippingMethodService.getShippingMethodTypes()
                                    .then(result => this.ShippingMethodTypes = result.ShippingMethodTypes);
                            }

                            if (this.Mode.ChangeMode == 'slot') {
                                this.findTimeSlots();
                            }
                        })
                    },

                    selectFulfillmentMethod() {
                        this.SelectedFulfillment.Store = undefined;
                        this.Stores = undefined;
                        this.Mode.ChangeMode = 'store';
                        this.findStores();

                    },
                    changeMethod() {
                        if (!this.ShippingMethodTypes) {
                            self.shippingMethodService.getShippingMethodTypes()
                                .then(result => this.ShippingMethodTypes = result.ShippingMethodTypes);
                        }
                        this.Mode.ChangeMode = 'method';
                    },
                    changeStore() {
                        this.Mode.ChangeMode = 'store';
                    },
                    findStores() {
                        if (!this.Location) {
                            return;
                        }
                        self.eventHub.publish(FulfillmentEvents[FulfillmentEvents.CheckAvailability], { data: this.Location });
                        self.storeService.getStoresByLocation(this.Location)
                            .then(stores => {
                                this.Stores = self.storeService.filterStoresByFulfillmentMethod(stores, this.SelectedFulfillment.FulfillmentMethodType);
                            })
                    },
                    getSelectedStoreInfo(): Q.Promise<any> {
                        return self.selectedStoreService.getSelectedFulfillment()
                            .then(fulfillment => {
                                this.SelectedFulfillment = { ...fulfillment }
                             })
                    },
                    selectStore(event, store: any): Q.Promise<boolean> {
                        this.Mode.Loading = true;
                        this.Errors.StoreSelectionError = false;
                        var busy = self.asyncBusy({ elementContext: $(event.target) });
                        self.eventHub.publish(FulfillmentEvents.StoreUpdating, { data: store });
                        return self.selectedStoreService.setFulfillment(store.Id, this.SelectedFulfillment.FulfillmentMethodType)
                            .then(result => {
                                this.SelectedFulfillment = {...result};
                                self.eventHub.publish(FulfillmentEvents.FulfillmentMethodSelected, {
                                    data: this.SelectedFulfillment.FulfillmentMethodType
                                });
                                self.eventHub.publish(FulfillmentEvents.StoreSelected, { data: this.SelectedFulfillment.Store });
                                this.Mode.ChangeMode = 'slot';
                            })
                            .then(() => self.cartService.invalidateCache())
                            .then(() => Q.all([self.cartService.getCart(), this.findTimeSlots()]))
                            .then(([cart,]: any[]) => {
                                this.CurrentShipmentId = cart.CurrentShipmentId;
                                this.ShippingMethod = cart.ShippingMethod;
                                self.eventHub.publish(CartEvents.CartUpdated, { data: cart });
                            })
                            .fail((reason) => {
                                console.log(reason);
                                this.Errors.StoreSelectionError = true;
                                self.eventHub.publish(FulfillmentEvents.StoreSelectionFailed, { data: store });
                                return false;
                            })
                            .fin(() => {
                                busy.done();
                                this.Mode.Loading = false;
                                self.eventHub.publish(FulfillmentEvents.TimeSlotSelected, {
                                    data: { TimeSlot: undefined, TimeSlotReservation: undefined }
                                });
                            });
                    },
                    findTimeSlots(): Q.Promise<boolean> {
                        if (!this.SelectedStoreId) return Q.resolve(false);
                        this.Errors.TimeSlotLoadingError = false;
                        this.Mode.Loading = true;

                        return self.selectedStoreService.getTimeSlots(this.SelectedFulfillment.FulfillmentMethodType,
                            this.SelectedFulfillment.Store.Id, this.CurrentShipmentId)
                            .then(result => {
                                if (!result) return false;

                                let { DayAvailabilityList, StartHour, EndHour, RowLabelList } = result;

                                this.DayAvailabilityList = DayAvailabilityList;
                                this.TimeSlotRows = RowLabelList;
                                this.BeginTime = StartHour;
                                this.EndTime = EndHour;

                                return DayAvailabilityList.length;
                            }).fail(() => {
                                this.Errors.TimeSlotLoadingError = true;
                                return false;
                            })
                            .fin(() => this.Mode.Loading = false)
                    },
                    clearPostalCodeInput() {
                        this.PostalCode = undefined;
                        this.Location = undefined;
                    },
                    selectTimeSlot(timeSlot, day) {
                        self.eventHub.publish(FulfillmentEvents.TimeSlotUpdating, { data: timeSlot });
                        self.selectedStoreService.setTimeSlotId(this.SelectedFulfillment.Store.Id, this.CurrentShipmentId, timeSlot.Id, day.Date)
                            .then(cart => {
                                const { TimeSlotReservation } = cart;
                                this.SelectedFulfillment.TimeSlotReservation = TimeSlotReservation;
                                let data = { TimeSlot: { ...timeSlot }, TimeSlotReservation };
                                self.eventHub.publish(FulfillmentEvents.TimeSlotSelected, { data });
                                self.eventHub.publish('cartUpdated', { data: cart });
                            })
                            .fail(reason => {
                                console.log(reason);
                                self.eventHub.publish(FulfillmentEvents.TimeSlotSelected, {
                                    data: { TimeSlot: undefined, TimeSlotReservation: undefined }
                                });
                                self.eventHub.publish(FulfillmentEvents.TimeSlotSelectionFailed, { data: reason.Errors });
                            });
                    },
                    nexDay() {
                        this.StartIndex++;
                    },
                    prevDay() {
                        this.StartIndex--;
                    },
                    onClickNotNow() {
                        self.selectedStoreService.disableForcingToSelectTimeSlot();
                    }
                }
            });
        }
    }
}
