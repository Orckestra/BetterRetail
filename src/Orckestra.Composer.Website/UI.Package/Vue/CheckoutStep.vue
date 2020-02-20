<template>
  <div
    class="checkout-step-container"
    v-bind:class="{'text-success': active}"
    role="tabpanel"
    :id="stepId"
  >
    <slot v-bind="slotProps"></slot>
  </div>
</template>
<script>
module.exports = {
  name: "step-content",
  props: {
    /***
     * Function to execute before tab switch. Return value must be boolean
     * If the return result is false, tab switch is restricted
     */
    beforeChange: {
      type: Function
    },
    /***
     * Function to execute after tab switch. Return void for now.
     * Safe to assume necessary validation has already occured
     */
    afterChange: {
      type: Function
    }
  },
  inject: ["addStep", "removeStep", "nextStep"],
  data() {
    return {
      active: false,
      index: null,
      validationError: null,
      checked: false,
      stepId: ""
    };
  },
  computed: {
    slotProps() {
      var self = this;
      return {
        nextStep: this.$parent.nextStep,
        prevStep: this.$parent.prevStep,
        navigateToStep: this.$parent.navigateToStep,
        activeStepIndex: this.$parent.activeStepIndex,
        isLastStep: this.$parent.isLastStep,
        index: this.index,
        active: this.active,
        displayContinueButton: (!this.active && ((this.$parent.activeStepIndex + 1) === this.index)),
        selectStep: () => {
          this.$parent.navigateToStep(this.index);
        },
        preview: this.index < this.$parent.activeStepIndex
      };
    },

  },
  methods: {
  },
  mounted() {
    this.addStep(this);
  },
  destroyed() {
    if (this.$el && this.$el.parentNode) {
      this.$el.parentNode.removeChild(this.$el);
    }
    this.removeStep(this);
  }
};
</script>