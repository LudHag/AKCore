<template>
    <div id="album-edit-app">
        <div class="album-upload">
            <div class="row">
                <div class="col-md-12">
                    <button type="button" id="add-album" class="btn btn-primary" @click.prevent="openCreate">
                        Skapa album
                    </button>
                </div>
            </div>
             <div class="alert alert-danger" style="display: none;">
            </div>
             <div class="alert alert-success" style="display: none;">
            </div>
            <div class="row edit-form" id="add-album-container" v-if="createOpened">
                <div class="col-md-6">
                    <form action="/AlbumEdit/AddAlbum" method="post" @submit.prevent="createAlbum">
                        <div class="form-group">
                            <label for="name">Albumnamn: </label>
                            <input ref="addalbuminput" type="text" class="form-control" id="name" name="name" required placeholder="Albumnamn">
                        </div>
                        <button type="submit" class="btn btn-default">Skapa</button>
                    </form>
                </div>
            </div>
        </div>
        <album-edit-item v-for="album in albums" 
            :album="album" 
            :key="album.id" 
            @name="changeName" 
            @delete="deleteAlbum"
            @image="pickImage">
        </album-edit-item>
        <image-picker-modal :show-modal="showImagePicker" @close="closeImagePicker" @image="imageSelected"></image-picker-modal>
    </div>
</template>
<script>
import ApiService from "../../services/apiservice";
import AlbumEditItem from "./AlbumEditItem";
import ImagePickerModal from "../ImagePickerModal";

export default {
  components: {
    AlbumEditItem,
    ImagePickerModal
  },
  data() {
    return {
      albums: null,
      createOpened: false,
      showImagePicker: false,
      selectedAlbum: -1
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
      ApiService.postByUrl(
        "/AlbumEdit/DeleteAlbum/" + id,
        error,
        null,
        () => {
          this.albums = this.albums.filter(album => {
            return album.id !== id;
          });
        }
      );
    },
    openCreate() {
      this.createOpened = !this.createOpened;
      if (this.createOpened) {
        this.$nextTick(() => this.$refs.addalbuminput.focus());
      }
    },
    createAlbum() {
      const error = $(".alert-danger");
      const success = $(".alert-success");
      const form = $(event.target);
      ApiService.defaultFormSend(form, error, success, () => {
        this.loadAlbumData();
        this.createOpened = false;
      });
    },
    loadAlbumData() {
      ApiService.get("/AlbumEdit/AlbumData", null, res => {
        this.albums = res;
      });
    },
    pickImage(id) {
      this.showImagePicker = true;
      this.selectedAlbum = id;
    },
    closeImagePicker() {
      this.showImagePicker = false;
      this.selectedAlbum = -1;
    },
    imageSelected(image) {
      const error = $(".alert-danger");
      ApiService.postByObject("/AlbumEdit/UpdateImage", 
        {id: this.selectedAlbum, src: '/media/' + image.name}, 
        error,
        null, () => {
          this.loadAlbumData();
        }
      );
      this.closeImagePicker();
    }
  },
  created() {
    this.loadAlbumData();
  }
};
</script>
<style lang="scss" scoped>
.edit-form {
  padding-top: 0;
}
</style>
