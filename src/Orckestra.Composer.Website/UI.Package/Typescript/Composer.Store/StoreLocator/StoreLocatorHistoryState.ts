///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='./IStoreLocatorHistoryState.ts' />

module Orckestra.Composer {
    'use strict';

    export class StoreLocatorHistoryState implements IStoreLocatorHistoryState {
        point: google.maps.LatLng;
        page: number;
        zoom: number;
        center: google.maps.LatLng;
        pos: number;

        public parseHistoryState() {
            if (!history.state) { return; }

            if (history.state.p_lat && history.state.p_lng) {
                this.point = new google.maps.LatLng(history.state.p_lat, history.state.p_lng);
            }

            if (history.state.c_lat && history.state.c_lng) {
                this.center = new google.maps.LatLng(history.state.c_lat, history.state.c_lng);
            }

            this.zoom = history.state.zoom;
            this.page = history.state.page;
            this.pos = history.state.pos;
        }

        public isDefined = (): boolean => !!this.point;

        public historyPushState(update: IStoreLocatorHistoryState) {
            if (update.page) {
                this.page = update.page;
            }
            if (update.point) {
                this.point = update.point;
            }
            if (update.zoom) {
                this.zoom = update.zoom;
            }
            if (update.center) {
                this.center = update.center;
            }
            if (update.pos >= 0) {
                this.pos = update.pos;
            }

            if (this.point) {
                let obj = {
                    'p_lat': this.point.lat(),
                    'p_lng': this.point.lng(),
                    'page': this.page,
                    'zoom': this.zoom,
                    'c_lat': this.center.lat(),
                    'c_lng': this.center.lng(),
                    'pos': this.pos
                };

                if (history.state) {
                    history.replaceState(obj, null, null);
                } else {
                    history.pushState(obj, null, null);
                }
            }
        }
    }
}
