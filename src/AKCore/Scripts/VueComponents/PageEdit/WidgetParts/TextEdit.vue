<template>
  <div class="col-sm-6">
    <!-- <textarea class="mce-content" v-html="value" @input="update"></textarea> -->
    <div ref="pell" class="pell" />
  </div>
</template>
<script>
import { exec, init } from "pell";

export default {
  props: ["value"],
  data() {
    return {
      editor: null
    };
  },
  mounted() {
    this.editor = init({
      element: this.$refs.pell,
      onChange: html => {
        this.$emit("input", html);
      },
      defaultParagraphSeparator: "p",
      classes: {
        content: "pell-content body-content",
        button: "pell-button",
        selected: "pell-button-selected"
      },
      actions: [
        {
          name: "backColor",
          icon: '<div style="background-color:pink;">A</div>',
          title: "Highlight Color",
          result: () => exec("backColor", "pink")
        },
        "bold",
        "italic",
        "underline",
        "strikethrough",
        "heading1",
        "heading2",
        "paragraph",
        "quote",
        "olist",
        "ulist",
        "code",
        "line",
        {
          name: "image",
          result: () => {
            const url = window.prompt("Enter the image URL");
            if (url) pell.exec("insertImage", url);
          }
        },
        {
          name: "link",
          result: () => {
            const url = window.prompt("Enter the link URL");
          }
        }
      ]
    });
    this.editor.content.innerHTML = this.value;
  }
};
</script>
<style lang="scss" scoped>
@import "../../../../node_modules/pell/src/pell";
.pell {
  border: 2px solid #000;
  border-radius: 0;
  box-shadow: none;
}
/deep/ .pell-content {
  height: 220px;
  overflow-y: auto;
}
</style>
