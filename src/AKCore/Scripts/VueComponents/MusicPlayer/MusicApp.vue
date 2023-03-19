<template>
  <div id="music-app">
    <h2 class="album-title" v-if="currentAlbum">
      {{ currentAlbum.name }}
      <a
        v-if="loggedIn"
        href="#"
        class="botlink"
        @click.prevent="showDesc"
        title="Visa AI beskrivning"
      ></a>
    </h2>
    <div v-if="showingDesc" class="aidesc">
      <p v-if="aiDesc">Albumbeskrivning genererad av ChatGPT:</p>
      <p v-if="aiDesc">{{ aiDesc }}</p>
      <spinner :size="'medium'" v-else></spinner>
    </div>
    <div class="player-container" v-if="currentAlbum">
      <AlbumDisplay
        :album="currentAlbum"
        @add-track="addTrack"
        :show-tracks="playList.length > 0"
      ></AlbumDisplay>
      <PlayList
        :play-list="playList"
        :album="currentAlbum"
        :playing="playing"
        @remove="removeTrack"
        @remove-before="removeBefore"
        @add-track="addTrack"
        @playpause="playPause"
        @stop="stop"
      ></PlayList>
    </div>
    <album-list
      v-if="albums && Object.keys(albums).length > 1"
      @add-track="addTrack"
      @select="selectAlbum"
      :albums="albums"
    ></album-list>
  </div>
</template>
<script setup lang="ts">
import AlbumList from "./AlbumList.vue";
import AlbumDisplay from "./AlbumDisplay.vue";
import PlayList from "./PlayList.vue";
import Spinner from "../Spinner.vue";
import { Album, Track } from "./models";
import { ref, onMounted } from "vue";
import { getFromApi } from "../../services/apiservice";

const albums = ref<Album[]>([]);
const currentAlbum = ref<Album | null>(null);
const playList = ref<Track[]>([]);
const playing = ref(false);
const aiDesc = ref<string | null>(null);
const showingDesc = ref(false);

const prop = defineProps<{
  loggedIn: boolean;
}>();

const setCurrentAlbum = (currentAlbumId: string) => {
  if (!albums.value || parseInt(currentAlbumId) < 1) {
    return null;
  }
  currentAlbum.value =
    albums.value.find((x) => x.id === currentAlbumId) ?? null;
};

const selectAlbum = (album: Album) => {
  aiDesc.value = null;
  showingDesc.value = false;
  setCurrentAlbum(album.id);
};

const playPause = () => {
  playing.value = !playing.value;
};

const stop = () => {
  playing.value = false;
};

const addTrack = (track: Track) => {
  const newTrack = { ...track, key: track.id + playList.value.length };
  playList.value.push(newTrack);
};

const removeTrack = (track: Track) => {
  const index = playList.value.indexOf(track);
  if (index > -1) {
    playList.value.splice(index, 1);
  }
};

const removeBefore = (index: number) => {
  if (playList.value.length > index) {
    playList.value.splice(0, index);
  }
};

const showDesc = async () => {
  if (!currentAlbum.value) {
    return;
  }
  showingDesc.value = !showingDesc.value;
  if (showingDesc.value && !aiDesc.value) {
    try {
      aiDesc.value = (
        await getFromApi<{ albumInfo: string }>(
          `/ExtraInfo/GetAlbumInfo?id=${currentAlbum.value.id}`
        )
      ).albumInfo;
    } catch (error) {
      aiDesc.value = null;
      showingDesc.value = false;
      throw error;
    }
  }
};

onMounted(() => {
  //@ts-ignore
  // eslint-disable-next-line no-undef
  albums.value = widgetAlbums;
  if (!albums.value) {
    return;
  }
  const album = albums.value[Math.floor(Math.random() * albums.value.length)];
  setCurrentAlbum(album.id);
});
</script>
<style lang="scss" scoped>
@import "bootstrap-sass/assets/stylesheets/bootstrap/_variables.scss";
@import "../../../Styles/variables.scss";
.album-title {
  background-color: #111111;
  border-radius: 5px;
  padding: 10px 20px;
  margin-bottom: 20px;
  text-transform: uppercase;
}

.player-container {
  display: flex;
  background-color: #111111;
  border-radius: 5px;
  padding: 20px;
  max-height: 320px;
}

.botlink {
  background: url("/images/aibot.svg") no-repeat;
  color: #fff;
  font-size: 12px;
  position: absolute;
  right: 30px;
  height: 30px;
  width: 30px;
  top: 13px;
}
.aidesc {
  margin-bottom: 20px;
}

@media screen and (max-width: $screen-xs-max) {
  .player-container {
    display: block;
    margin-top: 20px;
    max-height: inherit;
    max-height: initial;
  }
}
</style>
