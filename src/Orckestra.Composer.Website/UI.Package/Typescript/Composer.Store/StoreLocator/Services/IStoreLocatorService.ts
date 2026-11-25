///<reference path='../../../../Typings/tsd.d.ts' />
namespace Orckestra.Composer {
    export interface IStoreLocatorService {
        getMapConfiguration(): Promise<any>;

        getStore(storeNumber: string): Promise<any>;

        getStores(southWest: google.maps.LatLng, northEast: google.maps.LatLng, searchPoint: google.maps.LatLng,
            page: number, pageSize: number): Promise<any>;

        getMarkers(southWest: google.maps.LatLng, northEast: google.maps.LatLng,
            zoomLevel: number, searchPoint: google.maps.LatLng, isSearch: boolean, pageSize: number): Promise<any>;
    }
}
