///<reference path='../../../../Typings/tsd.d.ts' />
///<reference path='../../../../Typings/vue/index.d.ts' />

module Orckestra.Composer {
    'use strict';

    export class StepperStepVueComponent {
        static componentMame: string = 'stepper-step';

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

                    /***
                     * Function to execute for reset changes in completed next steps
                     */
                    resetChanges: {
                        type: Function
                    },

                    fulfilled: Boolean,
                    loading: Boolean,
                    title: String,
                    mobileTitle: String
                },
                inject: ['addStep', 'removeStep', 'resetNextSteps', 'navigateToStep'],
                data() {
                    return {
                        active: false,
                        index: null,
                        validationError: null,
                        checked: false,
                        stepId: ''
                    };
                },
                computed: {
                    slotProps() {
                        return {
                            nextStep: this.nextStep,
                            prevStep: this.prevStep,
                            resetNextSteps: () => this.resetNextSteps(this.index),
                            selectStep: () => this.navigateToStep(this.index),
                            navigateToStep: this.navigateToStep,
                            activeStepIndex: this.$parent.activeStepIndex,
                            isLastStep: this.$parent.isLastStep,
                            index: this.index,
                            active: this.active,
                            show: this.active || (this.$parent.showPreviousSteps && this.$parent.activeStepIndex > this.index),
                            passed: this.fulfilled && (!this.active || this.$parent.completed),
                            next: this.index === this.$parent.activeStepIndex + 1
                        };
                    },

                },
                methods: {
                    nextStep() {
                        this.navigateToStep(this.index + 1)
                    },
                    prevStep() {
                        this.navigateToStep(this.index - 1)
                    }
                },
                mounted() {
                    this.addStep(this);
                    if (this.fulfilled === undefined) {
                        this.fulfilled = true;
                    }
                },
                destroyed() {
                    if (this.$el && this.$el.parentNode) {
                        this.$el.parentNode.removeChild(this.$el);
                    }
                    this.removeStep(this);
                },
                template: `
                    <div class="stepper-step-container"
                         v-bind:class="{
                             'active-step': active,
                             'fulfilled-step' : fulfilled,
                             'next-step': slotProps.next,
                             'loading' : loading
                         }"
                         role="tabpanel"
                         v-bind:id="stepId"
                         v-show="slotProps.show">
                        <div class="loading-spinner">
                            <div class="spinner-border text-info" role="status">
                                <span class="sr-only">Loading...</span>
                            </div>
                        </div>
                        <slot v-bind="slotProps" ></slot>
                    </div>
                `
            };
        }
    }
}
