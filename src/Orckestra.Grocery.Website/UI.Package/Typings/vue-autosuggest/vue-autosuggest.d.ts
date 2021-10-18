import {VueConstructor} from "../vue";

export interface VueAutosuggest {
  new (): any
  VueAutosuggest: VueConstructor;
}

export const Autosuggest: VueAutosuggest;
