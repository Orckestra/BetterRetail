<template>
  <div
    class="checkout-step-container"
    v-bind:class="{'text-success': active}"
    role="tabpanel"
    :id="tabId"
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
  inject: ["addTab", "removeTab", "nextTab"],
  data() {
    return {
      active: false,
      index: null,
      validationError: null,
      checked: false,
      tabId: ""
    };
  },
  computed: {
    slotProps() {
      var self = this;
      return {
        nextTab: this.$parent.nextTab,
        prevTab: this.$parent.prevTab,
        navigateToTab: this.$parent.navigateToTab,
        activeTabIndex: this.$parent.activeTabIndex,
        isLastStep: this.$parent.isLastStep,
        index: this.index,
        active: this.active,
        selectTab: () => {
          this.$parent.navigateToTab(this.index);
        },
        preview: this.index < this.$parent.activeTabIndex
      };
    },
  },
  methods: {
  },
  mounted() {
    this.addTab(this);
  },
  destroyed() {
    if (this.$el && this.$el.parentNode) {
      this.$el.parentNode.removeChild(this.$el);
    }
    this.removeTab(this);
  }
};
</script>