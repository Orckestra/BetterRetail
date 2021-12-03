module Orckestra.Composer {

    'use strict';

    export  class PriceHelper  {
        public static  PricePerUnit(price: string,
            productUnitQuantity: string,
            productUnitSize: string,
            convertedVolumeMeasurement: string) : any  {
            if(!convertedVolumeMeasurement || convertedVolumeMeasurement === '0' || 
                !productUnitQuantity || productUnitQuantity === '0' ||
                !productUnitSize || productUnitSize === '0' ) return;

            let priceNum = parseFloat(price.replace(/[^0-9\.-]+/g,""));
            let stepOne = priceNum / parseFloat(productUnitQuantity);
            let stepTwo = stepOne / parseFloat(productUnitSize);
            let pricePerUnit = stepTwo * parseFloat(convertedVolumeMeasurement);
            let pricePerUnitRounded = (Math.round( pricePerUnit * 100 ) / 100).toFixed(2);
            
            let formatedPrice = price.replace(priceNum.toFixed(2), pricePerUnitRounded);
            return  formatedPrice;
        }

        public static HasUnitValues(product: any) {
            return (product.ProductUnitQuantity > 0) && (product.ProductUnitSize > 0) && (product.ProductUnitMeasure != null);
        }

        public static IsPricePerUnitZero(price: any) {
            return parseFloat(price.replace(/[^0-9\.-]+/g,'')) == 0.00;
        }
                              
    };
}
