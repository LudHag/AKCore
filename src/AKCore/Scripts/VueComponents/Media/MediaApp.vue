<template>
  <div id="media-app"></div>
</template>
<script>
import ApiService from "../../services/apiservice";

export default {
  data() {
    return {
      mediaList: null
    };
  },
  methods: {
    loadMediaList() {
      ApiService.get("/MenuEdit/MenuListData", null, res => {
        this.menus = res.menus;
        this.pages = res.pages;
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
