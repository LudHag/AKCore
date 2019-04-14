<template>
  <div id="music-app">
    <h2 class="album-title" v-if="currentAlbum">{{this.currentAlbum.name}}</h2>
    <div class="player-container" v-if="currentAlbum">
      <div class="album-display">
        <img class="album-display-image" :src="this.currentAlbum.image">
        <a href="#" class="play-all" @click.prevent="playAll">
          Spela alla spår
          <span class="glyphicon glyphicon-play"></span>
        </a>
      </div>
    </div>
    <album-list v-if="albums" @select="selectAlbum" :albums="albums"></album-list>
  </div>
</template>
<script>
import AlbumList from "./AlbumList";

export default {
  data() {
    return {
      albums: null,
      currentAlbumId: -1
    };
  },
  components: {
    AlbumList
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
    }
  },
  created() {
    this.albums = widgetAlbums;
    this.currentAlbumId = Object.keys(widgetAlbums)[0];
  }
};
</script>
<style lang="scss" scoped>
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

.album-display {
  flex-grow: 1;
  text-align: center;
  max-width: 290px;
  margin: auto;
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
</style>
