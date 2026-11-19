///<reference path='./IAnalyticsCoupon.ts' />
namespace Orckestra.Composer {
    'use strict';
    export interface IAnalyticsOrder {
        id?: string;
        affiliation?: string;
        revenue?: string;
        tax?: string;
        shipping?: string;
        currencyCode?: string;
        coupon?: string;
    }
}