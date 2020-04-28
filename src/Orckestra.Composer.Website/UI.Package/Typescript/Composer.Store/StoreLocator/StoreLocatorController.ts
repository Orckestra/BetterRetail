///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../../Typings/vue/index.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../../Mvc/IControllerActionContext.ts' />
///<reference path='./Services/StoreLocatorService.ts' />
///<reference path='./MapService.ts' />
///<reference path='./Services/GeoLocationService.ts' />
///<reference path='./IStoreLocatorInitializationOptions.ts' />
///<reference path='./IMapOptions.ts' />
///<reference path='./StoreLocatorHistoryState.ts' />
///<reference path='./IStoreLocatorHistoryState.ts' />
///<reference path='../../Cache/CacheProvider.ts' />

module Orckestra.Composer {

    export class StoreLocatorController extends Controller {

        protected _storeLocatorService: IStoreLocatorService = new StoreLocatorService();
        protected _geoService: GeoLocationService = new GeoLocationService();
        protected _mapService: MapService = new MapService(this.eventHub);
        protected _storeLocatorOptions: IStoreLocatorInitializationOptions;
        protected _historyState: StoreLocatorHistoryState = new StoreLocatorHistoryState();
        protected _isRestoreListPaging: boolean = false;
        protected _searchPointAddressCacheKey: string = 'StoreLocatorSearchAddress';
        protected cache = CacheProvider.instance().defaultCache;
        protected VueStoreList: any;

        private _searchBox: google.maps.places.SearchBox;
        private _searchBoxJQ: JQuery;
        private _searchPoint: google.maps.LatLng;
        private _searchPointMarker: google.maps.Marker;
        private _isSearch: boolean = false;
        private _timer: number;
        private _enterPressedTimer: number;
        private _getCurrentLocation: Q.Deferred<google.maps.LatLng> = Q.defer<google.maps.LatLng>();

        public getCurrentLocation(): Q.Promise<google.maps.LatLng> {
            return this._getCurrentLocation.promise;
        }

        public initialize(options: IStoreLocatorInitializationOptions = {
            mapId: 'map',
            coordinates: { Lat: -33.8688, Lng: 151.2195 },
            showNearestStoreInfo: true
        }) {
            this._storeLocatorOptions = options;
            this.registerStoreLocatorVue();
        }

        public initializeController() {
            super.initialize();
            this.registerSubscriptions();

            // get current location
            this._geoService.geolocate().then(location => {
                this._getCurrentLocation.resolve(location);
            }, reason => this._getCurrentLocation.resolve(null));

            this._storeLocatorService.getMapConfiguration()
                .then(configuration => {
                    this.initSearchBox();
                    let postedAddress = this.initCacheData();
                    let mapOptions = this.getMapOptions(this._storeLocatorOptions, this._historyState);

                    if (configuration.ZoomLevel) {
                        this._storeLocatorOptions.zoomLevel = configuration.ZoomLevel;
                    }
                    if (configuration.MarkerPadding) {
                        this._storeLocatorOptions.markerPadding = configuration.MarkerPadding;
                    }

                    this._mapService.initialize(mapOptions);

                    return this._mapService.mapInitialized().then(() => {
                        if (!this._historyState.isDefined()) {
                            this._mapService.centerMap(configuration.Bounds);
                        }

                        this.searchBoxSetBounds(configuration.Bounds);
                        return postedAddress;
                    });
                })
                .then(postedAddress => {
                    if (this._historyState.isDefined()) {
                        this.restoreMapFromHistoryState();
                        return null;
                    }

                    if (postedAddress) {
                        return this._geoService.getLocationByAddress(postedAddress);
                    } else {
                        return this.getCurrentLocation()
                            .then(currentLocation => this._geoService.getAddressByLocation(currentLocation)
                                .then(address => {
                                    this.setPostedAddress(address);
                                    return currentLocation;
                                })
                            );
                    }
                })
                .then(currentLocation => {
                    if (!currentLocation) { return ; }
                    this.eventHub.publish('searchPointChanged', { data: currentLocation });
                })
                .fail(reason => this.handlePromiseFail('StoreLocator Initialize', reason));
        }

        protected getCommonStoreLocatorVueConfig(self: StoreLocatorController): any {
            return {
                data: {
                    NextPage: null,
                    Stores: [],
                },
                mounted() {
                    self.initializeController();
                },
                methods: {
                    loadNextStoresPage() {
                        self.getStores(this.NextPage.Page).then(result => {
                            let {NextPage, Stores} = result;
                            this.setStoreList({NextPage, Stores: [...this.Stores, ...Stores]});
                        });
                    },
                    onTitleClick() {
                        self.rememberPosition();
                    },
                    setStoreList(result: any) {
                        let {NextPage, Stores} = result;
                        this.NextPage = NextPage;
                        this.Stores = Stores;

                        self.setGoogleDirectionLinks();
                    },
                    currentLocationAction() {
                        self.searchCurrentLocation();
                    }
                }
            };
        }

        protected registerStoreLocatorVue() {
            let self: StoreLocatorController = this;
            let commonOptions =  this.getCommonStoreLocatorVueConfig(self);

            this.VueStoreList = new Vue({
                el: '#storeLocator',
                mounted: commonOptions.mounted,
                data: {
                    ...commonOptions.data,
                    SelectedStoreId: null,
                    StoreLocatorLocationError: false,
                },
                methods: {
                    ...commonOptions.methods,
                    selectPickupStore(store: any) {
                        this.SelectedStoreId = store.Id;
                    },
                    showStoreLocatorLocationError() {
                        this.StoreLocatorLocationError = true;
                    }
                }
            });
        }

        private registerSubscriptions() {
            this.eventHub.subscribe('mapBoundsUpdated', e => this.onMapBoundsUpdated(e.data, this._isSearch));
            this.eventHub.subscribe('searchPointChanged', e => this.setSearchLocationInMap(e.data));
            this.eventHub.subscribe('markerClick', e => this.onMarkerClick(e.data));
            this.eventHub.subscribe('clusterClick', e => this.onClusterClick(e.data));
        }

        private initSearchBox() {
            this._searchBoxJQ = this.findElement('input[name="storeLocatorSearchInput"]');
            this._searchBox = new google.maps.places.SearchBox(<HTMLInputElement>this._searchBoxJQ[0]);

            this.searchBoxOnPlacesChanged();
            this.searchBoxOnEnterPressed();
        }

        private searchBoxOnPlacesChanged() {
            this._searchBox.addListener('places_changed', () => {
                clearTimeout(this._enterPressedTimer);
                var places = this._searchBox.getPlaces();
                if (places && places.length && places[0].geometry) {
                    this.eventHub.publish('searchPointChanged', { data: places[0].geometry.location });
                }
            });
        }

        private searchBoxOnEnterPressed() {
            this._searchBoxJQ.on('keypress', (e) => {
                var key = e.which || e.keyCode;
                if (key === 13) {
                    this._enterPressedTimer = setTimeout(() => {
                        if (this._searchPoint) {
                            this.setSearchLocationInMap(this._searchPoint);
                        }
                    }, 750);
                }
            });
        }

        private initCacheData(): Q.Promise<string> {
            // first check if address is posted from other page.
            let postedAddress = this.getPostedAddress();

            if (postedAddress) {  return Q.resolve(postedAddress); }

            // then check history state
            this._historyState.parseHistoryState();

            // then if any entered address saved in local storage
            return this.cache.get<any>(this._searchPointAddressCacheKey)
                .then(cachedAddr => {
                    this.setPostedAddress(cachedAddr);
                    return cachedAddr;

                }).fail(() =>  '');
        }

        protected getMapOptions(storeLocatorOption: IStoreLocatorInitializationOptions, historyState: IStoreLocatorHistoryState): IMapOptions {
            let { coordinates, mapId } = storeLocatorOption;

            let mapCenter = new google.maps.LatLng(coordinates.Lat, coordinates.Lng);
            let mapOptions: IMapOptions = {
                mapCanvas: this.findElement(`#${mapId}`)[0],
                infoWindowMaxWidth: 450,
                options: {
                    center: historyState.point || mapCenter,
                    zoom: historyState.zoom || 1,
                    mapTypeId: google.maps.MapTypeId.ROADMAP,
                    panControl: false,
                    keyboardShortcuts: true,
                    scaleControl: false,
                    scrollwheel: false,
                    zoomControl: true,
                    streetViewControl: false,
                    overviewMapControl: true,
                    overviewMapControlOptions: { opened: false }
                }
            };

            return mapOptions;
        }

        protected getPostedAddress(): string {
            return this._searchBoxJQ.val();
        }

        protected setPostedAddress(address: string) {
            this._searchBoxJQ.val(address);
        }

        protected findElement(selector: string): JQuery {
            return $(selector);
        }

        protected getContainer(): JQuery {
            return $('body');
        }

        protected getPageSize(): any {
            return this.context.container.data('pagesize');
        }

        private searchBoxSetBounds(bounds: any) {
            var southWest = new google.maps.LatLng(bounds.SouthWest.Lat, bounds.SouthWest.Lng);
            var northEast = new google.maps.LatLng(bounds.NorthEast.Lat, bounds.NorthEast.Lng);
            bounds = new google.maps.LatLngBounds(southWest, northEast);

            this._searchBox.setBounds(bounds);
        }

        protected onMapBoundsUpdated(data?: any, isSearch?: boolean): void {
            clearTimeout(this._timer);
            this._timer = setTimeout(() => {
                this.updateMarkers(data, isSearch);
            }, 750);
        }

        protected onMarkerClick(marker?: Marker): void {
            if (marker != null && marker.storeNumber) {
                this._storeLocatorService.getStore(marker.storeNumber)
                    .then((store) => {

                        this.getCurrentLocation().then(location => {
                            if (location) {
                                store.GoogleDirectionsLink = this._geoService.getDirectionLatLngSourceAddress(store.GoogleDirectionsLink, location);
                            }
                            var content = this.getRenderedTemplateContents('StoreMapMarkerInfo', store);
                            this._mapService.openInformationWindow(content, marker.value);
                        });

                    })
                    .fail(reason => this.handlePromiseFail('StoreLocator OnMarkerClick', reason));
            }
        }

        protected onClusterClick(marker?: Marker): void {
            this._mapService.getInformationWindow().close();
            this._mapService.getMap().panTo(marker.value.getPosition());
            marker.value.setMap(null);
            this._mapService.getMap().setZoom(this._mapService.getMap().getZoom() + 1);
        }

        protected updateMarkers(data?: any, isSearch: boolean = false) {
            let mapBounds = this._mapService.getBounds(this._storeLocatorOptions.markerPadding);
            let zoomLevel = this._mapService.getZoom();
            let searchPoint = this._searchPoint;
            let page = this._isRestoreListPaging ? this._historyState.page : 1;
            let pageSize = page * this.getPageSize();

            this._storeLocatorService.getMarkers(mapBounds.getSouthWest(), mapBounds.getNorthEast(), zoomLevel, searchPoint, isSearch, pageSize)
                .then((result) => {
                    if (result.Lat && result.Lng) {
                        this._mapService.extendBounds(searchPoint, new google.maps.LatLng(result.Lat, result.Lng));
                    } else {
                        this._mapService.setMarkers(result.Markers, isSearch);

                        if (this._isRestoreListPaging && result.NextPage) {
                            result.NextPage.Page = this._historyState.page + 1;
                        }
                        this.VueStoreList.setStoreList(result);
                        if (this._isRestoreListPaging && this._historyState.pos) {
                            $('html, body').animate({
                                scrollTop: this._historyState.pos
                            }, 500);
                            this._historyState.historyPushState({});
                        }
                        this._isRestoreListPaging = false;


                        if (this._storeLocatorOptions.showNearestStoreInfo && result.Stores) {
                            let firstStore = result.Stores[0];
                            if (firstStore && firstStore.SearchIndex === 1) {
                                this.setNearestStoreInfo(firstStore.DestinationToSearchPoint);
                            }
                        }
                    }

                    let center = this._mapService.getMap().getCenter();
                    this._historyState.historyPushState({ page, point: searchPoint, zoom: zoomLevel, center });
                    this._isSearch = false;
                })
                .fail(reason => this.handlePromiseFail('StoreLocator UpdateMarkers getMarkers', reason));
        }

        private setSearchLocationInMap(point: google.maps.LatLng, zoomLevel: number = this._storeLocatorOptions.zoomLevel) {
            this._searchPoint = point;
            let title = this.getPostedAddress();
            this.createSearchPointMarker(point, title);
            this._isSearch = true;
            this.cache.set(this._searchPointAddressCacheKey, title);
            this._mapService.setLocationInMap(point, zoomLevel);
        }

        private createSearchPointMarker(searchPoint: google.maps.LatLng, title: string) {
            if (this._searchPointMarker == null) {
                this._searchPointMarker = this._mapService.createMarkerOnMap(searchPoint, title);
            } else {
                this._searchPointMarker.setPosition(searchPoint);
                this._searchPointMarker.setTitle(title);
            }
        }

        public searchCurrentLocation() {
            this._geoService.geolocate()
                .then(currentLocation => this._geoService.getAddressByLocation(currentLocation)
                    .then( address => {
                        this.setPostedAddress(address);
                        this.eventHub.publish('searchPointChanged', { data: currentLocation });
                    })
                )
                .fail(reason => this.handlePromiseFail('StoreLocator searchCurrentLocation', reason));
        }

        // Remember element position in history
        public rememberPosition() {
            let pos = $(document).scrollTop();
            this._historyState.historyPushState({ pos });
        }

        protected setNearestStoreInfo(info: string) {
            let nearestInfoPanel = this.findElement('#store-locator-nearest');
            if (!nearestInfoPanel.length) { return; }


            if (!this.findElement('#nearestInfo').length) {
                nearestInfoPanel.html(nearestInfoPanel.html().replace('{0}', '<strong id=\'nearestInfo\'></strong>'));
            }

            this.findElement('#nearestInfo').html(info);
            nearestInfoPanel.removeClass('d-none');
        }

        protected getStores(page): Q.Promise<any> {
            this._historyState.historyPushState({ page });
            return this.getStoresForPage(page, this.getPageSize());
        }

        protected getStoresForPage(page: number, pageSize?: number): Q.Promise<any> {
            let mapBounds = this._mapService.getBounds(this._storeLocatorOptions.markerPadding);
            let searchPoint = this._searchPoint;

            return this._storeLocatorService.getStores(mapBounds.getSouthWest(), mapBounds.getNorthEast(), searchPoint, page, pageSize);
        }

        protected setGoogleDirectionLinks(): Q.Promise<any> {

            return this.getCurrentLocation().then(location => {
                this._geoService.updateDirectionLinksWithLatLngSourceAddress(this.getContainer(), location);
            });
        }

        protected restoreMapFromHistoryState() {
            this._searchPoint = this._historyState.point;
            this.createSearchPointMarker(this._historyState.point, this.getPostedAddress());

            if (this._historyState.center) {
                this._mapService.getMap().setCenter(this._historyState.center);
            }

            if (this._historyState.page > 1) {
                this._isRestoreListPaging = true;
            }
        }

        protected handlePromiseFail(title: string, reason: any) {
            if (typeof reason === 'object') {
                console.log(title + ': ' + reason.message);
                if (reason.code === 1) {
                    this.VueStoreList.showStoreLocatorLocationError();
                }
            } else {
                console.log(title + ': ' + reason);
            }
        }
    }
}
