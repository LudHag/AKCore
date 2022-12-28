import { createApp } from "vue";
import VideoBar from "./VueComponents/VideoBar.vue";
import MembersList from "./VueComponents/MembersList.vue";
// import ProfileApp from "./VueComponents/ProfileApp.vue";
import UpcomingApp from "./VueComponents/Upcoming/UpcomingApp.vue";
// import AdminEventApp from "./VueComponents/AdminEvent/AdminEventApp.vue";
import AlbumEditApp from "./VueComponents/AlbumEdit/AlbumEditApp.vue";
import MenuEditApp from "./VueComponents/MenuEdit/MenuEditApp.vue";
import MediaApp from "./VueComponents/Media/MediaApp.vue";
import MusicApp from "./VueComponents/MusicPlayer/MusicApp.vue";
import MailBoxApp from "./VueComponents/MailBox/MailBoxApp.vue";
import LoginApp from "./VueComponents/Login/LoginApp.vue";

$(".videos-app").each(function () {
  const widgetId = $(this).data("id");
  createApp(VideoBar, { videos: videos[widgetId] }).mount(
    `#videos-app-${widgetId}`
  );
});

if ($("#search-widget").length > 0) {
  createApp(MembersList, {
    members: memberList,
    instruments: instruments,
  }).mount("#search-widget");
}
if ($("#upcoming-app").length > 0) {
  createApp(UpcomingApp, {
    "event-id": eventId,
  }).mount("#upcoming-app");
}

// if ($("#profile-app").length > 0) {
//   createApp(ProfileApp).mount("#profile-app");
// }

// if ($("#admin-event-app").length > 0) {
//   createApp(AdminEventApp).mount("#admin-event-app");
// }

if ($("#album-edit-app").length > 0) {
  createApp(AlbumEditApp).mount("#album-edit-app");
}

if ($("#menu-edit-app").length > 0) {
  createApp(MenuEditApp).mount("#menu-edit-app");
}

if ($("#media-app").length > 0) {
  createApp(MediaApp).mount("#media-app");
}

if ($("#music-app").length > 0) {
  createApp(MusicApp).mount("#music-app");
}

if ($("#mailbox").length > 0) {
  createApp(MailBoxApp).mount("#mailbox");
}

if ($("#loginapp").length > 0) {
  createApp(LoginApp).mount("#loginapp");
}
