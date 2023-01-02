<template>
  <div id="music-app">
    <h2 class="album-title" v-if="currentAlbum">
      {{ currentAlbum.name }}
    </h2>
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
import { Albums, Album, Track } from "./models";
import { ref, onMounted } from "vue";

const albums = ref<Albums | null>(null);
const currentAlbum = ref<Album | null>(null);
const playList = ref<Track[]>([]);
const playing = ref(false);

const setCurrentAlbum = (currentAlbumId: number) => {
  if (!albums.value || currentAlbumId < 1) {
    return null;
  }
  currentAlbum.value = albums.value[currentAlbumId];
  currentAlbum.value.tracks = [...albums.value[currentAlbumId].tracks];
};

const selectAlbum = (album: Album) => {
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

onMounted(() => {
  //@ts-ignore
  albums.value = widgetAlbums;
  if (!albums.value) {
    return;
  }
  const albumIds = Object.keys(albums.value);
  setCurrentAlbum(
    parseInt(albumIds[Math.floor(Math.random() * albumIds.length)])
  );
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

@media screen and (max-width: $screen-xs-max) {
  .player-container {
    display: block;
    margin-top: 20px;
    max-height: inherit;
    max-height: initial;
  }
}
</style>
