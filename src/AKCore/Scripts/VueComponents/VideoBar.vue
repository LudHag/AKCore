<template>
  <div v-if="!hideComponent" class="videos-app">
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
import { sharedVideoState } from "./sharedState";

const props = defineProps<{
  title?: string;
  videos: Array<Video>;
}>();

const filteredVideos = computed(() => {
  if (
    !sharedVideoState.searchPhrase ||
    sharedVideoState.searchPhrase.trim() === ""
  ) {
    return props.videos;
  }

  const searchTerm = sharedVideoState.searchPhrase.toLowerCase().trim();
  return props.videos.filter((video) =>
    video.title.toLowerCase().includes(searchTerm),
  );
});

const hideComponent = computed(() => {
  return sharedVideoState.searchPhrase && filteredVideos.value.length === 0;
});
</script>
<style lang="scss" scoped>
@use "@styles/variables.scss";
.videos-app {
  position: relative;
  margin-bottom: 20px;
  h2 {
    margin-bottom: 20px;
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

@media screen and (max-width: variables.$screen-xs-max) {
  .videos-app .videos .video {
    width: 140px;
  }
}
</style>
