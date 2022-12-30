<template>
  <div :class="fullwidth ? 'col-sm-12' : 'col-sm-6'">
    <!-- <textarea ref="editor" class="mce-content" v-html="modelValue"></textarea> -->

    <editor
      ref="editor"
      api-key="no-api-key"
      :initial-value="modelValue"
      :init="getConfig()"
    ></editor>
  </div>
</template>
<script>
import { EventBus } from "../../../utils/eventbus";
import Editor from "@tinymce/tinymce-vue";

export default {
  props: ["modelValue", "fullwidth"],
  components: {
    Editor,
  },
  data() {
    return {
      editorId: null,
    };
  },
  mounted() {
    EventBus.on("editor-updated", this.update);
  },
  beforeDestroy() {
    EventBus.off("editor-updated", this.update);
  },
  updated() {
    tinymce.get(this.getEdId()).setContent(this.modelValue);
  },
  methods: {
    getEdId() {
      if (!this.editorId) {
        this.editorId = this.$refs.editor.id;
      }
      return this.editorId;
    },
    update(event) {
      const editorId = this.getEdId();
      if (editorId === event.id) {
        this.$emit("update:modelValue", event.content);
      }
    },
    getConfig() {
      return {
        selector: ".widget-area .mce-content",
        theme: "modern",
        plugins: [
          "advlist link image imagetools lists charmap print hr anchor spellchecker searchreplace wordcount code fullscreen media nonbreaking",
          "table contextmenu directionality emoticons template textcolor paste textcolor colorpicker textpattern",
        ],
        height: 200,
        table_appearance_options: false,
        menubar: false,
        elementpath: true,
        convert_urls: false,
        toolbar1:
          "bold italic underline strikethrough | alignleft aligncenter alignright alignjustify | styleselect code fullscreen",
        toolbar2:
          "searchreplace bullist | undo redo | link unlink image | hr removeformat | charmap table",
        style_formats: [
          { title: "Vanlig text", block: "p" },
          { title: "Stor text", block: "p", classes: "big" },
          { title: "Rubrik 1", block: "h1" },
          { title: "Rubrik 2", block: "h2" },
          { title: "Infobox", selector: "p", classes: "infobox" },
          { title: "3-delskolumn", block: "p", classes: "col-sm-4" },
          { title: "Block med marginal", block: "p", classes: "col-xs-12" },
        ],
        content_css: "/dist/main.css",
        body_class: "body-content",
        setup: function (ed) {
          ed.on("change", function (e) {
            const elcontent = ed.getContent();
            EventBus.trigger("editor-updated", {
              id: ed.id,
              content: elcontent,
            });
          });
        },
      };
    },
  },
};
</script>
<style lang="scss" scoped></style>
