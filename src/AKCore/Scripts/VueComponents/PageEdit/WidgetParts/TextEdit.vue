<template>
  <div :class="fullwidth ? 'col-sm-12' : 'col-sm-6'">
    <textarea ref="editor" class="mce-content" v-html="value"></textarea>
  </div>
</template>
<script>
import { EventBus } from "../../../utils/eventbus";
export default {
  props: ["value", "fullwidth"],
  data() {
    return {
      editorId: null
    };
  },
  mounted() {
    EventBus.$on("editor-updated", this.update);
  },
  beforeDestroy() {
    EventBus.$off("editor-updated", this.update);
  },
  methods: {
    update(event) {
      if (!this.editorId) {
        this.editorId = this.$refs.editor.id;
      }
      if (this.editorId === event.id) {
        this.$emit("input", event.content);
      }
    }
  }
};
</script>
<style lang="scss" scoped></style>
