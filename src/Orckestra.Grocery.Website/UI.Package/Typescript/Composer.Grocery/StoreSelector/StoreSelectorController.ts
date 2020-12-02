///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='./VueComponents/StepperStepVueComponent.ts' />
///<reference path='./VueComponents/StepperVueComponent.ts' />
///<reference path='../../Composer.Store/StoreLocator/Services/GeoLocationService.ts' />
///<reference path='../../Composer.Store/Store/StoreService.ts' />
///<reference path='../FulfillmentService.ts' />
///<reference path='../../Composer.SingleCheckout/Services/ShippingMethodService.ts' />
///<reference path='../../Composer.SingleCheckout/Enums/FulfillmentMethodTypes.ts' />
/// <reference path='../../Composer.Cart/CartSummary/CartService.ts' />
///<reference path='../FulfillmentEvents.ts' />

module Orckestra.Composer {
    export class StoreSelectorController extends Controller {
        public StoreSelector: Vue;
        protected GeoService: GeoLocationService = new GeoLocationService();
        protected storeService: IStoreService = StoreService.instance();
        protected selectedFulfillmentService: IFulfillmentService = FulfillmentService.instance();
        protected shippingMethodService: ShippingMethodService = new ShippingMethodService();
        protected cartService = CartService.getInstance();
        protected cache = CacheProvider.instance().defaultCache;

        public initialize() {
            super.initialize();

            let getFulfillmentPromise = this.selectedFulfillmentService.getSelectedFulfillment();
            let getShippingMethodTypesPromise = this.shippingMethodService.getShippingMethodTypes();
            Q.all([getFulfillmentPromise, getShippingMethodTypesPromise])
                .spread((fulfillment, shippingMethodTypes) => {
                    this.initializeVueComponent(fulfillment, shippingMethodTypes);
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

        private initializeVueComponent(fulfillment, shippingMethodTypesVm: any) {
            let self: StoreSelectorController = this;
            let commonFulfillmentOptions = FulfillmentHelper.getCommonSelectedFulfillmentStateOptions(fulfillment);
            this.StoreSelector = new Vue({
                el: '#vueStoreSelector',
                components: {
                    [StepperVueComponent.componentMame]: StepperVueComponent.getComponent(),
                    [StepperStepVueComponent.componentMame]: StepperStepVueComponent.getComponent(),
                },
                data: {
                    ...commonFulfillmentOptions.data,
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
                        SeeMoreStores: false,
                        Stepper: !(fulfillment.Store)
                    }
                },
                mounted() {
                    this.initPostalCodeSearchBox();
                    self.eventHub.subscribe(FulfillmentEvents.FulfillmentMethodSelected,  e => this.onMethodSelected(e.data));
                    self.eventHub.subscribe(FulfillmentEvents.StoreSelected,  e => this.onStoreSelected(e.data));
                    self.eventHub.subscribe(FulfillmentEvents.StoreUpdating, e => this.onStoreUpdating(e.data));
                    self.eventHub.subscribe(FulfillmentEvents.TimeSlotSelected,  e => this.onSlotSelected(e.data));
                    self.eventHub.subscribe(FulfillmentEvents.TimeSlotUpdating, e => this.onSlotUpdating(e.data));
                    self.eventHub.subscribe(FulfillmentEvents.TimeSlotSelectionFailed, e => this.onSlotFailed(e.data));
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
                        let { ReservationDate: date, FulfillmentLocationTimeSlotId: id } = this.ReservedSlotData.TimeSlotReservation;
                        return date + id;
                    },
                    ...commonFulfillmentOptions.computed
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

                        self.eventHub.publish(FulfillmentEvents.CheckAvailability, { data: this.Location });
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
                        this.Errors.StoreSelectionError = false;

                        var busy = self.asyncBusy({ elementContext: $(event.target) });

                        step.selectStep(); //Select store step for erase next steps
                        self.cartService.invalidateCache();
                        return self.selectedFulfillmentService.setFulfillment(store.Id, this.ShippingMethodType)
                            .then((fulfillment) => {
                                if (fulfillment) {
                                    this.SelectedStoreId = fulfillment.Store.Id;
                                    this.Mode.SeeMoreStores = false;
                                    self.eventHub.publish(FulfillmentEvents.StoreSelected, { data: fulfillment.Store });
                                }
                                step.nextStep();
                                return fulfillment;
                            })
                            .fail((reason) => {
                                console.log(reason);
                                this.Errors.StoreSelectionError = true;
                                self.eventHub.publish(FulfillmentEvents.StoreSelectionFailed, { data: store });
                                return false;
                            })
                            .fin(() => {
                                this.Mode.Loading = false;
                                busy.done();
                                self.eventHub.publish(FulfillmentEvents.TimeSlotSelected, {
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
                            .then(cart => {
                                this.CurrentShipmentId = cart.CurrentShipmentId;
                                this.findTimeSlots();
                                return true;
                            });
                    },
                    findTimeSlots(): Q.Promise<boolean> {
                        if (!this.SelectedStoreId) return Q.resolve(false);
                        this.Errors.TimeSlotLoadingError = false;
                        this.Mode.Loading = true;

                        return self.selectedFulfillmentService.getTimeSlots(this.ShippingMethodType, this.SelectedStoreId, this.CurrentShipmentId)
                            .then(result => {
                                if (!result) return false;

                                let { DayAvailabilityList, StartHour, EndHour, RowLabelList } = result;

                                this.DayAvailabilityList = DayAvailabilityList;
                                this.TimeSlotRows = RowLabelList;
                                this.BeginTime = StartHour;
                                this.EndTime = EndHour;

                                return !!DayAvailabilityList.length;
                            }).fail(() => {
                                this.Errors.TimeSlotLoadingError = true;
                                return false;
                            })
                            .fin(() => this.Mode.Loading = false)
                    },
                    selectTimeSlot(timeSlot, day) {
                        this.Errors.TimeSlotSelectionError = false;
                        self.eventHub.publish(FulfillmentEvents.TimeSlotUpdating, { data: timeSlot });
                        self.selectedFulfillmentService.setTimeSlotId(this.SelectedStoreId, this.CurrentShipmentId, timeSlot.Id, day.Date)
                            .then(cart => {
                                const { TimeSlotReservation } = cart;
                                this.ReservedSlotData = { TimeSlot: { ...timeSlot }, TimeSlotReservation };
                                self.eventHub.publish(FulfillmentEvents.TimeSlotSelected, { data: this.ReservedSlotData });
                                self.eventHub.publish('cartUpdated', { data: cart });
                                this.nextStep();
                                setTimeout(() => this.$refs.startShoppingButton.scrollIntoView({ behavior: "smooth", block: "nearest" }));

                            })
                            .fail(reason => {
                                this.findTimeSlots(); //try to reload slots
                                this.ReservedSlotData = { TimeSlot: undefined, TimeSlotReservation: undefined };
                                this.Errors.TimeSlotSelectionError = TimeSlotsHelper.getTimeSlotReservationError(reason.Errors);
                                self.eventHub.publish(FulfillmentEvents.TimeSlotSelected, {
                                    data: { TimeSlot: undefined, TimeSlotReservation: undefined }
                                });
                                self.eventHub.publish(FulfillmentEvents.TimeSlotSelectionFailed, { data: reason.Errors });
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
                    ...commonFulfillmentOptions.methods
                }
            });
        }
    }
}
