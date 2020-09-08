///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../../Typings/vue/index.d.ts' />

module Orckestra.Composer {
    'use strict';

    export class CheckoutStepVueComponent {
        static componentMame: string = 'checkout-step';

        static initialize() {
            Vue.component(this.componentMame, this.getComponent());
        }

        static getComponent() {
            return {
                props: {
                    /***
                     * Function to execute before step switch. Return value must be boolean
                     * If the return result is false, step switch is restricted
                     */
                    beforeChange: {
                        type: Function
                    },

                    /***
                     * Function to execute before step enter. Return value must be boolean
                     * If the return result is false, step switch is restricted
                     */
                    beforeEnter: {
                        type: Function
                    },
                    /***
                     * Function to execute after step switch. Return void for now.
                     * Safe to assume necessary validation has already occured
                     */
                    afterChange: {
                        type: Function
                    },
                    /***
                     * Function to determine if step fulfilled so next step can be switched. Return value must be boolean
                     */

                    fulfilled:  {
                        type: Boolean,
                        default: true
                    },
                    /***
                     * Property to determine if this step is loading. It is used to show loading spinner over the step container
                     */
                    loading: Boolean
                },
                inject: ['addStep', 'removeStep', 'nextStep', 'isStepExist', 'nextStepId', 'getPrevStepInstance'],
                data() {
                    return {
                        active: false,
                        id: null,
                        validationError: null,
                        checked: false,
                        elementId: ''
                    };
                },
                computed: {
                    slotProps() {
                        return {
                            nextStep: this.$parent.nextStep,
                            prevStep: this.$parent.prevStep,
                            navigateToStep: this.$parent.navigateToStep,
                            activeStepId: this.$parent.activeStepId,
                            isLastStep: this.$parent.isLastStep,
                            id: this.id,
                            active: this.active,
                            displayContinueButton: (!this.checked && (this.nextStepId() === this.id)),
                            selectStep: function () {
                                this.$parent.navigateToStep(this.id);
                            },
                            preview: this.fulfilled && this.checked && !this.active,
                            next: this.id === this.nextStepId(),
                            show: this.isStepExist(this),
                            prevFulfilled: (() => {
                                let prevStep = this.getPrevStepInstance(this.id);
                                return prevStep && prevStep.fulfilled
                            })()
                        };
                    },

                },
                methods: {},
                mounted() {
                    this.addStep(this);
                },
                destroyed() {
                    if (this.$el && this.$el.parentNode) {
                        this.$el.parentNode.removeChild(this.$el);
                    }
                    this.removeStep(this);
                },
                template: `
                    <div class="checkout-step-container"
                    	 v-show="slotProps.show"
                         v-bind:class="{'active-step': active,
                         'fulfilled-step' : fulfilled,
                         'preview-step': slotProps.preview,
                         'next-step': slotProps.next,
                         'loading' : loading
                        }"
                         role="tabpanel"
                         v-bind:id="elementId">
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
