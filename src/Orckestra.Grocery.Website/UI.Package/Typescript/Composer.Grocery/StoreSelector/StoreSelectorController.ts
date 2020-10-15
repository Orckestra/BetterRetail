///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='./VueComponents/StepperStepVueComponent.ts' />
///<reference path='./VueComponents/StepperVueComponent.ts' />
///<reference path='../../Composer.Store/StoreLocator/Services/GeoLocationService.ts' />
///<reference path='../../Composer.Store/Store/StoreService.ts' />
///<reference path='../SelectedStoreService.ts' />
///<reference path='../../Composer.SingleCheckout/Services/ShippingMethodService.ts' />
///<reference path='../../Composer.SingleCheckout/Enums/FulfillmentMethodTypes.ts' />
/// <reference path='../../Composer.Cart/CartSummary/CartService.ts' />
///<reference path='../SelectedStoreEvents.ts' />

module Orckestra.Composer {
    export class StoreSelectorController extends Controller {
        public StoreSelector: Vue;
        protected GeoService: GeoLocationService = new GeoLocationService();
        protected storeService: IStoreService = StoreService.instance();
        protected selectedStoreService: ISelectedStoreService = SelectedStoreService.instance();
        protected shippingMethodService: ShippingMethodService = new ShippingMethodService();
        protected cartService = CartService.getInstance();

        public initialize() {
            super.initialize();

            let getDefaultStorePromise = this.selectedStoreService.getStore();
            let getShippingMethodTypesPromise = this.shippingMethodService.getShippingMethodTypes();
            Q.all([getDefaultStorePromise, getShippingMethodTypesPromise])
                .spread((defaultStore, shippingMethodTypes) => {
                    this.initializeVueComponent(defaultStore, shippingMethodTypes);
                })
                .fail((reason: any) => {
                    console.error('Error while initializing StoreSelectorController.', reason);
                });
        }

        private handleStartShoppingRedirect() {
            let startShoppingEl = this.context.container.find("#startShoppingUrl");
            if (startShoppingEl && !Utils.IsC1ConsolePreview()) {
                let redirectUrl = startShoppingEl.attr("href");
                window.location.href = redirectUrl;
            }
        }

        private initializeVueComponent({Store, TimeSlot}, shippingMethodTypesVm: any) {
            let self: StoreSelectorController = this;

            this.StoreSelector = new Vue({
                el: '#vueStoreSelector',
                components: {
                    [StepperVueComponent.componentMame]: StepperVueComponent.getComponent(),
                    [StepperStepVueComponent.componentMame]: StepperStepVueComponent.getComponent(),
                },
                data: {
                    DefaultStore: Store,
                    AllStores: undefined,
                    Stores: [],
                    SelectedStoreId: undefined,
                    PostalCode: undefined,
                    Location: undefined,
                    ShippingMethodType: undefined,
                    ShippingMethodTypes: shippingMethodTypesVm.ShippingMethodTypes,
                    DaysOnPage: 5,
                    StartIndex: 0,
                    ReservedSlotData: undefined,
                    DayAvailabilityList: [],
                    BeginTime: 0,
                    EndTime: 0,
                    TimeSlotRows: [],
                    CurrentShipmentId: undefined,
                    Mode: {
                        Loading: false,
                        SeeMoreStores: false
                    },
                    Errors: {
                        TimeSlotLoadingError: false,
                        TimeSlotSelectionError: false
                    }
                },
                mounted() {
                    this.initPostalCodeSearchBox();
                },
                computed: {
                    IsLocationFilled() {
                        return !!this.Location;
                    },
                    IsShippingMethodTypeFilled() {
                        return !!this.ShippingMethodType;
                    },
                    IsStoreIdFilled() {
                        return !!this.SelectedStoreId;
                    },
                    IsTimeSlotFilled() {
                        return !!this.ReservedSlotData;
                    },
                    IsLoading() {
                        return this.Mode.Loading;
                    },
                    SelectedStore() {
                        return _.find(this.AllStores, (s: any) => s.Id === this.SelectedStoreId);
                    },
                    NearestStore() {
                        return this.Stores ? this.Stores[0] : undefined;
                    },
                    NoStores() {
                        return !this.Stores || this.Stores.length === 0;
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
                        if (!this.ReservedSlotData || !this.ReservedSlotData.TimeSlotReservation) return '';
                        let {ReservationDate: date, FulfillmentLocationTimeSlotId: id} = this.ReservedSlotData.TimeSlotReservation;
                        return date + id;
                    }
                },
                methods: {
                    initPostalCodeSearchBox() {
                        let opt: google.maps.places.AutocompleteOptions = { fields: ['geometry'] };
                        this.postalCodeSearchBox = new google.maps.places.Autocomplete(this.$refs.postalCodeInput, opt);

                        this.postalCodeSearchBox.addListener('place_changed', () => {
                            let place = this.postalCodeSearchBox.getPlace();
                            if (place &&  place.geometry) {
                                this.Location = place.geometry.location;
                                this.PostalCode = this.$refs.postalCodeInput.value;
                                self.eventHub.publish(SelectedStoreEvents[SelectedStoreEvents.LocationSelected], {data: this.Location});
                            } else {
                                this.Location = undefined;
                            }
                        });
                    },
                    changePostalCode(step) {
                        this.Location = undefined;
                        step.selectStep();
                    },
                    clearPostalCodeInput() {
                        this.PostalCode = undefined;
                        this.Location = undefined;
                    },
                    clearShippingMethodType() {
                        this.ShippingMethodType = undefined;
                    },
                    clearSelectedStore() {
                        this.SelectedStoreId = undefined;
                    },
                    clearSelectedTimeSlot() {
                        this.ReservedSlotData = undefined;
                    },
                    processLocation(): Q.Promise<boolean> {
                        if (!this.IsLocationFilled)
                            return Q.resolve(false);

                        self.eventHub.publish(SelectedStoreEvents[SelectedStoreEvents.CheckAvailability], {data: this.Location});
                        return self.storeService.getStoresByLocation(this.Location)
                            .then(stores => {
                                this.AllStores = stores;
                            })
                            .then(() => true);
                    },
                    processShippingMethodType(): Q.Promise<boolean> {
                        this.Stores = self.storeService.filterStoresByFulfillmentMethod(this.AllStores, this.ShippingMethodType);
                        return Q.resolve(true);
                    },
                    selectStore(event, store: any, step: any): Q.Promise<boolean> {
                        this.Mode.Loading = true;
                        this.Errors.TimeSlotSelectionError = false;

                        var busy = self.asyncBusy({elementContext: $(event.target)});
 
                        step.selectStep(); //Select store step for erase next steps
                        self.cartService.invalidateCache();
                        return self.selectedStoreService.setFulFilledMethodType(this.ShippingMethodType)
                            .then(() => self.selectedStoreService.setStore(store.Id))
                            .then((storeResult) => {
                                if (storeResult) {
                                    self.eventHub.publish(SelectedStoreEvents[SelectedStoreEvents.StoreUpdating], { data: store });
                                    this.SelectedStoreId = store.Id;
                                    this.Mode.SeeMoreStores = false;
                                    step.nextStep();
                                }

                                return storeResult;
                            })
                            .fail((reason) => {
                                console.log(reason);
                                return false;
                            })
                            .fin(() => { 
                                this.Mode.Loading = false;
                                busy.done();
                                self.eventHub.publish(SelectedStoreEvents[SelectedStoreEvents.StoreSelected], { data: store });
                                self.eventHub.publish(SelectedStoreEvents[SelectedStoreEvents.TimeSlotSelected], {
                                    data: { TimeSlot: undefined, TimeSlotReservation: undefined }
                                });
                            });
                    },
                    seeMoreStores() {
                        this.Mode.SeeMoreStores = true;
                    },
                    seeLessStores() {
                        this.Mode.SeeMoreStores = false;
                    },
                    processStore(): Q.Promise<boolean> {
                        return self.cartService.getCart()
                            .then(cart => this.CurrentShipmentId = cart.CurrentShipmentId)
                            .then(() => this.findTimeSlots())
                    },
                    findTimeSlots(): Q.Promise<boolean> {
                        if (!this.SelectedStoreId) return Q.resolve(false);
                        this.Errors.TimeSlotLoadingError = false;

                        return self.selectedStoreService.getTimeSlots(this.ShippingMethodType, this.SelectedStoreId, this.CurrentShipmentId)
                            .then(result => {
                                if (!result) return false;

                                let {DayAvailabilityList, StartHour, EndHour, RowLabelList} = result;

                                this.DayAvailabilityList = DayAvailabilityList;
                                this.TimeSlotRows = RowLabelList;
                                this.BeginTime = StartHour;
                                this.EndTime = EndHour;

                                return !!DayAvailabilityList.length;
                            }).fail(() => {
                                this.Errors.TimeSlotLoadingError = true;
                                return false;
                            })
                    },
                    selectTimeSlot(timeSlot, day) {
                        this.Errors.TimeSlotSelectionError = false;
                        self.eventHub.publish(SelectedStoreEvents[SelectedStoreEvents.TimeSlotUpdating], {data: timeSlot});
                        self.selectedStoreService.setTimeSlotId(this.SelectedStoreId, this.CurrentShipmentId, timeSlot.Id, day.Date)
                            .then(cart => {
                                const {TimeSlotReservation} = cart;
                                this.ReservedSlotData = {TimeSlot: {...timeSlot}, TimeSlotReservation};
                                self.eventHub.publish(SelectedStoreEvents[SelectedStoreEvents.TimeSlotSelected], {data: this.ReservedSlotData});
                                self.eventHub.publish('cartUpdated', {data: cart});
                                this.nextStep();
                                setTimeout(() => this.$refs.startShoppingButton.scrollIntoView({behavior: "smooth", block: "nearest"}));
                                
                            })
                            .fail(reason => {
                                this.findTimeSlots(); //try to reload slots
                                this.ReservedSlotData = {TimeSlot: undefined, TimeSlotReservation: undefined };
                                this.Errors.TimeSlotSelectionError = TimeSlotsHelper.getTimeSlotReservationError(reason.Errors);
                                self.eventHub.publish(SelectedStoreEvents[SelectedStoreEvents.TimeSlotSelected], {
                                    data: {TimeSlot: undefined, TimeSlotReservation: undefined}
                                });
                                self.eventHub.publish(SelectedStoreEvents[SelectedStoreEvents.TimeSlotSelectionFailed], {data: reason.Errors});
                            });
                    },
                    nextStep() {
                        this.$children[0].activeStep.nextStep();
                    },
                    nexDay() {
                        this.StartIndex++;
                    },
                    prevDay() {
                        this.StartIndex--;
                    },
                }
            });
        }

        private getAddressFromLocation(location: google.maps.LatLng): Q.Promise<string> {
            return this.GeoService.getAddressByLocation(location)
        }
    }
}
