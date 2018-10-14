import Vue from "vue";
import VideoBar from "./VueComponents/VideoBar";
import MembersList from "./VueComponents/MembersList";
import ProfileApp from "./VueComponents/ProfileApp";
import UpcomingApp from "./VueComponents/Upcoming/UpcomingApp";
import AdminEventApp from "./VueComponents/AdminEvent/AdminEventApp";
import GhostApp from "./VueComponents/Ghost";

$(".videos-app").each(function () {
    const widgetId = $(this).data("id");
    const videoApp = new Vue({
        el: `#videos-app-${widgetId}`,
        data: {
            videos: videos[widgetId]
        },
        template: "<video-bar :videos='videos' />",
        components: { VideoBar }
    });
});

if ($("#search-widget").length > 0) {
    const searchApp = new Vue({
        el: `#search-widget`,
        data: {
            members: memberList,
            instruments
        },
        template: "<members-list :members='members' :instruments='instruments' />",
        components: { MembersList }
    });
}
if ($("#upcoming-app").length > 0) {
    const upcomingApp = new Vue({
        el: `#upcoming-app`,
        data: {
            eventId
        },
        template: "<upcoming-app :event-id='eventId' />",
        components: { UpcomingApp }
    });
}

if ($("#profile-app").length > 0) {
    const profileApp = new Vue({
        el: `#profile-app`,
        template: "<profile-app />",
        components: { ProfileApp }
    });
}

if ($("#admin-event-app").length > 0) {
    const adminEventApp = new Vue({
        el: `#admin-event-app`,
        template: "<admin-event-app />",
        components: { AdminEventApp }
    });
}

if ($("#spooky-container").length > 0) {
    const ghostApp = new Vue({
        el: `#spooky-container`,
        template: "<ghost-app />",
        components: { GhostApp }
    });
}

