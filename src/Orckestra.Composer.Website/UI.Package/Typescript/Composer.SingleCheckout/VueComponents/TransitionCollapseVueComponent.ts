///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../../Typings/vue/index.d.ts' />

module Orckestra.Composer {
    'use strict';

    export class TransitionCollapseVueComponent {
        static  componentMame: string = 'transition-collapse';
        static initialize() {
            Vue.component(this.componentMame, this.getComponent());
        }

        static getComponent() {
            return {
                methods: {
                    beforeEnter(element) {
                        requestAnimationFrame(() => {
                            if (!element.style.height) {
                                element.style.height = '0px';
                            }

                            element.style.display = null;
                        });
                    },
                    /**
                     * @param {HTMLElement} element
                     */
                    enter(element) {
                        requestAnimationFrame(() => {
                            requestAnimationFrame(() => {
                                element.style.height = `${element.scrollHeight}px`;
                            });
                        });
                    },
                    /**
                     * @param {HTMLElement} element
                     */
                    afterEnter(element) {
                        element.style.height = null;
                    },
                    /**
                     * @param {HTMLElement} element
                     */
                    beforeLeave(element) {
                        requestAnimationFrame(() => {
                            if (!element.style.height) {
                                element.style.height = `${element.offsetHeight}px`;
                            }
                        });
                    },
                    /**
                     * @param {HTMLElement} element
                     */
                    leave(element) {
                        requestAnimationFrame(() => {
                            requestAnimationFrame(() => {
                                element.style.height = '0px';
                            });
                        });
                    },
                    /**
                     * @param {HTMLElement} element
                     */
                    afterLeave(element) {
                        element.style.height = null;
                    },
                },
                template: `
                <transition-group name="collapse" tag="div"
                        @before-enter="beforeEnter"
                        @enter="enter"
                        @after-enter="afterEnter"
                        @before-leave="beforeLeave"
                        @leave="leave"
                        @after-leave="afterLeave"
                        class="collapse-transition">
                    <slot />
                </transition-group>`
            };
        }
    }
}
