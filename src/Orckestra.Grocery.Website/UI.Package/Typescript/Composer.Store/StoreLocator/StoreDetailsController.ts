///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='./Services/GeoLocationService.ts' />
///<reference path='../../Composer.Grocery/FulfillmentService.ts' />
///<reference path='../../Composer.Grocery/FulfillmentHelper.ts' />
///<reference path='../../Composer.Grocery/FulfillmentEvents.ts' />

module Orckestra.Composer {

    export class StoreDetailsController extends Controller {

        protected _geoService: GeoLocationService = new GeoLocationService();
        protected fulfillmentService: IFulfillmentService = FulfillmentService.instance();

        private _map: google.maps.Map;
        private _marker: google.maps.Marker;

        public initialize() {
            super.initialize();

            this.fulfillmentService.getSelectedFulfillment()
                .then(fulfillment => this.initializeVueComponent(fulfillment));

            var center = new google.maps.LatLng(this.context.viewModel.latitude, this.context.viewModel.longitude);
            var mapOptions: google.maps.MapOptions = {
                center: center,
                zoom: this.context.viewModel.zoom ? this.context.viewModel.zoom : 14,
                mapTypeId: google.maps.MapTypeId.ROADMAP,
                panControl: false,
                keyboardShortcuts: false,
                scaleControl: false,
                scrollwheel: false,
                zoomControl: false,
                draggable: false,
                streetViewControl: false,
                overviewMapControl: false,
                overviewMapControlOptions: { opened: false },
                disableDefaultUI: true
            };

            this._map = new google.maps.Map(this.context.container.find(`#map`)[0], mapOptions);
            this._marker = new google.maps.Marker({
                position: center,
                map: this._map,
                icon: '/UI.Package/Images/map/marker-default.png'
            });

            this.context.window.addEventListener('resize', () => this._map.setCenter(this._marker.getPosition()));


            this.setGoogleDirectionLink();
        }

        protected initializeVueComponent(fulfillment) {
            let self: StoreDetailsController = this;
            let commonFulfillmentOptions = FulfillmentHelper.getCommonSelectedFulfillmentStateOptions(fulfillment);
            let data = self.context.viewModel;
            new Vue({
                el: "#vueStoreDetails",
                data: {
                    ...data,
                    ...commonFulfillmentOptions.data
                },
                mounted() {
                    self.eventHub.subscribe(FulfillmentEvents.StoreUpdating, e => this.onStoreUpdating(e.data));
                    self.eventHub.subscribe(FulfillmentEvents.StoreSelected, e => this.onStoreSelected(e.data));
                    self.eventHub.subscribe(FulfillmentEvents.TimeSlotUpdating, e => this.onSlotUpdating(e.data));
                    self.eventHub.subscribe(FulfillmentEvents.TimeSlotSelected, e => this.onSlotSelected(e.data));
                },
                computed: {
                    ...commonFulfillmentOptions.computed,
                    IsCurrentStoreSelected() {
                        return !!(this.SelectedFulfillment.Store && this.SelectedFulfillment.Store.Id === data.id);
                    },
                    IsSupportSelectedFulfillmentMethod() {
                        let type = this.SelectedFulfillment.FulfillmentMethodType;
                        return (this.SupportPickUp && type === FulfillmentMethodTypes.PickUp) ||
                            (this.SupportDelivery && type === FulfillmentMethodTypes.Delivery) ||
                            (this.SupportDelivery && type === FulfillmentMethodTypes.Shipping);
                    }
                },
                methods: {
                    selectStore() {
                        this.SelectedFulfillment.StoreLoading = true;
                        self.fulfillmentService.setFulfillment(data.id, this.SelectedFulfillment.FulfillmentMethodType)
                            .then(fulfillment => {
                                self.eventHub.publish(FulfillmentEvents.StoreSelected, { data: fulfillment.Store });
                            });
                    },
                    ...commonFulfillmentOptions.methods
                }
            })
        }

        protected setGoogleDirectionLink() {
            this._geoService.geolocate().then(location => {
                this._geoService.updateDirectionLinksWithLatLngSourceAddress(this.context.container, location);
            });
        }
    }
}