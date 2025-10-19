<template>
  <div class="videos-app">
    <h2 v-if="title" v-html="title"></h2>
    <div ref="container" class="videos-container">
      <div class="videos">
        <a
          v-for="video in filteredVideos"
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
import { computed } from "vue";
import { Video } from "./models";

const props = defineProps<{
  title?: string;
  videos: Array<Video>;
  searchPhrase?: string;
}>();

const filteredVideos = computed(() => {
  if (!props.searchPhrase || props.searchPhrase.trim() === "") {
    return props.videos;
  }

  const searchTerm = props.searchPhrase.toLowerCase().trim();
  return props.videos.filter((video) =>
    video.title.toLowerCase().includes(searchTerm),
  );
});
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
