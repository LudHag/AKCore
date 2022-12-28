<template>
  <div :class="fullwidth ? 'col-sm-12' : 'col-sm-6'">
    <textarea ref="editor" class="mce-content" v-html="modelValue"></textarea>
  </div>
</template>
<script>
import { EventBus } from "../../../utils/eventbus";
export default {
  props: ["modelValue", "fullwidth"],
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
        this.$emit("input", event.content);
      }
    },
  },
};
</script>
<style lang="scss" scoped></style>
