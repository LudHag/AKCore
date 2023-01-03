import { createApp } from "vue";
import VideoBar from "./VueComponents/VideoBar.vue";
import MembersList from "./VueComponents/MembersList.vue";
import ProfileApp from "./VueComponents/ProfileApp.vue";
import UpcomingApp from "./VueComponents/Upcoming/UpcomingApp.vue";
import MediaApp from "./VueComponents/Media/MediaApp.vue";
import MusicApp from "./VueComponents/MusicPlayer/MusicApp.vue";
import MailBoxApp from "./VueComponents/MailBox/MailBoxApp.vue";
import LoginApp from "./VueComponents/Login/LoginApp.vue";
import { Member } from "./VueComponents/models";

declare var videos: Record<
  number,
  Array<{
    link: string;
    title: string;
  }>
>;

$(".videos-app").each(function () {
  const widgetId = $(this).data("id");
  createApp(VideoBar, { videos: videos[widgetId] }).mount(
    `#videos-app-${widgetId}`
  );
});

declare var memberList: Record<string, Array<Member>>;
declare var instruments: string[];

if ($("#search-widget").length > 0) {
  createApp(MembersList, {
    members: memberList,
    instruments: instruments,
  }).mount("#search-widget");
}

declare var eventId: number;
if ($("#upcoming-app").length > 0) {
  createApp(UpcomingApp, {
    "event-id": eventId,
  }).mount("#upcoming-app");
}

if ($("#profile-app").length > 0) {
  createApp(ProfileApp).mount("#profile-app");
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
