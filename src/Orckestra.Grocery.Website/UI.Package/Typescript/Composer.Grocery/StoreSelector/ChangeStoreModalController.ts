///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../FulfillmentService.ts' />
///<reference path='../../Composer.Store/Store/StoreService.ts' />
///<reference path='../../Composer.SingleCheckout/Enums/FulfillmentMethodTypes.ts' />
/// <reference path='../../Composer.Cart/CartSummary/CartService.ts' />
///<reference path='../FulfillmentEvents.ts' />

module Orckestra.Composer {
    'use strict';

    export class ChangeStoreModalController extends Controller {

        public VueChangeStoreModal: Vue;
        protected selectedStoreService: IFulfillmentService = FulfillmentService.instance();
        protected storeService: IStoreService = StoreService.instance();
        protected cartService = CartService.getInstance();
        protected shippingMethodService: ShippingMethodService = new ShippingMethodService();

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
                    SelectedStore: { 
                        Store: undefined,
                        TimeSlotReservation: undefined,
                        FulfillmentMethodType: undefined
                    },
                    DayAvailabilityList: [],
                    BeginTime: 0,
                    EndTime: 0,
                    TimeSlotRows: [],
                    ShippingMethodType: undefined,
                    ShippingMethodTypes: undefined,
                    ShippingMethod: undefined,
                    PostalCode: undefined,
                    Location: undefined,
                    Stores: undefined,
                    SelectedStoreId: undefined,
                    CurrentShipmentId: undefined,
                    Mode: {
                        ChangeMode: 'store',
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
                        if (!this.ReservedSlotData) return '';
                        let { ReservationDate: date, FulfillmentLocationTimeSlotId: id } = this.ReservedSlotData.TimeSlotReservation;
                        return date + id;
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
                            } else {
                                this.Location = undefined;
                                this.Stores = undefined;
                            }
                        });
                    },
                    onModalOpened(data) {
                        this.getSelectedStoreInfo().then(() => {
                            if (!data) return;

                            this.Mode.ChangeMode = data.ChangeMode;
                            if (this.Mode.ChangeMode == 'method') {
                                self.shippingMethodService.getShippingMethodTypes()
                                    .then(result => this.ShippingMethodTypes = result.ShippingMethodTypes);
                            }

                            if(this.Mode.ChangeMode == 'slot') {
                                this.findTimeSlots();
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
                    selectFulfillmentMethod() {
                        self.selectedStoreService.setFulFilledMethodType(this.SelectedStore.FulfillmentMethodType)
                            .then(() => {
                                
                                self.eventHub.publish(FulfillmentEvents.FulfillmentMethodSelected, {
                                    data: this.SelectedStore.FulfillmentMethodType
                                });
                                self.eventHub.publish(FulfillmentEvents.StoreSelected, { data: undefined });
                                self.eventHub.publish(FulfillmentEvents.TimeSlotSelected, {
                                    data: { TimeSlot: undefined, TimeSlotReservation: undefined }
                                });
                                this.SelectedStore.Store = undefined;
                                this.SelectedStoreId = undefined;
                                this.Stores = undefined;
                                this.Mode.ChangeMode = 'store';
                                this.findStores();
                            });
      
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
                                this.Stores = self.storeService.filterStoresByFulfillmentMethod(stores, this.ShippingMethodType);
                            })
                    },
                    getSelectedStoreInfo(): Q.Promise<any> {
                        return self.selectedStoreService.getSelectedFulfillment()
                            .then(selectedStore => {
                                this.SelectedStore = {...selectedStore }
                                //this.ReservedSlotData = TimeSlotReservation && TimeSlotReservation.ReservationStatus != 3  ? { TimeSlot, TimeSlotReservation } : undefined;
                                //this.ShippingMethodType = FulfillmentMethodType;
                                this.SelectedStoreId =  this.SelectedStore.Store ? this.SelectedStore.Store .Id : undefined;
                            })
                    },
                    selectStore(event, store: any): Q.Promise<boolean> {
                        this.Mode.Loading = true;
                        var busy = self.asyncBusy({elementContext: $(event.target)});
                        self.eventHub.publish(FulfillmentEvents.StoreUpdating, { data: store });
                        return self.selectedStoreService.setStore(store.Id)
                            .then(result => {
                                if (result) {
                                    this.SelectedStore.Store = store;
                                    this.SelectedStoreId = store.Id;
                                    this.SelectedStore.TimeSlotReservation = undefined;
                                }

                                return result;
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
                                return false;
                            })
                            .fin(() => {
                                busy.done();
                                this.Mode.Loading = false;
                                this.Mode.ChangeMode = 'slot';
                                self.eventHub.publish(FulfillmentEvents.StoreSelected, { data: store });
                                self.eventHub.publish(FulfillmentEvents.TimeSlotSelected, {
                                    data: { TimeSlot: undefined, TimeSlotReservation: undefined }
                                });
                            });
                    },
                    findTimeSlots(): Q.Promise<boolean> {
                        if (!this.SelectedStoreId) return Q.resolve(false);
                        this.Errors.TimeSlotLoadingError = false;

                        return self.selectedStoreService.getTimeSlots(this.SelectedStore.FulfillmentMethodType, 
                            this.SelectedStore.Store.Id, this.CurrentShipmentId)
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
                        self.eventHub.publish(FulfillmentEvents.TimeSlotUpdating, { data: timeSlot });
                        self.selectedStoreService.setTimeSlotId(this.SelectedStore.Store.Id, this.CurrentShipmentId, timeSlot.Id, day.Date)
                            .then(cart => {
                                const {TimeSlotReservation} = cart;
                                this.SelectedStore.TimeSlotReservation = TimeSlotReservation;
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
