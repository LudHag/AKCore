import { createApp } from "vue";
import VideoBar from "./VueComponents/VideoBar.vue";
import MembersList from "./VueComponents/MembersList.vue";
import ProfileApp from "./VueComponents/ProfileApp.vue";
import UpcomingApp from "./VueComponents/Upcoming/UpcomingApp.vue";
import MediaApp from "./VueComponents/Media/MediaApp.vue";
import MusicApp from "./VueComponents/MusicPlayer/MusicApp.vue";
import MailBoxApp from "./VueComponents/MailBox/MailBoxApp.vue";
import LoginApp from "./VueComponents/Login/LoginApp.vue";
import { Member, Video } from "./VueComponents/models";

declare const videos: Record<number, Array<Video>>;

const videoApps = Array.from(
  document.getElementsByClassName("videos-app") as HTMLCollectionOf<HTMLElement>
);

videoApps.forEach((app) => {
  const widgetId = parseInt(app.dataset.id as string);
  createApp(VideoBar, { videos: videos[widgetId] }).mount(
    `#videos-app-${widgetId}`
  );
});

declare const memberList: Record<string, Array<Member>>;
declare const instruments: string[];

if (document.getElementById("search-widget")) {
  createApp(MembersList, {
    members: memberList,
    instruments: instruments,
  }).mount("#search-widget");
}

declare const eventId: number;
if (document.getElementById("upcoming-app")) {
  createApp(UpcomingApp, {
    "event-id": eventId,
  }).mount("#upcoming-app");
}

if (document.getElementById("profile-app")) {
  createApp(ProfileApp).mount("#profile-app");
}

if (document.getElementById("media-app")) {
  createApp(MediaApp).mount("#media-app");
}

declare const loggedIn: boolean;
if (document.getElementById("music-app")) {
  createApp(MusicApp, {
    loggedIn: loggedIn,
  }).mount("#music-app");
}

if (document.getElementById("mailbox")) {
  createApp(MailBoxApp).mount("#mailbox");
}

if (document.getElementById("loginapp")) {
  createApp(LoginApp).mount("#loginapp");
}
