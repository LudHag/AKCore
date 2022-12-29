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
      if (!this.value.albums) {
        return this.albums;
      }
      return this.albums.filter((x) => !this.value.albums.includes(x.id));
    },
    selectedAlbums() {
      if (!this.value.albums) {
        return [];
      }
      return this.albums.filter((x) => this.value.albums.includes(x.id));
    },
  },
  methods: {
    addAlbum(album) {
      this.value.albums.push(album.id);
    },
    removeAlbum(album) {
      this.value.albums = this.value.albums.filter((x) => x != album.id);
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
