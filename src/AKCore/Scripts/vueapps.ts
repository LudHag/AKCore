import { createApp } from "vue";
import VideoBar from "./VueComponents/VideoBar.vue";
import MembersList from "./VueComponents/MembersList.vue";
import ProfileApp from "./VueComponents/Profile/ProfileApp.vue";
import UpcomingApp from "./VueComponents/Upcoming/UpcomingApp.vue";
import MediaApp from "./VueComponents/Media/MediaApp.vue";
import MusicApp from "./VueComponents/MusicPlayer/MusicApp.vue";
import MailBoxApp from "./VueComponents/MailBox/MailBoxApp.vue";
import LoginApp from "./VueComponents/Login/LoginApp.vue";
import { Member, Video } from "./VueComponents/models";
import NotificationApp from "./VueComponents/Notifications/NotificationApp.vue";
import { getCookie } from "./general";
import CountDownApp from "./VueComponents/Countdown/CountDownApp.vue";
import VideosSearch from "./VueComponents/VideosSearch.vue";

const videosHeaderApp = document.getElementById("videos-header-search-app");
if (videosHeaderApp) {
  createApp(VideosSearch).mount(videosHeaderApp);
}

declare const videos: Record<number, { title?: string; videos: Array<Video> }>;

const videoApps = Array.from(
  document.getElementsByClassName(
    "videos-app",
  ) as HTMLCollectionOf<HTMLElement>,
);

videoApps.forEach((app) => {
  const widgetId = parseInt(app.dataset.id as string);
  const videoData = videos[widgetId];

  createApp(VideoBar, {
    ...videoData,
  }).mount(`#videos-app-${widgetId}`);
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
declare const selectedDate: string;
if (document.getElementById("countdown")) {
  createApp(CountDownApp, {
    selectedDate: selectedDate,
  }).mount("#countdown");
}

if (document.getElementById("loginapp")) {
  createApp(LoginApp).mount("#loginapp");
}

const notificationElement = document.getElementById("notificationapp");
if (notificationElement && window.innerWidth < 760) {
  createApp(NotificationApp, {
    recruitsInfoDisabled: getCookie("recruitsPopup") === "hide",
    mailboxInfoDisabled: getCookie("mailboxPopup") === "hide",
    recruits: Number(notificationElement.dataset.recruits),
    mails: Number(notificationElement.dataset.mails),
  }).mount("#notificationapp");
}
