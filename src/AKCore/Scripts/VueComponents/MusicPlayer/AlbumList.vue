<template>
  <div class="album-list">
    <ul>
      <li v-for="album in albumList" :key="album.id">{{album.name}}</li>
    </ul>
  </div>
</template>
<script>
import ApiService from "../../services/apiservice";

export default {
  props: ["albums"],
  computed: {
    albumList: function () {
      const keys = Object.keys(this.albums);
      return keys.map(key => Object.assign(this.albums[key], {id: key}));
    }
  }
}
</script>
<style lang="scss" scoped>
</style>
