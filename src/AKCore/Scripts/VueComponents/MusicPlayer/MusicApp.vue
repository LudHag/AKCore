<template>
  <div id="music-app">
    <h2 class="album-title" v-if="currentAlbum">{{this.currentAlbum.name}}</h2>
    <div class="player-container" v-if="currentAlbum">
      <AlbumDisplay :album="currentAlbum" @playall="playAll"></AlbumDisplay>
      <PlayList
        :play-list="playList"
        :album="currentAlbum"
        :playing="playing"
        @playpause="playPause"
        @stop="stop"
      ></PlayList>
    </div>
    <album-list v-if="albums" @select="selectAlbum" :albums="albums"></album-list>
  </div>
</template>
<script>
import AlbumList from "./AlbumList";
import AlbumDisplay from "./AlbumDisplay";
import PlayList from "./PlayList";

export default {
  data() {
    return {
      albums: null,
      currentAlbumId: -1,
      playList: [],
      playing: false
    };
  },
  components: {
    AlbumList,
    AlbumDisplay,
    PlayList
  },
  computed: {
    currentAlbum() {
      if (!this.albums || this.currentAlbumId < 1) {
        return null;
      }
      return this.albums[this.currentAlbumId];
    }
  },
  methods: {
    selectAlbum(album) {
      this.currentAlbumId = album.id;
    },
    playAll() {
      console.log("play all");
    },
    playPause() {
      this.playing = !this.playing;
    },
    stop() {
      this.playing = false;
    }
  },
  created() {
    this.albums = widgetAlbums;
    this.currentAlbumId = Object.keys(widgetAlbums)[0];
  }
};
</script>
<style lang="scss" scoped>
@import "~bootstrap-sass/assets/stylesheets/bootstrap/_variables.scss";
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
