///<reference path='../../../../Typings/tsd.d.ts' />
///<reference path='../../../../Typings/vue/index.d.ts' />

module Orckestra.Composer {
    'use strict';

    export class FacetTreeVueComponent {
        static componentName: string = 'facets-tree';

        static initialize() {
            Vue.component(this.componentName, this.getComponent());
        }

        static getComponent() {
            return {
                name: FacetTreeVueComponent.componentName,
                components: {},
                props: {
                    nodeсlicked: {
                        type: Function,
                        required: false,
                    },
                    loading: {
                        type: Boolean,
                        required: false
                    },
                    node: {
                        type: Object,
                        required: false
                    },
                    parentnode: {
                        type: Object,
                        required: false
                    },
                    showmoretext: {
                        type: String,
                        required: false
                    },
                    showlesstext: {
                        type: String,
                        required: false
                    },
                    categoryid: {
                        type: String,
                        required: false
                    }
                },

                computed: {
                    currentNode() {
                        return this.node ? this.node: this.parentnode;
                    },
                    hasChildren() {
                        const { ChildNodes } = this.currentNode;
                        return ChildNodes && ChildNodes.length > 0
                    },
                    visibleNodes() {
                        const { ChildNodes, MaxCollapsedCount } = this.currentNode;
                        return this.isSelectedInColapsed ? ChildNodes: ChildNodes.slice(0, MaxCollapsedCount);
                    },
                    collapsedNodes() {
                        const { ChildNodes, MaxCollapsedCount, MaxExpandedCount } = this.currentNode;
                        return this.isSelectedInColapsed ? [] : ChildNodes.slice(MaxCollapsedCount, MaxExpandedCount);
                    },
                    isSelectedInColapsed() {
                        const { ChildNodes, MaxCollapsedCount } = this.currentNode;
                        return ChildNodes.map(e => e.IsSelected).lastIndexOf(true) >= MaxCollapsedCount;
                    }
                },
                methods: {
                    isHighlighted(facet) {
                        return facet.IsSelected && (!facet.ChildNodes || facet.ChildNodes.every(child => !child.IsSelected));
                    }
                },
                mounted() {

                },
                template: `
                 
                <div class="mb-1"
                    :class="{'form-check': !!node }"
                    :data-facetfieldname="node?.FieldName"
                    :data-facettype="node?.FacetType">
                    <label v-if="node" 
                        class="m-0" :class="{'selected': node.IsSelected, 'highlighted': isHighlighted(node)}">
                        <input v-if="node.FacetType == 'MultiSelect'"
                            type="checkbox"
                            :disabled="!node.IsRemovable"
                            class="form-check-input"
                            :name="node.FieldName + '[]'"
                            :value="node.Value"
                            :data-type="node.FacetType"
                            :data-facetfieldname="node.FieldName"
                            :data-facetvalue="node.Value"
                            :data-categoryid="node.CategoryId"
                            :checked="node.IsSelected"
                            :data-selected="node.IsSelected"
                            v-on:click="(event) => nodeсlicked(event, node.IsSelected)" 
                        />
                        <input v-else-if="node.FacetType == 'SingleSelect'"
                            type="checkbox"
                            :disabled="!node.IsRemovable"
                            class="form-check-input"
                            :name="node.FieldName"
                            :data-facetfieldname="node.FieldName"
                            :data-facetvalue="node.Value"
                            :value="node.Value"
                            :data-type="node.FacetType"
                            :data-categoryid="node.CategoryId"
                            :checked="node.IsSelected"
                            :data-selected="node.IsSelected"
                            v-on:click="(event) => nodeсlicked(event, node.IsSelected)" 
                        />
                        {{node.Title}} ({{node.Quantity}})
                    </label>

                <div v-if="hasChildren">
                  <facets-tree
                     v-for="childNode in visibleNodes"
                        :key="childNode.CategoryId"
                        :node="childNode"
                        :parentnode="currentNode"
                        :nodeсlicked="nodeсlicked"
                        :showmoretext="showmoretext"
                        :showlesstext="showlesstext"
                        :categoryid="categoryid"   />              
                  <div v-if="collapsedNodes.length" >
                  <div class="collapse" :id="'onDemandFacets-' + currentNode.FieldName">
                    <facets-tree
                        v-for="childNode in collapsedNodes"
                        :key="childNode.CategoryId"
                        :node="childNode"
                        :parentnode="currentNode"
                        :nodeсlicked="nodeсlicked"
                        :showmoretext="showmoretext"
                        :showlesstext="showlesstext"
                        :categoryid="categoryid"                  
                    />
                    </div>
                    <a class="btn  btn-link  collapsed  font-weight-bold" 
                        data-toggle="collapse" 
                        :data-target="'#onDemandFacets-' + currentNode.FieldName">
                        <span class="more">{{showmoretext}} <i class="fa fa-angle-down" /></span>
                        <span class="less">{{showlesstext}} <i class="fa fa-angle-up" /></span>
                    </a>
                  </div>
                </div>
              </div>`
            };
        }
    }
}
