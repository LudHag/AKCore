<template>
  <div id="pageedit-app" v-if="pageModel">
    <div class="row">
      <form method="post" @submit.prevent="save">
        <div class="alert alert-danger" style="display: none;"></div>
        <div class="alert alert-success" style="display: none;"></div>
        <page-meta v-model="pageModel"></page-meta>
      </form>
    </div>
    <add-widget @add="widgetAdd"></add-widget>
    <ul class="widget-area">
      <li
        v-for="widget in pageModel.widgets"
        class="row widget"
        :class="widget.type"
        :key="widget.Id"
      >
        <div class="widget-header">
          <h4>{{ getHeader(widget.type) }}</h4>
          <span
            class="btn pull-right remove-widget glyphicon glyphicon-remove"
          ></span
          ><span
            class="btn pull-right min-widget glyphicon glyphicon-minus"
          ></span>
        </div>
        <div class="widget-body">
          <!-- <partial name="Widgets/@w.Type" model="w" /> -->
        </div>
      </li>
    </ul>
  </div>
</template>
<script>
import Constants from "../../constants";
import PageMeta from "./PageMeta";
import AddWidget from "./AddWidget";
import ApiService from "../../services/apiservice";
import { getHeader } from "./functions";

export default {
  components: {
    PageMeta,
    AddWidget
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
    getHeader,
    widgetAdd(type) {
      console.log(type);
    },
    save(e) {
      const self = this;
      const success = $(".alert-success");
      const error = $(".alert-danger");
      ApiService.postByObject(
        window.location.href,
        this.pageModel,
        error,
        success,
        null
      );
    }
  }
};
</script>
<style lang="scss">
.widget-area {
  list-style-type: none;
  padding: 0;
}
</style>
