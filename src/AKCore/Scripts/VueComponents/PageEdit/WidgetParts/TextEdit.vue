<template>
  <div class="col-sm-6">
    <!-- <textarea class="mce-content" v-html="value" @input="update"></textarea> -->
    <editor-menu-bar :editor="editor" v-slot="{ commands, isActive }">
      <button :class="{ 'is-active': isActive.bold() }" @click="commands.bold">
        Bold
      </button>
    </editor-menu-bar>
    <editor-content class="tipeditor" :editor="editor" @input="update" />
  </div>
</template>
<script>
import { Editor, EditorContent, EditorMenuBar } from "tiptap";
import { Blockquote, Bold, Link } from "tiptap-extensions";

export default {
  components: { EditorContent, EditorMenuBar },
  props: ["value"],
  data() {
    return {
      editor: new Editor({
        extensions: [new Blockquote(), new Bold(), new Link()],
        content: this.value
      })
    };
  },
  methods: {
    update(event) {
      this.$emit("input", event.target.value);
    }
  },
  beforeDestroy() {
    this.editor.destroy();
  }
};
</script>
<style lang="scss" scoped>
.tipeditor /deep/ .ProseMirror {
  min-height: 230px;
  border: 1px solid green;
}
</style>
