<template>
  <div class="album-display" v-if="album">
    <img class="album-display-image" :src="album.image" />
    <div class="tracks" v-if="showTracks">
      <PlayListItem
        v-for="track in tracks"
        :key="track.id"
        :track="track"
        :no-add="true"
        :small="true"
        @select="$emit('add-track', track)"
      ></PlayListItem>
    </div>
  </div>
</template>
<script setup lang="ts">
import { computed } from "vue";
import PlayListItem from "./PlayListItem.vue";
import { Album } from "./models";
import { nameCompare } from "../../utils/functions";

const { album } = defineProps<{ album: Album; showTracks: boolean }>();

const tracks = computed(() => {
  const trackKeys = Object.keys(album.tracks);
  return trackKeys
    .map((key: string) => album.tracks[parseInt(key)])
    .sort(nameCompare);
});
</script>
<style lang="scss" scoped>
.album-display {
  flex-grow: 1;
  text-align: center;
  max-width: 290px;
  margin: auto;
  position: relative;
}

.play-all {
  display: block;
  margin-top: 10px;
  font-size: 16px;
  font-weight: 800;
}

.album-display-image {
  height: 220px;
  max-width: 220px;
}

.tracks {
  position: absolute;
  top: 0;
  left: 0;
  background-color: rgba(0, 0, 0, 0.7);
  height: 100%;
  width: 100%;
  overflow-y: auto;
  overflow-x: hidden;
}

.album-display {
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
    border-radius: 10px;
  }

  ::-webkit-scrollbar-thumb {
    border-radius: 10px;
    -webkit-box-shadow: inset 0 0 6px #a5a2a0;
  }
}
</style>
