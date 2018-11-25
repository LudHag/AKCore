<template>
    <div id="album-edit-app">
        <album-edit-item v-for="album in albums" :album="album" :key="album.id">
        </album-edit-item>
    </div>
</template>
<script>
import ApiService from "../../services/apiservice";
import AlbumEditItem from "./AlbumEditItem";

export default {
  components: {
    AlbumEditItem
  },
  data() {
    return {
      albums: null
    };
  },
  created() {
    ApiService.get("/AlbumEdit/AlbumData", null, res => {
      this.albums = res;
    });
  }
};
</script>
<style lang="scss">

</style>
