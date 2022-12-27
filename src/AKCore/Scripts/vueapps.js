import Vue from "vue/dist/vue.esm.js";
import VideoBar from "./VueComponents/VideoBar.vue";
import MembersList from "./VueComponents/MembersList.vue";
import ProfileApp from "./VueComponents/ProfileApp.vue";
import UpcomingApp from "./VueComponents/Upcoming/UpcomingApp.vue";
import AdminEventApp from "./VueComponents/AdminEvent/AdminEventApp.vue";
import AlbumEditApp from "./VueComponents/AlbumEdit/AlbumEditApp.vue";
import MenuEditApp from "./VueComponents/MenuEdit/MenuEditApp.vue";
import MediaApp from "./VueComponents/Media/MediaApp.vue";
import MusicApp from "./VueComponents/MusicPlayer/MusicApp.vue";
import MailBoxApp from "./VueComponents/MailBox/MailBoxApp.vue";
import LoginApp from "./VueComponents/Login/LoginApp.vue";

$(".videos-app").each(function () {
  const widgetId = $(this).data("id");
  const videoApp = new Vue({
    el: `#videos-app-${widgetId}`,
    data: {
      videos: videos[widgetId],
    },
    template: "<video-bar :videos='videos' />",
    components: { VideoBar },
  });
});

if ($("#search-widget").length > 0) {
  const searchApp = new Vue({
    el: `#search-widget`,
    data: {
      members: memberList,
      instruments,
    },
    template: "<members-list :members='members' :instruments='instruments' />",
    components: { MembersList },
  });
}
if ($("#upcoming-app").length > 0) {
  const upcomingApp = new Vue({
    el: `#upcoming-app`,
    data: {
      eventId,
    },
    template: "<upcoming-app :event-id='eventId' />",
    components: { UpcomingApp },
  });
}

if ($("#profile-app").length > 0) {
  const profileApp = new Vue({
    el: `#profile-app`,
    template: "<profile-app />",
    components: { ProfileApp },
  });
}

if ($("#admin-event-app").length > 0) {
  const adminEventApp = new Vue({
    el: `#admin-event-app`,
    template: "<admin-event-app />",
    components: { AdminEventApp },
  });
}

if ($("#album-edit-app").length > 0) {
  const albumEdit = new Vue({
    el: `#album-edit-app`,
    template: "<album-edit-app />",
    components: { AlbumEditApp },
  });
}

if ($("#menu-edit-app").length > 0) {
  const menuEdit = new Vue({
    el: `#menu-edit-app`,
    template: "<menu-edit-app />",
    components: { MenuEditApp },
  });
}

if ($("#media-app").length > 0) {
  const mediaApp = new Vue({
    el: `#media-app`,
    template: "<media-app />",
    components: { MediaApp },
  });
}

if ($("#music-app").length > 0) {
  const musicPlayer = new Vue({
    el: `#music-app`,
    template: "<music-app />",
    components: { MusicApp },
  });
}

if ($("#mailbox").length > 0) {
  const mailbox = new Vue({
    el: `#mailbox`,
    template: "<mail-box-app />",
    components: { MailBoxApp },
  });
}

if ($("#loginapp").length > 0) {
  const loginapp = new Vue({
    el: `#loginapp`,
    template: "<login-app />",
    components: { LoginApp },
  });
}
