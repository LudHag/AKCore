<template>
  <div>
    <div class="playingnow" v-if="trackPlaying" v-html="nowPlayingText"></div>
    <div class="controls" v-if="trackPlaying">
      <a
        href="#"
        @click.prevent="$emit('playpause')"
        v-if="playing"
        class="pauseplay glyphicon glyphicon-pause"
      ></a>
      <a
        href="#"
        @click.prevent="$emit('playpause')"
        v-else
        class="pauseplay glyphicon glyphicon-play"
      ></a>
      <div
        class="progress-container"
        ref="progressref"
        @click.prevent="clickProgress"
      >
        <div class="music-progress">
          <span
            class="music-progress-bar"
            :style="{ width: progress + '%' }"
          ></span>
        </div>
      </div>
      <div class="progresstime">{{ timeDisplay }}</div>
      <a
        href="#"
        @click.prevent="$emit('replay')"
        :class="{ active: replay }"
        class="replay glyphicon glyphicon-repeat"
      ></a>
    </div>
    <audio
      class="player"
      v-if="trackPlaying"
      ref="player"
      :src="trackPlaying.filepath"
    ></audio>
  </div>
</template>
<script setup lang="ts">
import { ref, computed, watch, nextTick, toRefs } from "vue";
import { fmtMSS } from "@utils/functions";

import { Track } from "./models";

const emit = defineEmits<{
  (e: "next"): void;
  (e: "replay"): void;
  (e: "playpause"): void;
}>();

const props = defineProps<{
  playing: boolean;
  trackPlaying?: Track;
  reset: boolean;
  replay: boolean;
}>();

const { playing, trackPlaying, reset } = toRefs(props);

const trackLength = ref(0);
const timePlayed = ref(0);
const player = ref<HTMLAudioElement>();
const progressref = ref<HTMLDivElement>();

const timeDisplay = computed(() => {
  return fmtMSS(timePlayed.value) + "/" + fmtMSS(trackLength.value);
});

const progress = computed(() => {
  if (timePlayed.value === 0 || trackLength.value === 0) {
    return 0;
  }
  return (timePlayed.value / trackLength.value) * 100;
});

const nowPlayingText = computed(() => {
  return "Spelar nu: " + trackPlaying?.value?.name;
});

watch(
  () => playing.value,
  (val) => {
    const playerValue = player.value;
    if (!playerValue) {
      return;
    }
    if (val) {
      playerValue.play();
      playerValue.addEventListener("timeupdate", timeUpdate);
      playerValue.addEventListener("ended", endedListener);
    } else {
      playerValue.pause();
      playerValue.removeEventListener("timeupdate", timeUpdate);
      playerValue.removeEventListener("ended", endedListener);
    }
  },
);

watch(
  () => trackPlaying?.value,
  () => {
    const playerValue = player.value;
    if (!playerValue) {
      return;
    }
    if (playing.value) {
      nextTick(() => {
        playerValue.load();
        playerValue.play();
      });
    }
  },
);

watch(
  () => reset.value,
  (val) => {
    const playerValue = player.value;
    if (!playerValue) {
      return;
    }
    if (val) {
      trackLength.value = playerValue ? playerValue.duration : 0;
      timePlayed.value = 0;
      if (playerValue) {
        playerValue.currentTime = 0;
      }
    }
  },
);

const timeUpdate = (event: any) => {
  const playerValue = player.value;
  if (!playerValue) {
    return;
  }
  trackLength.value = playerValue.duration;
  timePlayed.value = event.target.currentTime;
};

const endedListener = () => {
  trackLength.value = 0;
  timePlayed.value = 0;
  next();
};

const next = () => {
  emit("next");
};

const clickProgress = (event: any) => {
  const playerValue = player.value;
  const progressContainer = progressref.value;
  if (!playerValue) {
    return;
  }
  const decimalProgress = event.offsetX / progressContainer!.offsetWidth;
  const time = decimalProgress * playerValue.duration;
  trackLength.value = playerValue.duration;
  timePlayed.value = time;
  playerValue.currentTime = time;
};
</script>
<style lang="scss" scoped>
@import "bootstrap-sass/assets/stylesheets/bootstrap/_variables.scss";
@import "../../../Styles/variables.scss";
.controls {
  border: 1px solid $akred;
  border-radius: 7px;
  padding: 7px;
  margin-top: 3px;
  margin-bottom: 12px;
  display: flex;
  align-items: center;
}

.pauseplay {
  margin-right: 7px;
}
.replay {
  margin-left: 7px;
  color: #6f6f6f;
  &.active {
    color: $akred;
  }
}
.progress-container {
  flex-grow: 1;
  height: 15px;
  display: flex;
  align-items: center;
  cursor: pointer;
}

.progresstime {
  min-width: 50px;
  font-size: 12px;
  text-align: right;
  margin-left: 10px;
}

.music-progress {
  height: 3px;
  background-color: #c1bfbd;
  width: 100%;

  .music-progress-bar {
    background-color: #6f6f6f;
    display: block;
    height: 100%;
    transition: width ease 0.1s;
  }
}

@media screen and (max-width: $screen-xs-max) {
  .player {
    width: 100%;
  }
}
</style>
