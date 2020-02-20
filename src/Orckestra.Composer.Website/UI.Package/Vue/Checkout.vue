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
    components: {
    },
    props: {
      id: {
        type: String,
        default: 'fw_' + (new Date()).valueOf()
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
        validator: (value) => {
          let acceptedValues = ['xs', 'sm', 'md', 'lg']
          return acceptedValues.indexOf(value) !== -1
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
        validator: (value) => {
          return value >= 0
        }
      }
    },
    provide () {
      return {
        addStep: this.addStep,
        removeStep: this.removeStep,
        nextStep: this.nextStep
      }
    },
    data () {
      return {
        activeStepIndex: 0,
        currentPercentage: 0,
        maxStep: 0,
        loading: false,
        steps: []
      }
    },
    computed: {
      slotProps () {
        return {
          nextStep: this.nextStep,
          prevStep: this.prevStep,
          activeStepIndex: this.activeStepIndex,
          isLastStep: this.isLastStep,
          fillButtonStyle: this.fillButtonStyle
        }
      },
      stepCount () {
        return this.steps.length
      },
      isLastStep () {
        return this.activeStepIndex === this.stepCount - 1
      },
      isVertical () {
        return this.layout === 'vertical'
      },
      displayPrevButton () {
        return this.activeStepIndex !== 0
      },
      stepPercentage () {
        return 1 / (this.stepCount * 2) * 100
      },

      fillButtonStyle () {
        return {
          backgroundColor: this.color,
          borderColor: this.color,
          color: 'white'
        }
      },
    },
    methods: {
      emitTabChange (prevIndex, nextIndex) {
        this.$emit('on-change', prevIndex, nextIndex)
        this.$emit('update:startIndex', nextIndex)
      },
      addStep (item) {
        const index = this.$slots.default.filter(d => item.$vnode.tag == d.tag).indexOf(item.$vnode);
        this.steps.splice(index, 0, item)
        // if a step is added before the current one, go to it
        if (index < this.activeStepIndex + 1) {
          this.maxStep = index
          this.changeStep(this.activeStepIndex + 1, index)
        }
        item.index = this.steps.indexOf(item);
        this.maxStep = this.steps.length - 1; //TODO: fix it
      },
      removeStep (item) {
        const tabs = this.steps
        const index = tabs.indexOf(item)
        if (index > -1) {
          // Go one step back if the current step is removed
          if (index === this.activeStepIndex) {
            this.maxStep = this.activeStepIndex - 1
            this.changeStep(this.activeStepIndex, this.activeStepIndex - 1)
          }
          if (index < this.activeStepIndex) {
            this.maxStep = this.activeStepIndex - 1
            this.activeStepIndex = this.activeStepIndex - 1
            this.emitTabChange(this.activeStepIndex + 1, this.activeStepIndex)
          }
          tabs.splice(index, 1)
        }
      },
      reset () {
        this.maxStep = 0;
        this.steps.forEach((tab) => {
          tab.checked = false
        })
        this.navigateToStep(0)
      },
      activateAll () {
        this.maxStep = this.steps.length - 1
        this.steps.forEach((tab) => {
          tab.checked = true
        })
      },
      navigateToStep (index) {
        let validate = index > this.activeStepIndex;
        if (index <= this.maxStep) {
          let cb = () => {
            if (validate && index - this.activeStepIndex > 1) {
              // validate all steps recursively until destination index
              this.changeStep(this.activeStepIndex, this.activeStepIndex + 1)
              this.beforeStepChange(this.activeStepIndex, cb)
            } else {
              this.changeStep(this.activeStepIndex, index)
              this.afterStepChange(this.activeStepIndex)
            }
          }
          if (validate) {
            this.beforeStepChange(this.activeStepIndex, cb)
          } else {
            this.setValidationError(null)
            cb()
          }
        }
        return index <= this.maxStep
      },
      nextStep () {
        let cb = () => {
          if (this.activeStepIndex < this.stepCount - 1) {
            this.changeStep(this.activeStepIndex, this.activeStepIndex + 1)
            this.afterStepChange(this.activeStepIndex)
          } else {
            this.$emit('on-complete')
          }
        }
        this.beforeStepChange(this.activeStepIndex, cb)
      },
      prevStep () {
        let cb = () => {
          if (this.activeStepIndex > 0) {
            this.setValidationError(null)
            this.changeStep(this.activeStepIndex, this.activeStepIndex - 1)
          }
        }
        if (this.validateOnBack) {
          this.beforeStepChange(this.activeStepIndex, cb)
        } else {
          cb()
        }
      },
      focusnextStep () {
        let tabIndex = getFocusedStepIndex(this.steps)
        if (tabIndex !== -1 && tabIndex < this.steps.length - 1) {
          let tabToFocus = this.steps[tabIndex + 1]
          if (tabToFocus.checked) {
            findElementAndFocus(tabToFocus.stepId)
          }
        }
      },
      focusprevStep () {
        let tabIndex = getFocusedStepIndex(this.steps)
        if (tabIndex !== -1 && tabIndex > 0) {
          let toFocusId = this.steps[tabIndex - 1].stepId
          findElementAndFocus(toFocusId)
        }
      },
      setLoading (value) {
        this.loading = value
        this.$emit('on-loading', value)
      },
      setValidationError (error) {
        this.steps[this.activeStepIndex].validationError = error
        this.$emit('on-error', error)
      },
      validateBeforeChange (promiseFn, callback) {
        this.setValidationError(null)
        // we have a promise
        if (this.isPromise(promiseFn)) {
          this.setLoading(true)
          promiseFn.then((res) => {
            this.setLoading(false)
            let validationResult = res === true
            this.executeBeforeChange(validationResult, callback)
          }).catch((error) => {
            this.setLoading(false)
            this.setValidationError(error)
          })
          // we have a simple function
        } else {
          let validationResult = promiseFn === true
          this.executeBeforeChange(validationResult, callback)
        }
      },
      executeBeforeChange (validationResult, callback) {
        this.$emit('on-validate', validationResult, this.activeStepIndex)
        if (validationResult) {
          callback()
        } else {
          this.steps[this.activeStepIndex].validationError = 'error'
        }
      },
      beforeStepChange (index, callback) {
        if (this.loading) {
          return
        }
        let oldStep = this.steps[index]
        if (oldStep && oldStep.beforeChange !== undefined) {
          let stepChangeRes = oldStep.beforeChange()
          this.validateBeforeChange(stepChangeRes, callback)
        } else {
          callback()
        }
      },
      afterStepChange (index) {
        if (this.loading) {
          return
        }
        let newStep = this.steps[index]
        if (newStep && newStep.afterChange !== undefined) {
          newStep.afterChange()
        }
      },
      changeStep (oldIndex, newIndex, emitChangeEvent = true) {
        let oldStep = this.steps[oldIndex]
        let newStep = this.steps[newIndex]
        if (oldStep) {
          oldStep.active = false
        }
        if (newStep) {
          newStep.active = true
        }
        if (emitChangeEvent && this.activeStepIndex !== newIndex) {
          this.emitTabChange(oldIndex, newIndex)
        }
        this.activeStepIndex = newIndex
        this.activateStepAndCheckStep(this.activeStepIndex)
        return true
      },
      deactivateSteps () {
        this.steps.forEach(step => {
          step.active = false
        })
      },
      activateStep (index) {
        this.deactivateSteps()
        let step = this.steps[index]
        if (step) {
          step.active = true
          step.checked = true
        }
      },
      activateStepAndCheckStep (index) {
        this.activateStep(index)
        if (index > this.maxStep) {
          this.maxStep = index
        }
        this.activeStepIndex = index
      },
      initializeSteps () {
        if (this.steps.length > 0 && this.startIndex === 0) {
          this.activateStep(this.activeStepIndex)
        }
        if (this.startIndex < this.steps.length) {
          this.activateStepAndCheckStep(this.startIndex)
        } else {
          window.console.warn(`Prop startIndex set to ${this.startIndex} is greater than the number of steps - ${this.steps.length}. Make sure that the starting index is less than the number of tabs registered`)
        }
      },

      isPromise (func) {
        return func.then && typeof func.then === 'function'
      }
    },
    mounted () {
      this.initializeSteps()
    },
    watch: {
      '$route.path' (newRoute) {
        this.checkRouteChange(newRoute)
      }
    }
  }
</script>