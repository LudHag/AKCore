import Vue from "vue/dist/vue.esm.js";
import RecruitsApp from "../VueComponents/Recruits/RecruitsApp.vue";

if ($("#recruits-app").length > 0) {
  let recruitsApp = new Vue({
    el: `#recruits-app`,
    template: "<recruits-app />",
    components: { RecruitsApp },
  });
}
