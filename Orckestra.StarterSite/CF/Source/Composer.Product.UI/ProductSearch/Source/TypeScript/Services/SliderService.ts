/// <reference path='../../../../../Composer.UI/Source/Typings/tsd.d.ts' />
/// <reference path='../../../../../Composer.UI/Source/TypeScript/JqueryPlugins/ISerializeObjectJqueryPlugin.ts' />
/// <reference path='../../../../../Composer.UI/Source/TypeScript/Mvc/Controller.ts' />
/// <reference path='../../../../../Composer.UI/Source/TypeScript/Mvc/IControllerActionContext.ts' />
/// <reference path='../../../../../Composer.UI/Source/TypeScript/Mvc/IControllerContext.ts' />
///<reference path='../../../../../Composer.UI/Source/TypeScript/Mvc/IControllerActionContext.ts' />
/// <reference path='../../../../../Composer.UI/Source/TypeScript/Events/IEventHub.ts' />
/// <reference path='../../../../../Composer.UI/Source/TypeScript/System/IDisposable.ts' />

module Orckestra.Composer {
    'use strict';

    export class SliderService implements IDisposable {
        private sliderInstance: noUiSlider.noUiSlider;
        private facetFieldName: string;
        private maxLabel: string;
        private maxValue: string;
        private minValue: string;
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
            var sliderElement = this.context.find('.range').get(0);
            var defaultRange = [this.minValue, this.maxValue];
            var startRange = defaultRange;
            var selectedRange;
            var lowerRangeContext: JQuery  = this.context.find('.js-lowerValue');
            var upperRangeContext: JQuery = this.context.find('.js-higherValue');
            var parentPanel$ = $(sliderElement).closest('.panel');

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

            this.waitForHandles('.noUi-handle', () => {
                $(sliderElement).find('.noUi-handle').each((i, ele) => {
                    this.setupHandle($(sliderElement), $(ele), i, parentPanel$.attr('data-min'), parentPanel$.attr('data-max'));
                });
            });

        }

        private waitForHandles(selector, callback) {
            let that = this;
            if ($(selector).length) {
                callback();
            } else {
                setTimeout(function () {
                    that.waitForHandles(selector, callback);
                }, 100);
            }
        };

        private setSliderHandle(slider, i, value) {
            let r = [null, null];
            r[i] = value;
            slider['0'].noUiSlider.set(r);
        }

        private setHandleValueNow(handle, handleIndex, min, max) {
            let value;
            if (handleIndex === 0) {
                value = $('.js-lowerValue').val();
            } else {
                value = $('.js-higherValue').val();
                if (isNaN(value)) {
                    value = max;
                }
            }
            handle.attr('aria-valuenow', value);
        }

        private setupHandle(slider, handle, handleIndex, min, max) {
            let that = this;
            // adds aria attributes
            handle.attr('tabindex', 0);
            handle.attr('role', 'slider');
            handle.attr('aria-valuemin', min);
            handle.attr('aria-valuemax', max);
            that.setHandleValueNow(handle, handleIndex, min, max);
            if (handleIndex === 0) {
                $('.js-lowerValue').on('blur', event => handle.attr('aria-valuenow', $(event.target).val()));
            } else {
                $('.js-higherValue').on('blur', event => handle.attr('aria-valuenow', $(event.target).val()));
            }

            // handles keyboard updates
            // see http://refreshless.com/nouislider/examples/#section-keypress
            handle.on('keydown', function (event) {
                let value = Number((handleIndex === 0) ? $('.js-lowerValue').val() : $('.js-higherValue').val()),
                    newValue = 0,
                    steps = slider[0].noUiSlider.steps(),
                    handleSteps = steps[handleIndex],
                    haveMatch = true;

                if (isNaN(value)) {
                    value = max;
                }

                switch (event.which) {
                    case 13:
                        let button$ = handle.closest('fieldset').find('button');
                        if (button$.prop('disabled', false)) {
                            button$.trigger('click');
                        }
                        break;

                    case 40: // down
                    case 37: // left
                        // decrements value by a single step
                        newValue = value - handleSteps[0];
                        that.setSliderHandle(slider, handleIndex, newValue);
                        //slider.val(value - handleSteps[0]);
                        break;

                    case 38: // up
                    case 39: // right
                        // increments value by a single step
                        newValue = value + handleSteps[1];
                        that.setSliderHandle(slider, handleIndex, newValue);
                        //slider.val(value + handleSteps[1]);
                        break;

                    case 34: // page down
                        // decrements value by 10 steps
                        newValue = value - handleSteps[0] * 10;
                        that.setSliderHandle(slider, handleIndex, newValue);
                        //slider.val(value - (handleSteps[0] * 10));
                        break;

                    case 33: // page up
                        // increments value by 10 steps
                        newValue = value + handleSteps[1] + 10;
                        that.setSliderHandle(slider, handleIndex, newValue);
                        //slider.val(value + (handleSteps[1] * 10));
                        break;

                    case 36: // home
                        newValue = min;
                        that.setSliderHandle(slider, handleIndex, newValue);
                        //slider.val(min);
                        break;

                    case 35: // end
                        newValue = max;
                        that.setSliderHandle(slider, handleIndex, newValue);
                        //slider.val(max);
                        break;

                    default:
                        haveMatch = false;
                        return;
                }

                if (haveMatch) {
                    that.setHandleValueNow(handle, handleIndex, min, max);
                }

                event.preventDefault();
            });
        }

        private createSlider(startRange, sliderElement): noUiSlider.noUiSlider {
           noUiSlider.create(sliderElement, {
                start: startRange,
                connect: true,
                margin: this.step,
                step: this.step,
                range: {
                    'min': this.minValue,
                    'max': this.maxValue
                },
                format: {
                    to: value => this.formatTo(value),
                    from: value => this.formatFrom(value)
                }
            });

            return (<noUiSlider.Instance>sliderElement).noUiSlider;
        }

        public getKey() {
            return this.facetFieldName;
        }

        public getValues() {
            var values = <any[]>this.sliderInstance.get();

            if (values[1] === this.maxLabel) {
                values[1] = undefined;
            }

            return values;
        }
    }
}
