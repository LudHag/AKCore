import { createApp } from "vue";
import AdminEventApp from "./VueComponents/AdminEvent/AdminEventApp.vue";
import AlbumEditApp from "./VueComponents/AlbumEdit/AlbumEditApp.vue";
import MenuEditApp from "./VueComponents/MenuEdit/MenuEditApp.vue";
import RecruitsApp from "./VueComponents/Recruits/RecruitsApp.vue";
import UsersApp from "./VueComponents/Users/UsersApp.vue";
import MediaApp from "./VueComponents/Media/MediaApp.vue";
import PageEdit from "./VueComponents/PageEdit/PageEdit.vue";
import PagesApp from "./VueComponents/Pages/PagesApp.vue";

if (document.getElementById("pageedit-app")) {
  createApp(PageEdit).mount("#pageedit-app");
}

if (document.getElementById("media-app")) {
  createApp(MediaApp).mount("#media-app");
}

if (document.getElementById("admin-event-app")) {
  createApp(AdminEventApp).mount("#admin-event-app");
}

if (document.getElementById("album-edit-app")) {
  createApp(AlbumEditApp).mount("#album-edit-app");
}

if (document.getElementById("media-app")) {
  createApp(MediaApp).mount("#media-app");
}

if (document.getElementById("menu-edit-app")) {
  createApp(MenuEditApp).mount("#menu-edit-app");
}

if (document.getElementById("recruits-app")) {
  createApp(RecruitsApp).mount("#recruits-app");
}

if (document.getElementById("user-app")) {
  createApp(UsersApp).mount("#user-app");
}

if (document.getElementById("pages-app")) {
  createApp(PagesApp).mount("#pages-app");
}
