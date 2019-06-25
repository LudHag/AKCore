import Vue from "vue";
import UsersApp from "../VueComponents/Users/UsersApp";

const usercontainer = $("#user-container");
if (usercontainer.length > 0) {
    let usersApp = new Vue({
        el: `#user-app`,
        template: "<users-app />",
        components: { UsersApp }
    });
    Vue.directive('tooltip',
        function(el, binding) {
            $(el).tooltip({
                title: binding.value,
                placement: binding.arg,
                trigger: 'hover'
            });
        });
}