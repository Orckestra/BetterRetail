/// <reference path='../../../../Typings/tsd.d.ts' />
/// <reference path='../../../JQueryPlugins/ISerializeObjectJqueryPlugin.ts' />
/// <reference path='../../../Mvc/Controller.ts' />
/// <reference path='../../../Mvc/IControllerActionContext.ts' />
/// <reference path='../../../Mvc/IControllerContext.ts' />
/// <reference path='../../../Mvc/IControllerActionContext.ts' />
/// <reference path='../../../Events/IEventHub.ts' />
/// <reference path='../../../System/IDisposable.ts' />

module Orckestra.Composer {
    'use strict';

    export class SliderService implements IDisposable {
        private sliderInstance;
        private facetFieldName: string;
        private maxLabel: string;
        private maxValue: number;
        private minValue: number;
        private step;
        private applyButtonContext: JQuery;

        constructor(private context: JQuery, protected eventHub: IEventHub) {
            this.context = context;
        }

        public initialize(selectedValues) {
            this.applyButtonContext = this.context.find(':submit');
            this.mapData(this.context.data());
            this.initializeSlider(selectedValues);
        }

        public dispose() {
            this.sliderInstance.destroy();
        }

        protected mapData(containerData) {
            this.step = containerData.step || 1;
            this.maxLabel = containerData.maxLabel;
            this.maxValue = containerData.max;
            this.minValue = containerData.min;
            this.facetFieldName = containerData.facetfieldname;
        }

        public dirtied() {
            this.applyButtonContext.prop('disabled', false);
        }

        /**
         * Formatting for the formatted values of the slider. When getting.
         */
        public formatFrom(value) {
            if (this.maxLabel && value === this.maxLabel) {
                value = this.maxValue;
            }

            return value;
        }

        /**
         * Formatting for the formatted values of the slider. When setting.
         */
        public formatTo(value) {
            value = parseInt(value, 10) || 0;

            if (this.maxLabel && value === this.maxValue) {
                value = this.maxLabel;
            }

            return value;
        }

        private initializeSlider(facetData) {
            let sliderElement = this.context.find('.range').get(0);
            let defaultRange = [this.minValue, this.maxValue];
            let startRange = defaultRange;
            let selectedRange;
            let lowerRangeContext: JQuery = this.context.find('.js-lowerValue');
            let upperRangeContext: JQuery = this.context.find('.js-higherValue');

            // TODO handle array or not array
            if (facetData) {
                selectedRange = facetData.split('|');
                startRange = defaultRange.map((value, index) => {
                    return selectedRange[index] ? selectedRange[index] : defaultRange[index];
                });
            }

            this.sliderInstance = this.createSlider(startRange, sliderElement);

            this.sliderInstance.on('set', (values, handle) => {
                this.dirtied();
            });

            this.sliderInstance.on('update', (values: Array<string>, handle) => {
                lowerRangeContext.val(values[0]);
                upperRangeContext.val(values[1]);
            });

            lowerRangeContext.on('keyup', event => this.dirtied());
            upperRangeContext.on('keyup', event => this.dirtied());
            lowerRangeContext.on('blur', event => this.sliderInstance.set([$(event.target).val(), null]));
            upperRangeContext.on('blur', event => this.sliderInstance.set([null, $(event.target).val()]));

        }

        private createSlider(startRange, sliderElement) {
            return noUiSlider.create(sliderElement, {
                start: startRange,
                connect: true,
                margin: this.step,
                step: this.step,
                range: {
                    'min': [this.minValue],
                    'max': [this.maxValue]
                },
                format: {
                    to: value => this.formatTo(value),
                    from: value => this.formatFrom(value)
                }
            });
        }

        public getKey() {
            return this.facetFieldName;
        }

        public getValues() {
            let values = <any[]>this.sliderInstance.get();

            if (values[1] === this.maxLabel) {
                values[1] = undefined;
            }

            return values;
        }
    }
}
