<template>
  <div class="col-sm-6">
    <!-- <textarea class="mce-content" v-html="value" @input="update"></textarea> -->
    <editor-menu-bar :editor="editor" v-slot="{ commands, isActive }">
      <button
        class="fa fa-bold"
        :class="{ 'is-active': isActive.bold() }"
        @click="commands.bold"
      ></button>
    </editor-menu-bar>
    <editor-content class="tipeditor" :editor="editor" @update="update" />
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
        content: this.value,
        onUpdate: this.update
      })
    };
  },
  methods: {
    update(state) {
      this.$emit("input", state.getHTML());
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
  padding: 5px;
  border: 1px solid green;
}
</style>
