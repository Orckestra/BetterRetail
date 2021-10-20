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
                    categorypage: {
                        type: Boolean,
                        required: false
                    }
                },

                computed: {
                    currentNode() {
                        return this.node ?  this.node: this.parentnode;
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
                        return ChildNodes.findIndex((n:any) => n.IsSelected) > MaxCollapsedCount;
                    }
                },
                methods: {

                },
                mounted() {

                },
                template: `
                 
                <div class="form-check mb-1"
                    :data-facetfieldname="node?.FieldName"
                    :data-facettype="node?.FacetType">
                    <label v-if="node" class="form-check-label" v-bind:class="{'font-weight-bold': node.IsSelected}">
                        <input v-if="categorypage"
                            class="form-check-input"
                            type="radio"
                            :value="node.Value"
                            :checked="node.IsSelected"
                            :data-selected="node.IsSelected"
                            :data-type="node.FacetType"
                            :data-parentcategoryurl="parentnode?.CategoryUrl"
                            :data-categoryurl="node.CategoryUrl"
                            v-on:click="(e) => nodeсlicked(e, node.IsSelected)" />

                        <input v-else-if="node.FacetType == 'SingleSelect'"
                            class="form-check-input"
                            type="radio"
                            :name="node.FieldName"
                            :value="node.Value"
                            :checked="node.IsSelected"
                            :data-selected="node.IsSelected"
                            :data-type="node.FacetType"
                            v-on:click="(e) => nodeсlicked(e, node.IsSelected)" />

                        <input v-else-if="node.FacetType == 'MultiSelect'"
                            class="form-check-input"
                            type="checkbox"
                            :name="node.FieldName + '[]'"
                            :value="node.Value"
                            :data-selected="node.IsSelected"
                            :checked="node.IsSelected"
                            :data-type="node.FacetType"
                            v-on:click="(e) => nodeсlicked(e, node.IsSelected)" />

                        {{node.Title}} <span>({{node.Quantity}})</span>

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
                        :categorypage="categorypage"   />              
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
                        :categorypage="categorypage"                  
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
