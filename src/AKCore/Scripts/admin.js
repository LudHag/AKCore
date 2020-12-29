import Vue from "vue";
import "./Vendor/jquery-ui.js";
import "./Vendor/jquery.multi-select.js";
import "./Admin/pageedit.js";
import "./Admin/users.js";
import PageEdit from "./VueComponents/PageEdit/PageEdit";

if ($("#pageedit-app").length > 0) {
  new Vue({
    render: h => h(PageEdit)
  }).$mount("#pageedit-app");
}
