
$(function() {
    $(".videos-app").each(function() {
        var widgetId = $(this).data("id");
        new Vue({
            el: '#videos-app-' + widgetId,
            data: {
                videos: videos[widgetId],
                offset: 0,
                visibleWidth: 0,
                totalWidth: 0
            },
            methods: {
                rightClick: function() {
                    if (this.offset + this.visibleWidth < this.totalWidth) {
                        this.offset += this.visibleWidth;
                    }
                }
            },
            mounted: function() {
                var container = this.$refs.container;
                this.visibleWidth = container.offsetWidth;
                this.totalWidth = container.scrollWidth;
            }
        });
    });
});