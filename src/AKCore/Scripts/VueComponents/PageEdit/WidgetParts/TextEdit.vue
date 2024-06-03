<template>
  <div :class="fullwidth ? 'col-sm-12' : 'col-sm-6'">
    <textarea
      class="placeholder"
      v-if="disabled"
      v-html="modelValue"
      disabled
    ></textarea>

    <editor
      v-if="!disabled"
      api-key="no-api-key"
      :initial-value="modelValue || ''"
      :init="getConfig()"
    ></editor>
  </div>
</template>
<script setup lang="ts">
import { EventBus } from "../../../utils/eventbus";
import Editor from "@tinymce/tinymce-vue";
import { onMounted, ref, watch } from "vue";

const emit = defineEmits<{
  (e: "update:modelValue", value: string): void;
}>();

const props = defineProps<{
  modelValue: string | undefined;
  fullwidth?: boolean;
}>();

const disabled = ref(false);
const editorRef = ref(null);

onMounted(() => {
  EventBus.on("widgetDrag", (value: boolean) => {
    disabled.value = value;
  });
});

watch(
  () => props.modelValue,
  (value) => {
    if (editorRef.value) {
      // @ts-ignore
      if (value !== editorRef.value.getContent()) {
        // @ts-ignore
        editorRef.value.setContent(value || "");
      }
    }
  },
);

declare const cssMain: string;

const getConfig = () => {
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
    content_css: "/dist/" + cssMain,
    body_class: "body-content",
    setup: function (ed: any) {
      editorRef.value = ed;
      ed.on("change", function () {
        const elcontent = ed.getContent();
        emit("update:modelValue", elcontent);
      });
    },
    file_browser_callback: function (
      field_name: string,
      _url: any,
      type: any,
      // eslint-disable-next-line @typescript-eslint/no-unused-vars
      _win: any,
    ) {
      if (type === "image") {
        EventBus.trigger("loadimage", document.getElementById(field_name));
      }
      if (type === "file") {
        EventBus.trigger("loadfile", document.getElementById(field_name));
      }
    },
  };
};
</script>
<style lang="scss" scoped>
.placeholder {
  width: 100%;
  height: 288px;
}
</style>
