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
      <widget
        v-for="widget in pageModel.widgets"
        :value="widget"
        :key="widget.id"
        @updated="loadTiny"
        @remove="removeWidget(widget)"
      >
      </widget>
    </ul>
    <image-picker-modal
      v-if="saveImageDest"
      :show-modal="saveImageDest"
      :destination="saveImageDest"
      :notransition="true"
      @close="saveImageDest = null"
    ></image-picker-modal>
    <document-picker-modal
      v-if="saveDocumentDest"
      :show-modal="saveDocumentDest"
      :destination="saveDocumentDest"
      :notransition="true"
      @close="saveDocumentDest = null"
    >
    </document-picker-modal>
  </div>
</template>
<script>
import Constants from "../../constants";
import PageMeta from "./PageMeta";
import AddWidget from "./AddWidget";
import Widget from "./Widget";
import ApiService from "../../services/apiservice";
import { tinyMceOpts } from "./functions";
import ImagePickerModal from "../ImagePickerModal.vue";
import DocumentPickerModal from "../DocumentPickerModal.vue";

export default {
  components: {
    PageMeta,
    AddWidget,
    Widget,
    ImagePickerModal,
    DocumentPickerModal
  },
  data() {
    return {
      pageModel: null,
      saveImageDest: null,
      saveDocumentDest: null
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

    document.addEventListener(
      "keydown",
      e => {
        if (
          (window.navigator.platform.match("Mac") ? e.metaKey : e.ctrlKey) &&
          e.keyCode == 83
        ) {
          e.preventDefault();
          this.save();
        }
      },
      false
    );
  },
  updated() {
    this.loadTiny();
  },
  methods: {
    widgetAdd(type) {
      let newId = this.pageModel.widgets.length;
      while (this.pageModel.widgets.some(x => x.id === newId)) {
        newId++;
      }

      this.pageModel.widgets.push({ id: newId, type: type });
    },
    loadTiny() {
      tinymce.init(tinyMceOpts(this.selectImage, this.selectfile));
    },
    removeWidget(widget) {
      this.pageModel.widgets = this.pageModel.widgets.filter(
        x => x.id != widget.id
      );
    },
    selectImage(destination) {
      this.saveImageDest = destination;
    },
    selectfile(destination) {
      this.saveDocumentDest = destination;
    },
    save() {
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
