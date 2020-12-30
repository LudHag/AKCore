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
        "bold",
        "italic",
        "heading1",
        "heading2",
        "quote",
        "ulist",
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
@import "../../../../Styles/variables.scss";
.pell {
}
/deep/ .pell-content {
  height: 220px;
  overflow-y: auto;
  padding: 5px;
}

/deep/ .pell-actionbar {
  background-color: #fff;
  .pell-button {
    background-color: #fff;
    outline: none;
    box-shadow: none;
    border-radius: 0;
    font-size: 20px;
    padding: 4px;
    border: 0;
    &.pell-button-selected {
      font-weight: bold;
      color: $akred;
    }
  }
}
</style>
