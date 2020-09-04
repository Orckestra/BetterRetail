///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../../Typings/vue/index.d.ts' />
///<reference path='./CheckoutHelpers.ts' />

module Orckestra.Composer {
    'use strict';

    export class CheckoutPageVueComponent {
        static componentMame: string = 'checkout-page';

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
                     * Index of the initial step to display
                     */
                    startIndex: {
                        type: Number,
                        default: 0,
                        validator: function validator(value) {
                            return value >= 0;
                        }
                    }
                },
                provide() {
                    return {
                        addStep: this.addStep,
                        removeStep: this.removeStep,
                        nextStep: this.nextStep,
                        isStepExist: this.isStepExist,
                        nextStepId: this.nextStepId,
                        getPrevStepInstance: this.getPrevStepInstance
                    };
                },
                data: function data() {
                    return {
                        activeStepId: 0,
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
                            activeStepId: this.activeStepId,
                            isLastStep: this.isLastStep,
                            fillButtonStyle: this.fillButtonStyle
                        };
                    },
                    stepCount: function stepCount() {
                        return this.steps.length;
                    },
                    isLastStep: function isLastStep() {
                        return this.activeStepId === this.stepCount - 1;
                    },
                    isVertical: function isVertical() {
                        return this.layout === 'vertical';
                    },
                    displayPrevButton: function displayPrevButton() {
                        return this.activeStepId !== 0;
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
                    },
                },
                methods: {
                    getStepInstance(id) {
                        return this.steps.find(step => step.id === id);
                    },
                    getNextStepInstance(id) {
                        let listIndex = this.steps.findIndex(step => step.id === id);
                        return this.steps[listIndex + 1];
                    },
                    getPrevStepInstance(id) {
                        let listIndex = this.steps.findIndex(step => step.id === id);
                        return this.steps[listIndex - 1];
                    },
                    nextStepId() {
                        let nextStep = this.getNextStepInstance(this.activeStepId);
                        return nextStep && nextStep.id;
                    },
                    emitStepChange(prevIndex, nextIndex) {
                        this.$emit('on-change', prevIndex, nextIndex);
                        this.$emit('update:startIndex', nextIndex);
                    },
                    addStep(item) {
                        const index = this.$slots.default.filter(d => item.$vnode.tag === d.tag)
                            .indexOf(item.$vnode);
                        this.steps.splice(index, 0, item); // if a step is added before the current one, go to it

                        if (index < this.activeStepId + 1) {
                            this.maxStep = index;
                            this.changeStep(this.getNextStepInstance(this.activeStepId), this.getStepInstance(index));
                        }

                        item.id = this.steps.indexOf(item);
                        item.elementId = 'step' + index;
                        this.maxStep = this.steps.length - 1; //TODO: fix it
                    },
                    removeStep(item) {
                        let index = this.steps.indexOf(item);

                        if (index > -1) {
                            // Go one step back if the current step is removed
                            if (this.steps[index].id === this.activeStepId) {
                                this.changeStep(this.getStepInstance(this.activeStepId), this.getPrevStepInstance(this.activeStepId));
                            }

                            this.maxStep = this.steps.length - 1;

                            this.steps.splice(index, 1);
                        }
                    },
                    isStepExist(step) {
                        return this.steps.indexOf(step) >= 0;
                    },
                    reset() {
                        this.maxStep = 0;
                        this.steps.forEach(function (tab) {
                            tab.checked = false;
                        });
                        this.navigateToStep(0);
                    },
                    activateAll() {
                        this.maxStep = this.steps.length - 1;
                        this.steps.forEach(step => step.checked = true);
                    },
                    navigateToStep(id) {
                        let validate = id > this.activeStepId;

                        if (id <= this.maxStep) {
                            let cb = () => {
                                if (validate && id - this.activeStepId > 1) {
                                    // validate all steps recursively until destination id
                                    this.changeStep(this.getStepInstance(this.activeStepId), this.getNextStepInstance(this.activeStepId));
                                    this.beforeStepChange(this.activeStepId, cb);
                                } else {
                                    this.beforeStepEnter(id);
                                    this.changeStep(this.getStepInstance(this.activeStepId), this.getStepInstance(id));
                                    this.scrollToStep(id);
                                    this.afterStepChange(this.activeStepId);
                                }
                            };
                            if (validate) {
                                this.beforeStepChange(this.activeStepId, cb);
                            } else {
                                // when trying leave already saved step(edit mode) when edit it we need to validate it
                                let step = this.getNextStepInstance(this.activeStepId);
                                if (step && step.fulfilled) {
                                    this.beforeStepChange(this.activeStepId, cb);
                                } else {
                                    this.setValidationError(null);
                                    cb();
                                }
                            }
                        }

                        return id <= this.maxStep;
                    },
                    nextStep() {
                        let cb = () => {
                            if (this.activeStepId < this.stepCount - 1) {
                                this.changeStep(this.getStepInstance(this.activeStepId), this.getNextStepInstance(this.activeStepId));

                                this.afterStepChange(this.activeStepId);
                            } else {
                                this.$emit('on-complete');
                            }
                        };

                        this.beforeStepChange(this.activeStepId, cb);
                    },
                    prevStep() {
                        let cb = () => {
                            if (this.activeStepId > 0) {
                                this.setValidationError(null);

                                this.changeStep(this.getStepInstance(this.activeStepId), this.getPrevStepInstance(this.activeStepId));
                            }
                        };

                        if (this.validateOnBack) {
                            this.beforeStepChange(this.activeStepId, cb);
                        } else {
                            cb();
                        }
                    },
                    focusnextStep() {
                        var tabIndex = CheckoutHelpers.getFocusedStepIndex(this.steps);

                        if (tabIndex !== -1 && tabIndex < this.steps.length - 1) {
                            var tabToFocus = this.getNextStepInstance(tabIndex);

                            if (tabToFocus.checked) {
                                CheckoutHelpers.findElementAndFocus(tabToFocus.elementId);
                            }
                        }
                    },
                    focusprevStep() {
                        var tabIndex = CheckoutHelpers.getFocusedStepIndex(this.steps);

                        if (tabIndex !== -1 && tabIndex > 0) {
                            var toFocusId = this.getPrevStepInstance(tabIndex).elementId;
                            CheckoutHelpers.findElementAndFocus(toFocusId);
                        }
                    },
                    setLoading(value) {
                        this.loading = value;
                        this.$emit('on-loading', value);
                    },
                    setValidationError(error) {
                        this.getStepInstance(this.activeStepId).validationError = error;
                        this.$emit('on-error', error);
                    },
                    validateBeforeChange(promiseFn, callback) {
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
                    executeBeforeChange(validationResult, callback) {
                        this.$emit('on-validate', validationResult, this.activeStepId);

                        if (validationResult) {
                            callback();
                        } else {
                            this.getStepInstance(this.activeStepId).validationError = 'error';
                        }
                    },
                    beforeStepChange(id, callback) {
                        if (this.loading) {
                            return;
                        }

                        let oldStep = this.getStepInstance(id);

                        if (oldStep && oldStep.beforeChange !== undefined) {
                            let stepChangeRes = oldStep.beforeChange();
                            this.validateBeforeChange(stepChangeRes, callback);
                        } else {
                            callback();
                        }
                    },
                    beforeStepEnter(id) {
                        if (this.loading) {
                            return;
                        }

                        let newStep = this.getStepInstance(id);

                        if (newStep && newStep.beforeEnter !== undefined) {
                            newStep.beforeEnter();
                        }
                    },
                    afterStepChange(id) {
                        if (this.loading) {
                            return;
                        }

                        let newStep = this.getStepInstance(id);

                        if (newStep && newStep.afterChange !== undefined) {
                            newStep.afterChange();
                        }
                    },
                    changeStep(oldStep, newStep) {
                        var emitChangeEvent = arguments.length > 2 && arguments[2] !== undefined ? arguments[2] : true;
                        let newId = newStep && newStep.id;
                        let oldId = oldStep && oldStep.id;

                        if (oldStep) {
                            oldStep.active = false;
                        }

                        if (newStep) {
                            newStep.active = true;
                        }

                        if (emitChangeEvent && this.activeStepId !== newId) {
                            this.emitStepChange(oldId, newId);
                        }

                        this.activeStepId = newId;
                        this.activateStepAndCheckStep(this.activeStepId);
                        return true;
                    },
                    scrollToStep(stepIndex) {
                        let elementId = this.getStepInstance(stepIndex).elementId;
                        setTimeout(function () {
                            $('html, body').animate({
                                scrollTop: $('#' + elementId).offset().top
                            }, 500);
                        }, 500);
                    },
                    deactivateSteps() {
                        this.steps.forEach(function (step) {
                            step.active = false;
                        });
                    },
                    activateStep(id) {
                        this.deactivateSteps();
                        let step = this.getStepInstance(id);

                        if (step) {
                            step.active = true;
                            step.checked = true;
                        }
                    },
                    activateStepAndCheckStep(id) {
                        this.activateStep(id);

                        if (id > this.maxStep) {
                            this.maxStep = id;
                        }

                        this.activeStepId = id;
                    },
                    initializeSteps() {
                        if (this.steps.length > 0 && this.startIndex === 0) {
                            this.activateStep(this.activeStepId);
                        }

                        if (this.startIndex < this.steps.length) {
                            this.activateStepAndCheckStep(this.startIndex);
                        } else {
                            window.console.warn(`Prop startIndex set to ${this.startIndex} is greater than the number of steps - ${this.steps.length}. Make sure that the starting index is less than the number of tabs registered`);
                        }
                    },
                    findNotFilledStepId() {
                        let step = this.steps.find((step, index ) => !step.fulfilled || index === this.steps.length - 1);
                        return step && step.id;
                    }
                },
                mounted() {
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
