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
<script>
import TextEdit from "../WidgetParts/TextEdit.vue";
export default {
  components: { TextEdit },
  props: ["modelValue", "albums"],
  computed: {
    selectableAlbums() {
      if (!this.modelValue.albums) {
        return this.albums;
      }
      return this.albums.filter((x) => !this.modelValue.albums.includes(x.id));
    },
    selectedAlbums() {
      if (!this.modelValue.albums) {
        return [];
      }
      return this.albums.filter((x) => this.modelValue.albums.includes(x.id));
    },
  },
  methods: {
    addAlbum(album) {
      this.modelValue.albums.push(album.id);
    },
    removeAlbum(album) {
      this.modelValue.albums = this.modelValue.albums.filter(
        (x) => x != album.id
      );
    },
  },
};
</script>
<style lang="scss" scoped>
@import "../../../../Styles/variables.scss";
.album-list {
  list-style: none;
  padding: 0;
  max-height: 160px;
  overflow: auto;
  li {
    cursor: pointer;
    color: $akred;
    margin-bottom: 10px;
  }
}
</style>
