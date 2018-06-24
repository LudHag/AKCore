import Vue from "vue";
import VideoBar from "./VueComponents/VideoBar";

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
