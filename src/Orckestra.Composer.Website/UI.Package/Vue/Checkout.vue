<template>
  <div :id="id ? id : ''" class="single-page-checkout" :class="[stepSize, {vertical: isVertical}]" >
      <div class="wizard-tab-content">
        <slot v-bind="slotProps">
        </slot>
      </div>
  </div>
</template>
<script>


module.exports = {
    name: 'single-page-checkout',
    components: {},
    props: {
      id: {
        type: String,
        default: 'fw_' + new Date().valueOf()
      },
      title: {
        type: String,
        default: 'Awesome Wizard'
      },
      subtitle: {
        type: String,
        default: 'Split a complicated flow in multiple steps'
      },
      nextButtonText: {
        type: String,
        default: 'Next'
      },
      backButtonText: {
        type: String,
        default: 'Back'
      },
      finishButtonText: {
        type: String,
        default: 'Finish'
      },
      hideButtons: {
        type: Boolean,
        default: false
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

      /**
       * Name of the transition when transition between steps
       * */
      transition: {
        type: String,
        default: ''
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
          return item.$vnode.tag == d.tag;
        }).indexOf(item.$vnode);
        this.steps.splice(index, 0, item); // if a step is added before the current one, go to it

        if (index < this.activeStepIndex + 1) {
          this.maxStep = index;
          this.changeStep(this.activeStepIndex + 1, index);
        }

        item.index = this.steps.indexOf(item);
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
        var _this = this;

        var validate = index > this.activeStepIndex;

        if (index <= this.maxStep) {
          var cb = function cb() {
             _this.beforeStepEnter(index);
             _this.changeStep(_this.activeStepIndex, index);
          };

           this.beforeStepChange(this.activeStepIndex, cb);
        }

        return index <= this.maxStep;
      },
      nextStep: function nextStep() {
        var _this2 = this;

        var cb = function cb() {
          if (_this2.activeStepIndex < _this2.stepCount - 1) {
            _this2.changeStep(_this2.activeStepIndex, _this2.activeStepIndex + 1);

            _this2.afterStepChange(_this2.activeStepIndex);
          } else {
            _this2.$emit('on-complete');
          }
        };

        this.beforeStepChange(this.activeStepIndex, cb);
      },
      prevStep: function prevStep() {
        var _this3 = this;

        var cb = function cb() {
          if (_this3.activeStepIndex > 0) {
            _this3.setValidationError(null);

            _this3.changeStep(_this3.activeStepIndex, _this3.activeStepIndex - 1);
          }
        };

        if (this.validateOnBack) {
          this.beforeStepChange(this.activeStepIndex, cb);
        } else {
          cb();
        }
      },
      focusnextStep: function focusnextStep() {
        var tabIndex = getFocusedStepIndex(this.steps);

        if (tabIndex !== -1 && tabIndex < this.steps.length - 1) {
          var tabToFocus = this.steps[tabIndex + 1];

          if (tabToFocus.checked) {
            findElementAndFocus(tabToFocus.stepId);
          }
        }
      },
      focusprevStep: function focusprevStep() {
        var tabIndex = getFocusedStepIndex(this.steps);

        if (tabIndex !== -1 && tabIndex > 0) {
          var toFocusId = this.steps[tabIndex - 1].stepId;
          findElementAndFocus(toFocusId);
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
        var _this4 = this;

        this.setValidationError(null); // we have a promise

        if (this.isPromise(promiseFn)) {
          this.setLoading(true);
          promiseFn.then(function (res) {
            _this4.setLoading(false);

            var validationResult = res === true;

            _this4.executeBeforeChange(validationResult, callback);
          }).catch(function (error) {
            _this4.setLoading(false);

            _this4.setValidationError(error);
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
          window.console.warn("Prop startIndex set to ".concat(this.startIndex, " is greater than the number of steps - ").concat(this.steps.length, ". Make sure that the starting index is less than the number of tabs registered"));
        }
      },
      isPromise: function isPromise(func) {
        return func.then && typeof func.then === 'function';
      }
    },
    mounted: function mounted() {
      this.initializeSteps();
    },
    watch: {
      '$route.path': function $routePath(newRoute) {
        this.checkRouteChange(newRoute);
      }
    }
};
</script>