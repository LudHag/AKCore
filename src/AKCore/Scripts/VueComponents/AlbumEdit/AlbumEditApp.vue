<template>
  <div id="album-edit-app">
    <div class="album-upload">
      <div class="row">
        <div class="col-md-12">
          <div class="form-inline">
            <button
              type="button"
              id="add-album"
              class="btn btn-primary"
              @click.prevent="openCreate"
            >Skapa album</button>
            <div class="form-group search-input">
              <input type="text" class="form-control" placeholder="Sök här" v-model="search">
            </div>
            <div class="form-group">
              <select class="form-control" v-model="albumCategory">
                <option value>Filtrera på kategori</option>
                <option v-for="cat in albumCategories" :key="cat">{{cat}}</option>
              </select>
            </div>
          </div>
        </div>
      </div>
      <div class="alert alert-danger" style="display: none;"></div>
      <div class="alert alert-success" style="display: none;"></div>
      <div class="row edit-form" id="add-album-container" v-if="createOpened">
        <div class="col-md-6">
          <form action="/AlbumEdit/AddAlbum" method="post" @submit.prevent="createAlbum">
            <div class="form-group">
              <label for="name">Albumnamn:</label>
              <input
                ref="addalbuminput"
                type="text"
                class="form-control"
                id="name"
                name="name"
                required
                placeholder="Albumnamn"
              >
            </div>
            <button type="submit" class="btn btn-default">Skapa</button>
          </form>
        </div>
      </div>
    </div>
    <album-edit-item
      v-for="album in filteredAlbums"
      :album="album"
      :key="album.id"
      @name="changeName"
      @category="changeCategory"
      @delete="deleteAlbum"
      @image="pickImage"
      @tracks="uploadTracks"
    ></album-edit-item>
    <image-picker-modal
      :show-modal="showImagePicker"
      @close="closeImagePicker"
      @image="imageSelected"
    ></image-picker-modal>
    <album-upload-modal :album="tracksAlbum" @close="closeUploadModal" @update="loadAlbumData"></album-upload-modal>
  </div>
</template>
<script>
import ApiService from "../../services/apiservice";
import AlbumEditItem from "./AlbumEditItem";
import ImagePickerModal from "../ImagePickerModal";
import AlbumUploadModal from "./AlbumUploadModal";
import Constants from "../../constants";

export default {
  components: {
    AlbumEditItem,
    ImagePickerModal,
    AlbumUploadModal
  },
  data() {
    return {
      albums: null,
      createOpened: false,
      showImagePicker: false,
      selectedAlbum: -1,
      tracksAlbumId: -1,
      search: "",
      albumCategory: ""
    };
  },
  computed: {
    tracksAlbum() {
      if (!this.albums || this.tracksAlbumId == -1) {
        return null;
      }
      return this.albums.find(album => {
        return album.id === this.tracksAlbumId;
      });
    },
    filteredAlbums() {
      if (!this.albums) {
        return this.albums;
      }
      return this.albums.filter(album => {
        const albumCategory = album.category ? album.category : "Övrigt";
        return (
          (!this.search ||
            album.name.toLowerCase().indexOf(this.search.toLowerCase()) > -1) &&
          (!this.albumCategory || albumCategory === this.albumCategory)
        );
      });
    },
    albumCategories() {
      return Constants.ALBUMCATEGORIES;
    }
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
    changeCategory(category, id) {
      const error = $(".alert-danger");
      ApiService.postByObject(
        "/AlbumEdit/ChangeCategory",
        { id, category },
        error,
        null,
        () => {
          this.albums = this.albums.map(item => {
            if (item.id === id) {
              return Object.assign({}, item, { category });
            } else {
              return item;
            }
          });
        }
      );
    },
    deleteAlbum(id) {
      const error = $(".alert-danger");
      ApiService.postByUrl("/AlbumEdit/DeleteAlbum/" + id, error, null, () => {
        this.albums = this.albums.filter(album => {
          return album.id !== id;
        });
      });
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
      ApiService.postByObject(
        "/AlbumEdit/UpdateImage",
        { id: this.selectedAlbum, src: "/media/" + image.name },
        error,
        null,
        () => {
          this.loadAlbumData();
        }
      );
      this.closeImagePicker();
    },
    uploadTracks(album) {
      this.tracksAlbumId = album.id;
    },
    closeUploadModal() {
      this.tracksAlbumId = -1;
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
.search-input {
  margin-left: 30px;
}
</style>
