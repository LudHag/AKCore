<template>
    <div class="videos-app">
        <div class="videos-container"
             v-bind:class="{ showleft: showLeft, showright: showRight }"
             ref="container">
            <div class="videos" v-bind:style="{ transform: 'translateX(-' + offset + 'px)' }">
                <div class="video" v-on:click="onVideoClick(video)" v-for="video in videos" v-bind:key="video.title">
                    <img :src="'https://img.youtube.com/vi/' + video.link +'/0.jpg'" />
                    <span class="title" v-html="video.title"></span>
                    <span class="play-icon glyphicon glyphicon-play"></span>
                </div>
            </div>
        </div>
        <a href="#" v-if="showLeft" v-on:click.prevent="leftClick" class="arrow-container-left">
            <span class="glyphicon glyphicon-menu-left"></span>
        </a>
        <a href="#" v-if="showRight" v-on:click.prevent="rightClick" class="arrow-container-right">
            <span class="glyphicon glyphicon-menu-right"></span>
        </a>
        <div v-if="showVideo" class="video-mask" v-on:click="closeVideo">
            <div class="youtube-container">
                <iframe id="videoplayer"
                        frameborder="0"
                        allowfullscreen="1"
                        allow="autoplay; encrypted-media"
                        width="640" height="360"
                        :src="videoSrc"></iframe>
            </div>
        </div>
    </div>
</template>
<script>

    export default {
        props: ['videos'],
        data: function () {
            return {
                offset: 0,
                visibleWidth: 0,
                totalWidth: 0,
                showVideo: false,
                videoSrc: ""
            }
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
            leftClick: function() {
                if (this.offset - this.visibleWidth > 0) {
                    this.offset -= this.visibleWidth;
                } else {
                    this.offset = 0;
                }
            },
            onVideoClick: function(video) {
                this.videoSrc = "https://www.youtube.com/embed/" + video.link + "?autoplay=1";
                this.showVideo = true;
            },
            closeVideo: function() {
                this.showVideo = false;
            }
        },
        computed: {
            showRight: function() {
                return this.offset < this.totalWidth - this.visibleWidth;
            },
            showLeft: function() {
                return this.offset > 0;
            }
        },
        mounted: function () {
            const self = this;
            const container = this.$refs.container;
            this.visibleWidth = container.offsetWidth;
            this.totalWidth = container.scrollWidth;
            this.$nextTick(function () {
                window.addEventListener("resize",
                    function () {
                        self.visibleWidth = container.offsetWidth;
                        self.totalWidth = container.scrollWidth;
                    });
            });
        }
    }
</script>
<style lang="scss">

</style>
