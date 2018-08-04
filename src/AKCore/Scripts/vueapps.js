import Vue from "vue";
import VideoBar from "./VueComponents/VideoBar";
import MembersList from "./VueComponents/MembersList";
import UpcomingApp from "./VueComponents/Upcoming/UpcomingApp";

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
