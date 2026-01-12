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
            >
              Skapa album
            </button>
            <div class="form-group search-input">
              <input
                type="text"
                class="form-control"
                placeholder="Sök här"
                v-model="search"
              />
            </div>
            <div class="form-group">
              <select class="form-control" v-model="albumCategory">
                <option value>Filtrera på kategori</option>
                <option v-for="cat in ALBUMCATEGORIES" :key="cat">
                  {{ cat }}
                </option>
              </select>
            </div>
          </div>
        </div>
      </div>
      <div class="alert alert-danger" ref="error" style="display: none"></div>
      <div
        class="alert alert-success"
        ref="success"
        style="display: none"
      ></div>
      <div class="row edit-form" id="add-album-container" v-if="createOpened">
        <div class="col-md-6">
          <form
            action="/AlbumEdit/AddAlbum"
            method="post"
            @submit.prevent="createAlbum"
          >
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
              />
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
    <album-upload-modal
      :album="tracksAlbum"
      @close="closeUploadModal"
      @update="loadAlbumData"
    ></album-upload-modal>
  </div>
</template>
<script setup lang="ts">
import { postToApi, defaultFormSend, getFromApi } from "@services/apiservice";
import AlbumEditItem from "./AlbumEditItem.vue";
import ImagePickerModal from "../ImagePickerModal.vue";
import AlbumUploadModal from "./AlbumUploadModal.vue";
import { ALBUMCATEGORIES } from "../../constants";
import { computed, onMounted, ref } from "vue";
import { AlbumEditModel } from "./models";

const albums = ref<AlbumEditModel[]>([]);
const createOpened = ref(false);
const showImagePicker = ref(false);
const selectedAlbum = ref(-1);
const tracksAlbumId = ref(-1);
const search = ref("");
const albumCategory = ref("");
const error = ref<HTMLElement | null>(null);
const success = ref<HTMLElement | null>(null);

const tracksAlbum = computed(() => {
  if (!albums.value || tracksAlbumId.value == -1) {
    return null;
  }
  return (
    albums.value.find((album) => {
      return album.id === tracksAlbumId.value;
    }) ?? null
  );
});

const filteredAlbums = computed(() => {
  if (!albums.value) {
    return albums.value;
  }
  return albums.value.filter((album: AlbumEditModel) => {
    const currentCategory = album.category ? album.category : "Övrigt";
    return (
      (!search.value ||
        album.name.toLowerCase().indexOf(search.value.toLowerCase()) > -1) &&
      (!albumCategory.value || currentCategory === albumCategory.value)
    );
  });
});

const changeName = (name: string, id: number) => {
  postToApi(
    "/AlbumEdit/ChangeName",
    { id: id, name: name },
    error.value,
    null,
    () => {
      albums.value = albums.value.map((item) => {
        if (item.id === id) {
          return Object.assign({}, item, { name });
        } else {
          return item;
        }
      });
    },
  );
};

const changeCategory = (category: string, id: number) => {
  postToApi(
    "/AlbumEdit/ChangeCategory",
    { id, category },
    error.value,
    null,
    () => {
      albums.value = albums.value.map((item) => {
        if (item.id === id) {
          return Object.assign({}, item, { category });
        } else {
          return item;
        }
      });
    },
  );
};

const deleteAlbum = (id: number) => {
  postToApi("/AlbumEdit/DeleteAlbum/" + id, null, error.value, null, () => {
    albums.value = albums.value.filter((album) => {
      return album.id !== id;
    });
  });
};

const openCreate = () => {
  createOpened.value = !createOpened.value;
  if (createOpened.value) {
    setTimeout(() => {
      const input = document.getElementById("name") as HTMLInputElement;
      input.focus();
    }, 0);
  }
};

const createAlbum = (event: Event) => {
  defaultFormSend(
    event.target as HTMLFormElement,
    error.value,
    success.value,
    () => {
      loadAlbumData();
      createOpened.value = false;
    },
  );
};

const loadAlbumData = async () => {
  albums.value = await getFromApi<AlbumEditModel[]>("/AlbumEdit/AlbumData");
};

const pickImage = (id: number) => {
  showImagePicker.value = true;
  selectedAlbum.value = id;
};

const closeImagePicker = () => {
  showImagePicker.value = false;
  selectedAlbum.value = -1;
};

const imageSelected = (image: { name: string }) => {
  postToApi(
    "/AlbumEdit/UpdateImage",
    { id: selectedAlbum.value, src: "/media/" + image.name },
    error.value,
    null,
    () => {
      loadAlbumData();
    },
  );
  closeImagePicker();
};

const uploadTracks = (album: AlbumEditModel) => {
  tracksAlbumId.value = album.id;
};

const closeUploadModal = () => {
  tracksAlbumId.value = -1;
};

onMounted(() => {
  loadAlbumData();
});
</script>
<style lang="scss" scoped>
.edit-form {
  padding-top: 0;
}
.search-input {
  margin-left: 30px;
}
</style>
