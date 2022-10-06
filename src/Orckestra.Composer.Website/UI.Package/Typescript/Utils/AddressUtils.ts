/// <reference path='../../Typings/tsd.d.ts' />

module Orckestra.Composer {

    'use strict';

    export class AddressUtils {

        /**
         * Get current website id
         */
        public static isEquals(addr1, addr2): boolean {
            return (addr1.FirstName === addr2.FirstName) &&
                (addr1.LastName === addr2.LastName) &&
                (addr1.Line1 === addr2.Line1) &&
                (addr1.Line2 === addr2.Line2) &&
                (addr1.City === addr2.City) &&
                (addr1.RegionCode === addr2.RegionCode) &&
                (addr1.PostalCode === addr2.PostalCode) &&
                (addr1.PhoneNumber === addr2.PhoneNumber) &&
                (addr1.CountryCode === addr2.CountryCode);
        }
    }
}
