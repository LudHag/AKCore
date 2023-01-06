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
<style lang="scss" scoped>
@import "bootstrap-sass/assets/stylesheets/bootstrap/_variables.scss";
@import "../../Styles/variables.scss";
.videos-app {
  position: relative;

  .videos-container {
    overflow: hidden;
  }

  .videos {
    width: 3000px;
    width: max-content;
    transition: transform ease-in-out 0.4s;

    .video {
      display: inline-block;
      width: 170px;
      margin-right: 10px;
      margin-bottom: 20px;
      text-align: center;
      cursor: pointer;
      position: relative;

      .play-icon {
        position: absolute;
        left: 50%;
        top: 30%;
        transform: translateX(-50%);
        font-size: 40px;
        display: none;
      }

      &:hover .play-icon {
        display: block;
      }

      img {
        width: 100%;
        height: auto;
      }
    }
  }

  .arrow-container-right,
  .arrow-container-left {
    position: absolute;
    color: $akred;
    background-color: #000;
    top: 0;
    width: 40px;
    height: 150px;

    .glyphicon {
      font-size: 114px;
      transform: scaleX(0.4);
      margin-left: -38px;
    }
  }

  .arrow-container-right {
    right: 0;
  }

  .arrow-container-left {
    left: 0;
  }

  .title {
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
    display: block;
  }

  .video-mask {
    position: fixed;
    z-index: 3000;
    top: 0;
    right: 0;
    bottom: 0;
    left: 0;
    display: flex;
    justify-content: center;
    align-items: center;

    &:after {
      background: black;
      content: "";
      position: absolute;
      width: 100%;
      height: 100%;
      top: 0;
      left: 0;
      opacity: 0.5;
      z-index: 10;
    }
    .youtube-container {
      z-index: 5000;
    }
  }
}

@media screen and (max-width: $screen-xs-max) {
  .videos-app {
    .videos {
      .video {
        width: 77px;

        .play-icon {
          font-size: 25px;
          top: 15%;
        }
      }
    }

    .title {
      font-size: 12px;
    }

    .arrow-container-right,
    .arrow-container-left {
      .glyphicon {
        font-size: 78px;
        transform: scaleX(0.4);
        margin-left: -15px;
      }
    }
  }
}
</style>
