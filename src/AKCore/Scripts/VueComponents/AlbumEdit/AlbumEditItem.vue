<template>
  <div class="row">
    <div class="col-sm-2 image" @click="pickImage">
      <img class="album-img" :src="album.image" />
    </div>
    <div class="col-sm-4 name">
      <input
        ref="inputelement"
        v-if="editName"
        type="text"
        class="name-input"
        v-model="name"
        @keyup.enter="onInputBlur"
        @blur="onInputBlur"
      />
      <span class="album-name" v-if="!editName" @click="showInput">{{
        name
      }}</span>
    </div>
    <div class="col-sm-2 category">
      <select
        class="form-control"
        v-model="albumCategory"
        @change="onCategoryChange"
      >
        <option v-for="cat in ALBUMCATEGORIES" :key="cat">{{ cat }}</option>
      </select>
    </div>
    <div class="col-sm-1 actions">
      <a
        href="#"
        class="del-album btn glyphicon glyphicon-remove"
        @click.prevent="deleteAlbum"
      ></a>
    </div>
    <div class="col-sm-3 tracks" @click="uploadTracks">
      <span class="tracks-info">
        {{ tracks }} spår uppladdade. <br />Klicka här för att hantera.
      </span>
    </div>
  </div>
</template>
<script setup lang="ts">
import { computed, nextTick, onMounted, ref } from "vue";
import { ALBUMCATEGORIES } from "../../constants";
import { AlbumEditModel } from "./models";

const emit = defineEmits<{
  (e: "name", name: string, id: number): void;
  (e: "category", name: string, id: number): void;
  (e: "tracks", album: AlbumEditModel): void;
  (e: "delete", id: number): void;
  (e: "image", id: number): void;
}>();

const props = defineProps<{
  album: AlbumEditModel;
}>();

const editName = ref(false);
const name = ref("");
const albumCategory = ref("");
const inputelement = ref<HTMLInputElement | null>(null);

const showInput = () => {
  editName.value = true;
  nextTick(() => inputelement.value!.focus());
};

const onInputBlur = () => {
  if (!editName.value) {
    return;
  }
  editName.value = false;
  if (props.album.name !== name.value) {
    emit("name", name.value, props.album.id);
  }
};

const onCategoryChange = () => {
  emit("category", albumCategory.value, props.album.id);
};

const uploadTracks = () => {
  emit("tracks", props.album);
};

const deleteAlbum = () => {
  if (
    window.confirm(
      "är du säker på att du vill ta bort album med namn " +
        props.album.name +
        " ?",
    )
  ) {
    emit("delete", props.album.id);
  }
};

const pickImage = () => {
  emit("image", props.album.id);
};

const tracks = computed(() => props.album.tracks.length);

onMounted(() => {
  name.value = props.album.name;
  albumCategory.value = props.album.category ? props.album.category : "Övrigt";
});
</script>
<style lang="scss" scoped>
@import "../../../Styles/variables.scss";
.row:hover {
  background-color: #1a0000;
}

.image,
.name,
.actions,
.category,
.tracks {
  height: 100px;
  vertical-align: middle;
}

.actions,
.image,
.category,
.tracks {
  display: flex;
  align-items: center;
}

.tracks {
  cursor: pointer;
}

.name {
  font-size: 20px;
  display: flex;
  align-items: center;

  .name-input {
    background-color: transparent;
    border: 0;
    outline: 0;

    &:focus {
      background: $akwhite;
      color: #000;
    }
  }
}

.del-album {
  font-size: 20px;
  color: red;
}

.album-img {
  height: 100px;
  max-width: 100px;
}
</style>
