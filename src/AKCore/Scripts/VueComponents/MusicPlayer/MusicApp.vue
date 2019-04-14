<template>
  <div id="music-app">
    <h1>woop woop</h1>
    <album-list v-if="albums" :albums="albums"></album-list>
  </div>
</template>
<script>
import AlbumList from "./AlbumList";

export default {
  data() {
    return {
      albums: null
    };
  },
  components: {
    AlbumList
  },
  created() {
    this.albums = widgetAlbums;
  }
};
</script>
<style lang="scss" scoped>
</style>
