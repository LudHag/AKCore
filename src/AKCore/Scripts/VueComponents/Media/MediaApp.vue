<template>
  <div id="media-app">
    <media-list :categories="categories" @update="loadMediaList"></media-list>
  </div>
</template>
<script>
import ApiService from "../../services/apiservice";
import MediaList from "./MediaList";

export default {
  components: {
    MediaList
  },
  data() {
    return {
      categories: null
    };
  },
  methods: {
    loadMediaList() {
      ApiService.get("/Media/MediaData", null, res => {
        this.categories = res;
      });
    }
  },
  created() {
    this.loadMediaList();
  }
};
</script>
<style lang="scss" scoped>
</style>
