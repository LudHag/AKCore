<template>
  <div id="music-app">
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
