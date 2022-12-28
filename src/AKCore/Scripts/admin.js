import { createApp } from "vue";
import AdminEventApp from "./VueComponents/AdminEvent/AdminEventApp.vue";
import AlbumEditApp from "./VueComponents/AlbumEdit/AlbumEditApp.vue";
import MenuEditApp from "./VueComponents/MenuEdit/MenuEditApp.vue";
import RecruitsApp from "./VueComponents/Recruits/RecruitsApp.vue";
import UsersApp from "./VueComponents/Users/UsersApp.vue";
import PageEdit from "./VueComponents/PageEdit/PageEdit.vue";
import "./Admin/pageedit.js";

if ($("#pageedit-app").length > 0) {
  createApp(PageEdit).mount("#pageedit-app");
}

if ($("#admin-event-app").length > 0) {
  createApp(AdminEventApp).mount("#admin-event-app");
}

if ($("#album-edit-app").length > 0) {
  createApp(AlbumEditApp).mount("#album-edit-app");
}

if ($("#menu-edit-app").length > 0) {
  createApp(MenuEditApp).mount("#menu-edit-app");
}

if ($("#recruits-app").length > 0) {
  createApp(RecruitsApp).mount("#recruits-app");
}

if ($("#user-app").length > 0) {
  createApp(UsersApp).mount("#user-app");
}
