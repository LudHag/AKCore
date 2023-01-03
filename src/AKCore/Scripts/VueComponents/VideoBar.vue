<template>
  <div class="videos-app">
    <div
      class="videos-container"
      v-bind:class="{ showleft: showLeft, showright: showRight }"
      ref="container"
    >
      <div
        class="videos"
        v-bind:style="{ transform: 'translateX(-' + offset + 'px)' }"
      >
        <div
          class="video"
          v-on:click="onVideoClick(video)"
          v-for="video in videos"
          v-bind:key="video.title"
        >
          <img :src="'https://img.youtube.com/vi/' + video.link + '/0.jpg'" />
          <span class="title" v-html="video.title"></span>
          <span class="play-icon glyphicon glyphicon-play"></span>
        </div>
      </div>
    </div>
    <a
      href="#"
      v-if="showLeft"
      v-on:click.prevent="leftClick"
      class="arrow-container-left"
    >
      <span class="glyphicon glyphicon-menu-left"></span>
    </a>
    <a
      href="#"
      v-if="showRight"
      v-on:click.prevent="rightClick"
      class="arrow-container-right"
    >
      <span class="glyphicon glyphicon-menu-right"></span>
    </a>
    <div v-if="showVideo" class="video-mask" v-on:click="closeVideo">
      <div class="youtube-container">
        <iframe
          id="videoplayer"
          frameborder="0"
          allow="autoplay; encrypted-media; fullscreen"
          width="640"
          height="360"
          :src="videoSrc"
        ></iframe>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref, computed, onMounted } from "vue";
import { Video } from "./models";

const { videos } = defineProps<{
  videos: Array<Video>;
}>();

const offset = ref(0);
const visibleWidth = ref(0);
const totalWidth = ref(0);
const showVideo = ref(false);
const videoSrc = ref("");
const container = ref<HTMLElement>();

const rightClick = () => {
  if (offset.value + visibleWidth.value < totalWidth.value) {
    if (offset.value + visibleWidth.value * 2 > totalWidth.value) {
      offset.value = totalWidth.value - visibleWidth.value;
    } else {
      offset.value += visibleWidth.value;
    }
  }
};

const leftClick = () => {
  if (offset.value - visibleWidth.value > 0) {
    offset.value -= visibleWidth.value;
  } else {
    offset.value = 0;
  }
};

const onVideoClick = (video: Video) => {
  videoSrc.value =
    "https://www.youtube.com/embed/" + video.link + "?autoplay=1";
  showVideo.value = true;
};

const closeVideo = () => {
  showVideo.value = false;
};

const showRight = computed(() => {
  return offset.value < totalWidth.value - visibleWidth.value;
});

const showLeft = computed(() => {
  return offset.value > 0;
});

onMounted(() => {
  const containerValue = container.value;
  if (!containerValue) return;
  visibleWidth.value = containerValue.offsetWidth;
  totalWidth.value = containerValue.scrollWidth;
  window.addEventListener("resize", () => {
    visibleWidth.value = containerValue.offsetWidth;
    totalWidth.value = containerValue.scrollWidth;
  });
});
</script>
<style lang="scss"></style>
