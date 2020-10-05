///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../SelectedStoreService.ts' />
///<reference path='../../Composer.Store/Store/StoreService.ts' />
///<reference path='../../Composer.SingleCheckout/Enums/FulfillmentMethodTypes.ts' />
/// <reference path='../../Composer.Cart/CartSummary/CartService.ts' />
///<reference path='../SelectedStoreEvents.ts' />

module Orckestra.Composer {
    'use strict';

    export class ChangeStoreModalController extends Controller {

        public VueChangeStoreModal: Vue;
        protected selectedStoreService: ISelectedStoreService = SelectedStoreService.instance();
        protected storeService: IStoreService = StoreService.instance();
        protected cartService: CartService = new CartService(new CartRepository(), this.eventHub);
        public initialize() {
            super.initialize();
            this.initializeVueComponent();
        }

        private initializeVueComponent() {
            let self: ChangeStoreModalController = this;

            this.VueChangeStoreModal = new Vue({
                el: '#changeStoreModal',
                data: {
                    DaysOnPage: 5,
                    StartIndex: 0,
                    ReservedSlotData: undefined,
                    DayAvailabilityList: [],
                    BeginTime: 0,
                    EndTime: 0,
                    TimeSlotRows: [],
                    ShippingMethodType: undefined,
                    ShippingMethod: undefined,
                    PostalCode: undefined,
                    Location: undefined,
                    Stores: [],
                    SelectedStoreId: undefined,
                    CurrentShipmentId: undefined,
                    Mode: {
                        ChangeStore: true,
                        Loading: false,
                    },
                    Errors: {
                        TimeSlotLoadingError: false
                    }
                },
                mounted() {
                    this.getCartPromise = () => self.cartService.getCart().then(cart => {
                        this.CurrentShipmentId = cart.CurrentShipmentId;
                        this.ShippingMethod = cart.ShippingMethod;
                    });
                    this.initPostalCodeSearchBox();

                    self.eventHub.subscribe('modal-opened', e => this.getCartPromise().then(() => this.onModalOpened(e.data)));
                },
                computed: {
                    DefaultStore() {
                        return this.Stores.find(store => store.Id === this.SelectedStoreId);
                    },
                    IsLoading() {
                        return this.Mode.Loading;
                    },
                    ShowStoreList() {
                        return this.Stores.length && this.Mode.ChangeStore
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
                        if (!this.ReservedSlotData) return '';
                        let { ReservationDate: date, FulfillmentLocationTimeSlotId: id } = this.ReservedSlotData.TimeSlotReservation;
                        return date + id;
                    }
                },
                methods: {
                    initPostalCodeSearchBox() {
                        this.postalCodeSearchBox = new google.maps.places.SearchBox(this.$refs.postalCodeInput);

                        this.postalCodeSearchBox.addListener('places_changed', () => {

                            let places = this.postalCodeSearchBox.getPlaces();
                            if (places && places.length && places[0].geometry) {
                                this.Location = places[0].geometry.location;
                                this.PostalCode = this.$refs.postalCodeInput.value;
                                self.eventHub.publish(SelectedStoreEvents[SelectedStoreEvents.LocationSelected], { data: this.Location });
                            } else {
                                this.Location = undefined;
                                this.Stores = [];
                            }
                        });
                    },
                    onModalOpened(data) {
                        this.getDefaultStoreInfo().then(() => {
                            if (!data) return;

                            this.Mode.ChangeStore = data.ChangeStore || !this.SelectedStoreId;
                            if (!this.Mode.ChangeStore) {
                                if (!this.ShippingMethod) {
                                    this.fixCartWithShippingInfo()
                                } else {
                                    this.findTimeSlots();
                                }
                            }
                        })
                    },
                    fixCartWithShippingInfo() {
                        self.selectedStoreService.setStore(this.SelectedStoreId)
                            .then(() => self.cartService.invalidateCache())
                            .then(() => Q.all([self.cartService.getCart(), this.findTimeSlots()]))
                            .then(([cart,]: any[]) => {
                                this.CurrentShipmentId = cart.CurrentShipmentId;
                                this.ShippingMethod = cart.ShippingMethod;
                            })
                    },
                    changeStore() {
                        this.Mode.ChangeStore = true;
                    },
                    findStores() {
                        if (!this.Location) {
                            return;
                        }
                        self.eventHub.publish(SelectedStoreEvents[SelectedStoreEvents.CheckAvailability], { data: this.Location });
                        self.storeService.getStoresByLocation(this.Location)
                            .then(stores => {
                                this.Stores = self.storeService.filterStoresByFulfillmentMethod(stores, this.ShippingMethodType);
                            })
                    },
                    getDefaultStoreInfo(): Q.Promise<any> {
                        return self.selectedStoreService.getStore()
                            .then(({ Store, TimeSlot, TimeSlotReservation, FulfillmentMethodTypeString }) => {
                                this.ReservedSlotData = TimeSlotReservation && TimeSlotReservation.ReservationStatus != 3  ? { TimeSlot, TimeSlotReservation } : undefined;
                                this.ShippingMethodType = FulfillmentMethodTypeString;
                                this.Stores = Store ? [Store] : [];
                                this.SelectedStoreId = Store ? Store.Id : undefined;
                            })
                    },
                    selectStore(store: any): Q.Promise<boolean> {
                        if (!this.Mode.ChangeStore) return;

                        this.Mode.Loading = true;
                        self.eventHub.publish(SelectedStoreEvents[SelectedStoreEvents.StoreUpdating], { data: store });
                        return self.selectedStoreService.setStore(store.Id)
                            .then(result => {
                                if (result) {
                                    this.SelectedStoreId = store.Id;
                                    this.ReservedSlotData = undefined;
                                }

                                return result;
                            })
                            .then(() => self.cartService.invalidateCache())
                            .then(() => Q.all([self.cartService.getCart(), this.findTimeSlots()]))
                            .then(([cart,]: any[]) => {
                                this.CurrentShipmentId = cart.CurrentShipmentId;
                                this.ShippingMethod = cart.ShippingMethod;
                                self.eventHub.publish('cartUpdated', { data: cart });
                            })
                            .fail((reason) => {
                                console.log(reason);
                                return false;
                            })
                            .fin(() => {
                                this.Mode.Loading = false;
                                this.Mode.ChangeStore = false;
                                self.eventHub.publish(SelectedStoreEvents[SelectedStoreEvents.StoreSelected], { data: store });
                                self.eventHub.publish(SelectedStoreEvents[SelectedStoreEvents.TimeSlotSelected], {
                                    data: { TimeSlot: undefined, TimeSlotReservation: undefined }
                                });
                            });
                    },
                    findTimeSlots(): Q.Promise<boolean> {
                        if (!this.SelectedStoreId) return Q.resolve(false);
                        this.Errors.TimeSlotLoadingError = false;

                        return self.selectedStoreService.getTimeSlots(this.ShippingMethodType, this.SelectedStoreId, this.CurrentShipmentId)
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
                    },
                    clearPostalCodeInput() {
                        this.PostalCode = undefined;
                        this.Location = undefined;
                    },
                    selectTimeSlot(timeSlot, day) {
                        self.eventHub.publish(SelectedStoreEvents[SelectedStoreEvents.TimeSlotUpdating], { data: timeSlot });
                        self.selectedStoreService.setTimeSlotId(this.SelectedStoreId, this.CurrentShipmentId, timeSlot.Id, day.Date)
                            .then(cart => {
                                const {TimeSlotReservation} = cart;
                                this.ReservedSlotData = { TimeSlot: { ...timeSlot }, TimeSlotReservation };
                                self.eventHub.publish(SelectedStoreEvents[SelectedStoreEvents.TimeSlotSelected], { data: this.ReservedSlotData });
                                self.eventHub.publish('cartUpdated', { data: cart });
                            })
                            .fail(reason => {
                                console.log(reason);
                                self.eventHub.publish(SelectedStoreEvents[SelectedStoreEvents.TimeSlotSelected], {
                                    data: { TimeSlot: undefined, TimeSlotReservation: undefined }
                                });
                                self.eventHub.publish(SelectedStoreEvents[SelectedStoreEvents.TimeSlotSelectionFailed], { data: reason.Errors });
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
