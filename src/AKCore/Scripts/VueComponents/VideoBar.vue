<template>
  <div class="videos-app">
    <h2 v-if="title">{{ title }}</h2>
    <div ref="container" class="videos-container">
      <div class="videos">
        <a
          v-for="video in videos"
          :key="video.title"
          class="video"
          :href="'https://www.youtube.com/watch?v=' + video.link"
          target="_blank"
        >
          <img :src="'https://img.youtube.com/vi/' + video.link + '/0.jpg'" />
          <span v-html="video.title" class="title"></span>
        </a>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { Video } from "./models";

defineProps<{
  title?: string;
  videos: Array<Video>;
}>();
</script>
<style lang="scss" scoped>
.videos-app {
  position: relative;
  h2 {
    margin-bottom: 20px;
    margin-top: 20px;
  }

  .videos {
    display: flex;
    gap: 20px;
    flex-wrap: wrap;
    .video {
      display: inline-block;
      width: 170px;
      text-align: center;
      cursor: pointer;
      position: relative;

      img {
        width: 100%;
        height: auto;
      }
    }
  }

  .title {
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
    display: block;
  }
}
</style>
