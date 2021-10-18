/// <reference path='../../../Typings/tsd.d.ts' />
/// <reference path='../../../Typescript/Utils/PriceHelper.ts' />

module Orckestra.Composer {
    'use strict';

(() => {

    describe('WHEN calling the priceHelper.pricePerUnit method', () => {
        describe('WITH not empty conversion params', () => {
            it('SHOULD convert to price per unit to $12.00', () => {
                //Arrange
                var price = '$12.00';
                var productUnitQuantity = '1';
                var productUnitSize = '1';
                var convertedVolumeMeasurement = '1';
                //Act
                var value = PriceHelper.PricePerUnit(price, productUnitQuantity, productUnitSize, convertedVolumeMeasurement );

                //Assert
                expect(value).toBe('$12.00');
            });

            it('SHOULD convert to price per unit to rounded $0.02', () => {
                //Arrange
                var price = '$3.00';
                var productUnitQuantity = '1';
                var productUnitSize = '200';
                var convertedVolumeMeasurement = '1';
                //Act
                var value = PriceHelper.PricePerUnit(price, productUnitQuantity, productUnitSize, convertedVolumeMeasurement );

                //Assert
                expect(value).toBe('$0.02');
            });

            it('SHOULD convert to price per unit to $16.00', () => {
                //Arrange
                var price = '$12.00';
                var productUnitQuantity = '1';
                var productUnitSize = '750';
                var convertedVolumeMeasurement = '1000';
                //Act
                var value = PriceHelper.PricePerUnit(price, productUnitQuantity, productUnitSize, convertedVolumeMeasurement );

                //Assert
                expect(value).toBe('$16.00');
            });
        });
        describe('WITH empty conversion params', () => {
            it('SHOULD convert to price per unit to undefined with convertedVolumeMeasurement 1', () => {
                //Arrange
                var price = '$12';
                var productUnitQuantity = '0';
                var productUnitSize = '0';
                var convertedVolumeMeasurement = '1';
                //Act
                var value = PriceHelper.PricePerUnit(price, productUnitQuantity, productUnitSize, convertedVolumeMeasurement );

                //Assert
                expect(value).toBe(undefined);
            });
            it('SHOULD convert to price per unit to  to undefined with convertedVolumeMeasurement 0', () => {
                //Arrange
                var price = '$12';
                var productUnitQuantity = '0';
                var productUnitSize = '0';
                var convertedVolumeMeasurement = '0';
                //Act
                var value = PriceHelper.PricePerUnit(price, productUnitQuantity, productUnitSize, convertedVolumeMeasurement );

                //Assert
                expect(value).toBe(undefined);
            });

            it('Should return undefined for undefined parameters', () => {
                //Arrange
                var price, productUnitQuantity , productUnitSize, convertedVolumeMeasurement ;
                //Act
                var value = PriceHelper.PricePerUnit(price, productUnitQuantity, productUnitSize, convertedVolumeMeasurement );

                //Assert
                expect(value).toBe(undefined);
            });
            it('Should return undefined for null parameters', () => {
                //Arrange
                var price = null;
                var productUnitQuantity = null;
                var productUnitSize = null;
                var convertedVolumeMeasurement = null;
                //Act
                var value = PriceHelper.PricePerUnit(price, productUnitQuantity, productUnitSize, convertedVolumeMeasurement );

                //Assert
                expect(value).toBe(undefined);
            });
        });
    });
})()
};