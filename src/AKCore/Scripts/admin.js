import Vue from "vue";
import "./Admin/users.js";
import "./Admin/pageedit.js";
import PageEdit from "./VueComponents/PageEdit/PageEdit";

if ($("#pageedit-app").length > 0) {
  new Vue({
    render: h => h(PageEdit)
  }).$mount("#pageedit-app");
}
