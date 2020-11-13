///<reference path='../../../../Typings/tsd.d.ts' />
///<reference path='../../../../Typings/vue/index.d.ts' />
///<reference path='../../../Composer.SingleCheckout/VueComponents/CheckoutHelpers.ts' />

module Orckestra.Composer {
    'use strict';

    export class StepperVueComponent {
        static  componentMame: string = 'stepper';
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

                    stepsClasses: {
                        type: [String, Array],
                        default: ''
                    },
                    stepSize: {
                        type: String,
                        default: 'col-sm-10 col-lg-7'
                    },
                    /***
                     *
                     * Index of the initial tab to display
                     */
                    startIndex: {
                        type: Number,
                        default: 0,
                        validator(value) {
                            return value >= 0;
                        }
                    },
                    showPreviousSteps: {
                        type: Boolean,
                        default: false,
                    }
                },
                provide() {
                    return {
                        addStep: this.addStep,
                        removeStep: this.removeStep,
                        resetNextSteps: this.resetNextSteps,
                        navigateToStep: this.navigateToStep
                    };
                },
                data() {
                    return {
                        activeStepIndex: 0,
                        currentPercentage: 0,
                        maxStep: 0,
                        loading: false,
                        steps: [],
                        completed: false
                    };
                },
                computed: {
                    slotProps() {
                        return {
                            activeStepIndex: this.activeStepIndex,
                            isLastStep: this.isLastStep,
                        };
                    },
                    stepCount() {
                        return this.steps.length;
                    },
                    isLastStep() {
                        return this.activeStepIndex === this.stepCount - 1;
                    },
                    displayPrevButton() {
                        return this.activeStepIndex !== 0;
                    },
                    stepPercentage() {
                        return 1 / (this.stepCount * 2) * 100;
                    },
                    activeStep() {
                        return this.steps.length > this.activeStepIndex && this.steps[this.activeStepIndex];
                    }
                },
                methods: {
                    emitStepChange(prevIndex, nextIndex) {
                        this.$emit('on-change', prevIndex, nextIndex);
                        this.$emit('update:startIndex', nextIndex);
                    },
                    addStep(item) {
                        let index = this.$slots.default.filter((d) => item.$vnode.tag === d.tag).indexOf(item.$vnode);
                        this.steps.splice(index, 0, item); // if a step is added before the current one, go to it

                        if (index < this.activeStepIndex + 1) {
                            this.maxStep = index;
                            this.changeStep(this.activeStepIndex + 1, index);
                        }

                        item.index = this.steps.indexOf(item);
                        item.stepId = 'step' + index;
                        this.maxStep = this.steps.length - 1; //TODO: fix it
                    },
                    removeStep(item) {
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
                                this.emitStepChange(this.activeStepIndex + 1, this.activeStepIndex);
                            }

                            tabs.splice(index, 1);
                        }
                    },
                    reset() {
                        this.completed = false
                        this.maxStep = 0;
                        this.steps.forEach(function (tab) {
                            tab.checked = false;
                        });
                        this.navigateToStep(0);
                    },
                    activateAll() {
                        this.maxStep = this.steps.length - 1;
                        this.steps.forEach(function (tab) {
                            tab.checked = true;
                        });
                    },
                    navigateToStep(index) {
                        var validate = index > this.activeStepIndex;

                        let cb = () => {
                            if (validate && index - this.activeStepIndex > 1) {
                                // validate all steps recursively until destination index
                                this.changeStep(this.activeStepIndex, this.activeStepIndex + 1);
                                this.beforeStepChange(this.activeStepIndex, cb);
                            } else {
                                if (index < this.stepCount ) {
                                    this.beforeStepEnter(index);
                                    this.changeStep(this.activeStepIndex, index);
                                    this.scrollToStep(index);
                                    this.afterStepChange(this.activeStepIndex);
                                } else {
                                    this.completed = true;
                                    this.$emit('on-complete');
                                }
                            }
                        };
                        if (validate) {
                            this.beforeStepChange(this.activeStepIndex, cb);
                        } else {
                            // when trying leave already saved step(edit mode) when edit it we need to validate it
                            let step = this.steps[this.activeStepIndex + 1];

                            if (step && step.fulfilled) {
                                this.beforeStepChange(this.activeStepIndex, cb);
                            } else {
                                this.setValidationError(null);
                                cb();
                                this.resetNextSteps(index);
                                this.completed = false;
                            }
                        }

                        return index <= this.maxStep;
                    },
                    resetNextSteps(index) {
                        for(let i = index + 1; i < this.steps.length; i++) {
                            let step = this.steps[i];
                            if(!step || !step.resetChanges || !step.fulfilled) continue;
                            step.resetChanges.call(this);
                        }
                    },
                    setLoading(value) {
                        this.loading = value;
                        this.$emit('on-loading', value);
                    },
                    setValidationError(error) {
                        this.steps[this.activeStepIndex].validationError = error;
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
                        this.$emit('on-validate', validationResult, this.activeStepIndex);

                        if (validationResult) {
                            callback();
                        } else {
                            this.steps[this.activeStepIndex].validationError = 'error';
                        }
                    },
                    beforeStepChange(index, callback) {
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
                    beforeStepEnter(index) {
                        if (this.loading) {
                            return;
                        }

                        let newStep = this.steps[index];

                        if (newStep && newStep.beforeEnter !== undefined) {
                            newStep.beforeEnter();
                        }
                    },
                    afterStepChange(index) {
                        if (this.loading) {
                            return;
                        }

                        let newStep = this.steps[index];

                        if (newStep && newStep.afterChange !== undefined) {
                            newStep.afterChange();
                        }
                    },
                    changeStep(oldIndex, newIndex) {
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
                            this.emitStepChange(oldIndex, newIndex);
                        }

                        this.activeStepIndex = newIndex;
                        this.activateStepAndCheckStep(this.activeStepIndex);
                        return true;
                    },
                    scrollToStep(stepIndex) {
                        let stepId = this.steps[stepIndex].stepId;
                        setTimeout(function () {
                            $('html, body').animate({
                                scrollTop: ($('#' + stepId).offset().top - 150)
                            }, 500);
                        }, 500);
                    },
                    deactivateSteps() {
                        this.steps.forEach((step) => step.active = false);
                    },
                    activateStep(index) {
                        this.deactivateSteps();
                        var step = this.steps[index];

                        if (step) {
                            step.active = true;
                            step.checked = true;
                        }
                    },
                    activateStepAndCheckStep(index) {
                        this.activateStep(index);

                        if (index > this.maxStep) {
                            this.maxStep = index;
                        }

                        this.activeStepIndex = index;
                    },
                    initializeSteps() {
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
                mounted() {
                    this.initializeSteps();
                },
                template: `
                <div class="bs-stepper linear row justify-content-center" >
                  <div class="bs-stepper-header col-12 sps sps--abv sticky-top" role="tablist">
                    <template  v-for="(step, index) in steps">
                        <div class="bs-stepper-tab" 
                             :class="[{active: step.active, passed: step.slotProps.passed}]">
                          <button :disabled="!step.slotProps.passed" 
                                  type="button" 
                                  class="step-trigger p-0" 
                                  @click="navigateToStep(index)">
                            <div class="bs-stepper-circle"><span class="fa fa-2x " :class="{'fa-check': step.slotProps.passed}"></span></div>
                            <p class="d-none d-sm-block m-2 bs-stepper-title">{{step.title}}</p>
                            <p class="d-block d-sm-none m-2 bs-stepper-mobiletitle">{{step.mobileTitle}}</p>
                          </button>
                        </div>
                    </template>
                  </div>
                  <div class="col-12" :class="[stepSize]">
                    <slot v-bind="slotProps">
                    </slot>
                  </div>
                </div> 
                `
            };
        }
    }
}
