///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../../Typings/vue/index.d.ts' />

module Orckestra.Composer {
    'use strict';

    export class CheckoutStepVueComponent {
        static  componentMame: string = 'checkout-step';
        static initialize() {
            Vue.component(this.componentMame, this.getComponent());
        }

        static getComponent() {
            return {
                props: {
                    /***
                     * Function to execute before tab switch. Return value must be boolean
                     * If the return result is false, tab switch is restricted
                     */
                    beforeChange: {
                        type: Function
                    },

                    /***
                     * Function to execute before tab enter. Return value must be boolean
                     * If the return result is false, tab switch is restricted
                     */
                    beforeEnter: {
                        type: Function
                    },
                    /***
                     * Function to execute after tab switch. Return void for now.
                     * Safe to assume necessary validation has already occured
                     */
                    afterChange: {
                        type: Function
                    },

                    fulfilled: Boolean,
                    loading: Boolean
                },
                inject: ['addStep', 'removeStep', 'nextStep'],
                data: function () {
                    return {
                        active: false,
                        index: null,
                        validationError: null,
                        checked: false,
                        stepId: ''
                    };
                },
                computed: {
                    slotProps: function () {
                        var self = this;
                        return {
                            nextStep: this.$parent.nextStep,
                            prevStep: this.$parent.prevStep,
                            navigateToStep: this.$parent.navigateToStep,
                            activeStepIndex: this.$parent.activeStepIndex,
                            isLastStep: this.$parent.isLastStep,
                            index: this.index,
                            active: this.active,
                            displayContinueButton: (!this.fulfilled && ((this.$parent.activeStepIndex + 1) === this.index)),
                            selectStep: function () {
                                this.$parent.navigateToStep(this.index);
                            },
                            preview: this.fulfilled && !this.active,
                            next: this.index === this.$parent.activeStepIndex + 1
                        };
                    },

                },
                methods: {},
                mounted: function () {
                    this.addStep(this);
                    if (this.fulfilled === undefined) {
                        this.fulfilled = true;
                    }
                },
                destroyed: function () {
                    if (this.$el && this.$el.parentNode) {
                        this.$el.parentNode.removeChild(this.$el);
                    }
                    this.removeStep(this);
                },
                template: `
                    <div class="checkout-step-container"
                         v-bind:class="{'active-step': active,
                         'fulfilled-step' : fulfilled,
                         'preview-step': slotProps.preview,
                         'next-step': slotProps.next,
                         'loading' : loading
                        }"
                         role="tabpanel"
                         v-bind:id="stepId">
                         <div class="loading-spinner">
                            <div class="spinner-border text-info" role="status">
                                <span class="sr-only">Loading...</span>
                            </div>
                         </div>
                        <slot v-bind="slotProps"></slot>
                    </div>`
            };
        }
    }
}
