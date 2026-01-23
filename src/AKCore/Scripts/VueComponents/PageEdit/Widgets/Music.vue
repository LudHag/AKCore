<template>
  <div>
    <div class="col-sm-6">
      <p>Valbara:</p>
      <ul class="album-list">
        <li
          v-for="album in selectableAlbums"
          :key="album.id"
          @click="addAlbum(album)"
        >
          {{ album.name }}
        </li>
      </ul>
    </div>
    <div class="col-sm-6">
      <p>Valda:</p>
      <ul class="album-list">
        <li
          v-for="album in selectedAlbums"
          :key="album.id"
          @click="removeAlbum(album)"
        >
          {{ album.name }}
        </li>
      </ul>
    </div>
  </div>
</template>
<script setup lang="ts">
import { computed } from "vue";
import { AlbumEditModel } from "../../AlbumEdit/models";
import { WidgetEditModel } from "../models";

const props = defineProps<{
  modelValue: WidgetEditModel;
  albums: AlbumEditModel[];
}>();

const selectableAlbums = computed(() => {
  if (!props.modelValue.albums) {
    return props.albums;
  }
  return props.albums.filter((x) => !props.modelValue.albums.includes(x.id));
});

const selectedAlbums = computed(() => {
  if (!props.modelValue.albums) {
    return [];
  }
  return props.albums.filter((x) => props.modelValue.albums.includes(x.id));
});

const addAlbum = (album: AlbumEditModel) => {
  props.modelValue.albums.push(album.id);
};

const removeAlbum = (album: AlbumEditModel) => {
  props.modelValue.albums = props.modelValue.albums.filter(
    (x) => x != album.id,
  );
};
</script>
<style lang="scss" scoped>
@use "@styles/variables.scss";
.album-list {
  list-style: none;
  padding: 0;
  max-height: 160px;
  overflow: auto;
  li {
    cursor: pointer;
    color: variables.$akred;
    margin-bottom: 10px;
  }
}
</style>
