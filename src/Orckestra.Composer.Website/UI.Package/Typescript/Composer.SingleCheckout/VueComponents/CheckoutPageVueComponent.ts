///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../../Typings/vue/index.d.ts' />
///<reference path='./CheckoutHelpers.ts' />

module Orckestra.Composer {
    'use strict';

    export class CheckoutPageVueComponent {
        static  componentMame: string = 'checkout-page';
        static initialize() {
            Vue.component(this.componentMame, this.getComponent());
        }

        static getComponent() {
            return {
                components: {},
                props: {
                    id: {
                        type: String,
                        default: 'fw_' + new Date().valueOf()
                    },
                    validateOnBack: Boolean,

                    /***
                     * Applies to text, border and circle
                     */
                    color: {
                        type: String,
                        default: '#e74c3c'
                    },
                    errorColor: {
                        type: String,
                        default: '#8b0000'
                    },
                    shape: {
                        type: String,
                        default: 'circle'
                    },
                    layout: {
                        type: String,
                        default: 'horizontal'
                    },
                    stepsClasses: {
                        type: [String, Array],
                        default: ''
                    },
                    stepSize: {
                        type: String,
                        default: 'md',
                        validator: function validator(value) {
                            var acceptedValues = ['xs', 'sm', 'md', 'lg'];
                            return acceptedValues.indexOf(value) !== -1;
                        }
                    },
                    /***
                     *
                     * Index of the initial tab to display
                     */
                    startIndex: {
                        type: Number,
                        default: 0,
                        validator: function validator(value) {
                            return value >= 0;
                        }
                    }
                },
                provide: function provide() {
                    return {
                        addStep: this.addStep,
                        removeStep: this.removeStep,
                        nextStep: this.nextStep
                    };
                },
                data: function data() {
                    return {
                        activeStepIndex: 0,
                        currentPercentage: 0,
                        maxStep: 0,
                        loading: false,
                        steps: []
                    };
                },
                computed: {
                    slotProps: function slotProps() {
                        return {
                            nextStep: this.nextStep,
                            prevStep: this.prevStep,
                            activeStepIndex: this.activeStepIndex,
                            isLastStep: this.isLastStep,
                            fillButtonStyle: this.fillButtonStyle
                        };
                    },
                    stepCount: function stepCount() {
                        return this.steps.length;
                    },
                    isLastStep: function isLastStep() {
                        return this.activeStepIndex === this.stepCount - 1;
                    },
                    isVertical: function isVertical() {
                        return this.layout === 'vertical';
                    },
                    displayPrevButton: function displayPrevButton() {
                        return this.activeStepIndex !== 0;
                    },
                    stepPercentage: function stepPercentage() {
                        return 1 / (this.stepCount * 2) * 100;
                    },
                    fillButtonStyle: function fillButtonStyle() {
                        return {
                            backgroundColor: this.color,
                            borderColor: this.color,
                            color: 'white'
                        };
                    }
                },
                methods: {
                    emitTabChange: function emitTabChange(prevIndex, nextIndex) {
                        this.$emit('on-change', prevIndex, nextIndex);
                        this.$emit('update:startIndex', nextIndex);
                    },
                    addStep: function addStep(item) {
                        var index = this.$slots.default.filter(function (d) {
                            return item.$vnode.tag === d.tag;
                        }).indexOf(item.$vnode);
                        this.steps.splice(index, 0, item); // if a step is added before the current one, go to it

                        if (index < this.activeStepIndex + 1) {
                            this.maxStep = index;
                            this.changeStep(this.activeStepIndex + 1, index);
                        }

                        item.index = this.steps.indexOf(item);
                        item.stepId = 'step' + index;
                        this.maxStep = this.steps.length - 1; //TODO: fix it
                    },
                    removeStep: function removeStep(item) {
                        var tabs = this.steps;
                        var index = tabs.indexOf(item);

                        if (index > -1) {
                            // Go one step back if the current step is removed
                            if (index === this.activeStepIndex) {
                                this.maxStep = this.activeStepIndex - 1;
                                this.changeStep(this.activeStepIndex, this.activeStepIndex - 1);
                            }

                            if (index < this.activeStepIndex) {
                                this.maxStep = this.activeStepIndex - 1;
                                this.activeStepIndex = this.activeStepIndex - 1;
                                this.emitTabChange(this.activeStepIndex + 1, this.activeStepIndex);
                            }

                            tabs.splice(index, 1);
                        }
                    },
                    reset: function reset() {
                        this.maxStep = 0;
                        this.steps.forEach(function (tab) {
                            tab.checked = false;
                        });
                        this.navigateToStep(0);
                    },
                    activateAll: function activateAll() {
                        this.maxStep = this.steps.length - 1;
                        this.steps.forEach(function (tab) {
                            tab.checked = true;
                        });
                    },
                    navigateToStep: function navigateToStep(index) {
                        var validate = index > this.activeStepIndex;

                        if (index <= this.maxStep) {
                            let cb = () => {
                                if (validate && index - this.activeStepIndex > 1) {
                                    // validate all steps recursively until destination index
                                    this.changeStep(this.activeStepIndex, this.activeStepIndex + 1);
                                    this.beforeStepChange(this.activeStepIndex, cb);
                                } else {
                                    this.beforeStepEnter(index);
                                    this.changeStep(this.activeStepIndex, index);
                                    this.scrollToStep(index);
                                    this.afterStepChange(this.activeStepIndex);
                                }
                            };
                            if (validate) {
                                this.beforeStepChange(this.activeStepIndex, cb);
                            } else {
                                // when trying leave already savedd step(edit mode) when edit it we need to validate it
                                let step = this.steps[this.activeStepIndex + 1];
                                if (step && step.fulfilled) {
                                    this.beforeStepChange(this.activeStepIndex, cb);
                                } else {
                                    this.setValidationError(null);
                                    cb();
                                }
                            }
                        }

                        return index <= this.maxStep;
                    },
                    nextStep: function nextStep() {
                        let cb = () => {
                            if (this.activeStepIndex < this.stepCount - 1) {
                                this.changeStep(this.activeStepIndex, this.activeStepIndex + 1);

                                this.afterStepChange(this.activeStepIndex);
                            } else {
                                this.$emit('on-complete');
                            }
                        };

                        this.beforeStepChange(this.activeStepIndex, cb);
                    },
                    prevStep: function prevStep() {
                        let cb = () => {
                            if (this.activeStepIndex > 0) {
                                this.setValidationError(null);

                                this.changeStep(this.activeStepIndex, this.activeStepIndex - 1);
                            }
                        };

                        if (this.validateOnBack) {
                            this.beforeStepChange(this.activeStepIndex, cb);
                        } else {
                            cb();
                        }
                    },
                    focusnextStep: function focusnextStep() {
                        var tabIndex = CheckoutHelpers.getFocusedStepIndex(this.steps);

                        if (tabIndex !== -1 && tabIndex < this.steps.length - 1) {
                            var tabToFocus = this.steps[tabIndex + 1];

                            if (tabToFocus.checked) {
                                CheckoutHelpers.findElementAndFocus(tabToFocus.stepId);
                            }
                        }
                    },
                    focusprevStep: function focusprevStep() {
                        var tabIndex = CheckoutHelpers.getFocusedStepIndex(this.steps);

                        if (tabIndex !== -1 && tabIndex > 0) {
                            var toFocusId = this.steps[tabIndex - 1].stepId;
                            CheckoutHelpers.findElementAndFocus(toFocusId);
                        }
                    },
                    setLoading: function setLoading(value) {
                        this.loading = value;
                        this.$emit('on-loading', value);
                    },
                    setValidationError: function setValidationError(error) {
                        this.steps[this.activeStepIndex].validationError = error;
                        this.$emit('on-error', error);
                    },
                    validateBeforeChange: function validateBeforeChange(promiseFn, callback) {
                        this.setValidationError(null); // we have a promise

                        if (CheckoutHelpers.isPromise(promiseFn)) {
                            this.setLoading(true);
                            promiseFn.then((res) => {
                                this.setLoading(false);

                                var validationResult = res === true;

                                this.executeBeforeChange(validationResult, callback);
                            }).catch((error) => {
                                this.setLoading(false);

                                this.setValidationError(error);
                            }); // we have a simple function
                        } else {
                            var validationResult = promiseFn === true;
                            this.executeBeforeChange(validationResult, callback);
                        }
                    },
                    executeBeforeChange: function executeBeforeChange(validationResult, callback) {
                        this.$emit('on-validate', validationResult, this.activeStepIndex);

                        if (validationResult) {
                            callback();
                        } else {
                            this.steps[this.activeStepIndex].validationError = 'error';
                        }
                    },
                    beforeStepChange: function beforeStepChange(index, callback) {
                        //alert('before-change-' + index);
                        if (this.loading) {
                            return;
                        }

                        var oldStep = this.steps[index];

                        if (oldStep && oldStep.beforeChange !== undefined) {
                            var stepChangeRes = oldStep.beforeChange();
                            this.validateBeforeChange(stepChangeRes, callback);
                        } else {
                            callback();
                        }
                    },
                    beforeStepEnter: function beforeStepEnter(index) {
                        if (this.loading) {
                            return;
                        }

                        var newStep = this.steps[index];

                        if (newStep && newStep.beforeEnter !== undefined) {
                            newStep.beforeEnter();
                        }
                    },
                    afterStepChange: function afterStepChange(index) {
                        if (this.loading) {
                            return;
                        }

                        var newStep = this.steps[index];

                        if (newStep && newStep.afterChange !== undefined) {
                            newStep.afterChange();
                        }
                    },
                    changeStep: function changeStep(oldIndex, newIndex) {
                        var emitChangeEvent = arguments.length > 2 && arguments[2] !== undefined ? arguments[2] : true;
                        var oldStep = this.steps[oldIndex];
                        var newStep = this.steps[newIndex];

                        if (oldStep) {
                            oldStep.active = false;
                        }

                        if (newStep) {
                            newStep.active = true;
                        }

                        if (emitChangeEvent && this.activeStepIndex !== newIndex) {
                            this.emitTabChange(oldIndex, newIndex);
                        }

                        this.activeStepIndex = newIndex;
                        this.activateStepAndCheckStep(this.activeStepIndex);
                        return true;
                    },
                    scrollToStep: function scrollToStep(stepIndex) {
                        let stepId = this.steps[stepIndex].stepId;
                        setTimeout(function () {
                            $('html, body').animate({
                                scrollTop: $('#' + stepId).offset().top
                            }, 500);
                        }, 200);
                    },
                    deactivateSteps: function deactivateSteps() {
                        this.steps.forEach(function (step) {
                            step.active = false;
                        });
                    },
                    activateStep: function activateStep(index) {
                        this.deactivateSteps();
                        var step = this.steps[index];

                        if (step) {
                            step.active = true;
                            step.checked = true;
                        }
                    },
                    activateStepAndCheckStep: function activateStepAndCheckStep(index) {
                        this.activateStep(index);

                        if (index > this.maxStep) {
                            this.maxStep = index;
                        }

                        this.activeStepIndex = index;
                    },
                    initializeSteps: function initializeSteps() {
                        if (this.steps.length > 0 && this.startIndex === 0) {
                            this.activateStep(this.activeStepIndex);
                        }

                        if (this.startIndex < this.steps.length) {
                            this.activateStepAndCheckStep(this.startIndex);
                        } else {
                            window.console.warn(`Prop startIndex set to ${this.startIndex} is greater than the number of steps - ${this.steps.length}. Make sure that the starting index is less than the number of tabs registered`);
                        }
                    },
                },
                mounted: function mounted() {
                    this.initializeSteps();
                },
                template: `
                    <div :id="id ? id : ''" class="single-page-checkout" :class="[stepSize, {vertical: isVertical}]" >
                        <div class="wizard-tab-content">
                          <slot v-bind="slotProps">
                          </slot>
                        </div>
                    </div>`
            };
        }
    }
}
