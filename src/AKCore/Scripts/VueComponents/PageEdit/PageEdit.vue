<template>
  <div id="pageedit-app" v-if="pageModel">
    <div class="row">
      <form method="post" @submit.prevent="save">
        <div class="alert alert-danger" style="display: none;"></div>
        <div class="alert alert-success" style="display: none;"></div>
        <page-meta v-model="pageModel"></page-meta>
      </form>
    </div>
  </div>
</template>
<script>
import Constants from "../../constants";
import PageMeta from "./PageMeta";

export default {
  components: {
    PageMeta
  },
  data() {
    return {
      pageModel: null
    };
  },
  created() {
    const self = this;
    $.ajax({
      url: window.location.href + "/Model",
      type: "GET",
      success: function(res) {
        self.pageModel = res;
      }
    });
  },
  methods: {
    save(e) {
      console.log(this.pageModel.name);
    }
  }
};
</script>
<style lang="scss"></style>
