import { createApp } from "vue";
import AdminEventApp from "@components/AdminEvent/AdminEventApp.vue";
import AlbumEditApp from "@components/AlbumEdit/AlbumEditApp.vue";
import MenuEditApp from "@components/MenuEdit/MenuEditApp.vue";
import RecruitsApp from "@components/Recruits/RecruitsApp.vue";
import UsersApp from "@components/Users/UsersApp.vue";
import MediaApp from "@components/Media/MediaApp.vue";
import PageEdit from "@components/PageEdit/PageEdit.vue";
import PagesApp from "@components/Pages/PagesApp.vue";
import "vue3-select-component/styles";
import "@styles/adminstyles.scss";

if (document.getElementById("pageedit-app")) {
  createApp(PageEdit).mount("#pageedit-app");
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
