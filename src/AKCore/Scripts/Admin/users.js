import Vue from "vue/dist/vue.esm.js";
import UsersApp from "../VueComponents/Users/UsersApp.vue";

const usercontainer = $("#user-container");
if (usercontainer.length > 0) {
  let usersApp = new Vue({
    el: `#user-app`,
    template: "<users-app />",
    components: { UsersApp },
  });
}
