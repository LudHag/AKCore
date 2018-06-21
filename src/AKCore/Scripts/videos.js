
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
                        if (this.offset + this.visibleWidth * 2 > this.totalWidth) {
                            this.offset = this.totalWidth - this.visibleWidth;
                        } else {
                            this.offset += this.visibleWidth;
                        }
                    }
                },
                leftClick: function () {
                    if (this.offset - this.visibleWidth > 0) {
                        this.offset -= this.visibleWidth;
                    } else {
                        this.offset = 0;
                    }
                }
            },
            computed: {
                showRight: function () {
                    return this.offset < this.totalWidth - this.visibleWidth;
                },
                showLeft: function () {
                    return this.offset > 0;
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