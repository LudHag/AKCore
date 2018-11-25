<template>
    <div id="album-edit-app">
        <div class="alert alert-danger" style="display: none;">
        </div>
        <album-edit-item v-for="album in albums" :album="album" :key="album.id" @name="changeName" @delete="deleteAlbum">
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
  methods: {
    changeName(name, id) {
      const error = $(".alert-danger");
      ApiService.postByObject(
        "/AlbumEdit/ChangeName",
        { id: id, name: name },
        error,
        null,
        () => {
          this.albums = this.albums.map(item => {
            if (item.id === id) {
              return Object.assign({}, item, { name });
            } else {
              return item;
            }
          });
        }
      );
    },
    deleteAlbum(id) {
      const error = $(".alert-danger");
      ApiService.postByUrl("/AlbumEdit/DeleteAlbum/" + id, error, null, item => {
        this.albums = this.albums.filter((album) => {
          return album.id !== id;
        });
      });
    }
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
