﻿<template>
  <div class="player-module">
    <Player
      :playing="playing"
      :track-playing="trackPlaying"
      :reset="reset"
      :replay="replay"
      @playpause="$emit('playpause')"
      @replay="replay = !replay"
      @next="next"
    ></Player>
    <div class="playlist">
      <PlayListItem
        v-for="track in tracks"
        :key="track.key"
        :track="track"
        :active="trackPlaying && track.key === trackPlaying.key"
        :remove="playList.length > 0"
        @select="selectTrack"
        @remove="$emit('remove', track)"
        @add="$emit('add-track', track)"
      ></PlayListItem>
    </div>
  </div>
</template>
<script setup lang="ts">
import { nextTick, ref, computed, toRefs, watch } from "vue";
import { nameCompare } from "../../utils/functions";
import { Album, Track } from "./models";
import Player from "./Player.vue";
import PlayListItem from "./PlayListItem.vue";

const emit = defineEmits<{
  (e: "playpause"): void;
  (e: "stop"): void;
  (e: "remove", track: Track): void;
  (e: "remove-before", index: number): void;
  (e: "add-track", track: Track): void;
}>();

const props = defineProps<{
  album: Album;
  playList: Track[];
  playing: boolean;
}>();

const { album, playList, playing } = toRefs(props);

const trackPlaying = ref<Track | undefined>();
const reset = ref(false);
const replay = ref(false);

const tracks = computed(() => {
  if (playList.value.length > 0) {
    return playList.value;
  }
  return [...album.value.tracks]
    .map((track) => ({ ...track, key: track.id }))
    .sort(nameCompare);
});

watch(
  () => playList.value,
  (val) => {
    if (val.length > 0 && !trackPlaying.value) {
      trackPlaying.value = val[0];
      nextTick(() => emit("playpause"));
    }
  },
);

const next = () => {
  const currentIndex = tracks.value.findIndex(
    (track) => track.key === trackPlaying?.value?.key,
  );
  if (replay.value) {
    return nextIfReplay(currentIndex);
  }

  let dontRemove = false;
  if (currentIndex === -1 || currentIndex + 1 >= tracks.value.length) {
    if (playList.value.length > 0 && trackPlaying.value !== playList.value[0]) {
      trackPlaying.value = tracks.value[0];
      dontRemove = true;
    } else {
      emit("playpause");
      trackPlaying.value = undefined;
    }
  } else {
    trackPlaying.value = tracks.value[currentIndex + 1];
  }
  if (playList.value.length > 0 && !dontRemove) {
    emit("remove", playList.value[0]);
  }
};

const nextIfReplay = (currentIndex: number) => {
  if (playList.value.length === 0) {
    trackPlaying.value = tracks.value[currentIndex];
    emit("stop");
    reset.value = true;
    nextTick(() => (reset.value = false));
    nextTick(() => emit("playpause"));
  } else {
    if (currentIndex + 1 >= tracks.value.length) {
      trackPlaying.value = tracks.value[0];
    } else {
      trackPlaying.value = tracks.value[currentIndex + 1];
    }
  }
};

const selectTrack = (track: Track) => {
  if (playing.value && trackPlaying.value === track) {
    reset.value = true;
    nextTick(() => (reset.value = false));
  } else {
    trackPlaying.value = track;
    if (!playing.value) {
      reset.value = true;
      nextTick(() => (reset.value = false));
      nextTick(() => emit("playpause"));
    }
    if (playList.value.length > 0) {
      const index = playList.value.indexOf(track);
      if (!replay.value) {
        emit("remove-before", index);
      }
    }
  }
};
</script>

<style lang="scss" scoped>
@import "bootstrap-sass/assets/stylesheets/bootstrap/_variables.scss";
@import "../../../Styles/variables.scss";
.player-module {
  flex-grow: 1;
  padding-left: 30px;
}
.playlist {
  height: 210px;
  overflow: auto;
}
.player-module {
  ::-webkit-scrollbar {
    width: 12px;
  }

  ::-webkit-scrollbar-track {
    -webkit-box-shadow: inset 0 0 6px rgba(0, 0, 0, 0.3);
    border-radius: 10px;
  }

  ::-webkit-scrollbar-thumb {
    border-radius: 10px;
    -webkit-box-shadow: inset 0 0 6px #a5a2a0;
  }

  ::-moz-scrollbar {
    width: 12px;
  }

  ::-moz-scrollbar-track {
    -webkit-box-shadow: inset 0 0 6px rgba(0, 0, 0, 0.3);
    box-shadow: inset 0 0 6px rgba(0, 0, 0, 0.3);
    border-radius: 10px;
  }

  ::-webkit-scrollbar-thumb {
    border-radius: 10px;
    -webkit-box-shadow: inset 0 0 6px #a5a2a0;
  }
}

@media screen and (max-width: $screen-xs-max) {
  .player-module {
    padding-top: 30px;
    padding-left: 0;
  }
}
</style>
