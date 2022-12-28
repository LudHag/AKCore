<template>
  <div id="pageedit-app" v-if="pageModel">
    <div class="row">
      <form method="post" @submit.prevent="save">
        <div class="alert alert-danger" style="display: none"></div>
        <div class="alert alert-success" style="display: none"></div>
        <div class="col-xs-12">
          <h2 v-if="selectedRevision">
            Version från {{ selectedRevision.modified }}
          </h2>
        </div>
        <page-meta v-model="usedModel"></page-meta>
        <page-versions
          :value="pageModel"
          :selectedRevision="selectedRevision"
          @select="selectRevision"
        ></page-versions>
      </form>
    </div>
    <add-widget @add="widgetAdd"></add-widget>
    <ul class="widget-area">
      <draggable
        v-model="usedModel.widgets"
        @start="drag = true"
        @end="drag = false"
        handle=".widget-header"
        item-key="id"
      >
        <template #item="{ element }">
          <widget
            :modelValue="element"
            :albums="pageModel.albums"
            @updated="loadTiny"
            @remove="removeWidget(element)"
          >
          </widget>
        </template>
      </draggable>
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
import PageMeta from "./PageMeta.vue";
import AddWidget from "./AddWidget.vue";
import Widget from "./Widget.vue";
import ApiService from "../../services/apiservice";
import { tinyMceOpts } from "./functions";
import ImagePickerModal from "../ImagePickerModal.vue";
import DocumentPickerModal from "../DocumentPickerModal.vue";
import draggable from "vuedraggable";
import PageVersions from "./PageVersions.vue";

export default {
  components: {
    PageMeta,
    AddWidget,
    Widget,
    ImagePickerModal,
    DocumentPickerModal,
    draggable,
    PageVersions,
  },
  data() {
    return {
      pageModel: null,
      saveImageDest: null,
      saveDocumentDest: null,
      drag: false,
      selectedRevision: null,
      usedModel: null,
    };
  },
  created() {
    const self = this;
    $.ajax({
      url: window.location.href + "/Model",
      type: "GET",
      success: function (res) {
        self.pageModel = res;
        self.usedModel = self.pageModel;
      },
    });
    document.addEventListener(
      "keydown",
      (e) => {
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
  watch: {
    drag(value) {
      Array.from(document.querySelectorAll(".mce-content"))
        .map((x) => x.id)
        .forEach((id) => {
          if (value) {
            tinymce.execCommand("mceRemoveEditor", false, id);
          } else {
            tinymce.execCommand("mceAddEditor", true, id);
          }
        });
    },
    selectedRevision(value) {
      if (value) {
        this.usedModel = value;
      } else {
        this.usedModel = this.pageModel;
      }
    },
  },
  methods: {
    selectRevision(revision) {
      this.selectedRevision = revision;
    },
    widgetAdd(type) {
      let newId = this.pageModel.widgets.length;
      while (this.pageModel.widgets.some((x) => x.id === newId)) {
        newId++;
      }

      this.pageModel.widgets.push({ id: newId, type: type, albums: [] });
    },
    loadTiny() {
      tinymce.init(tinyMceOpts(this.selectImage, this.selectfile));
    },
    removeWidget(widget) {
      this.pageModel.widgets = this.pageModel.widgets.filter(
        (x) => x.id != widget.id
      );
    },
    selectImage(destination) {
      this.saveImageDest = destination;
    },
    selectfile(destination) {
      this.saveDocumentDest = destination;
    },
    save() {
      if (this.selectedRevision) {
        if (
          !window.confirm(
            "Är du säker på att du vill ersätta sidan med denna version?"
          )
        ) {
          return;
        }
      }
      const self = this;
      const success = $(".alert-success");
      const error = $(".alert-danger");
      ApiService.postByObject(
        window.location.href,
        this.usedModel,
        error,
        success,
        (res) => {
          this.selectedRevision = null;
          this.pageModel = res.newModel;
        }
      );
    },
  },
};
</script>
<style lang="scss" scoped>
.widget-area {
  list-style-type: none;
  padding: 0;
}
</style>
