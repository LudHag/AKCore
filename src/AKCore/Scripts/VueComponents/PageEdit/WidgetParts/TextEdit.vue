<template>
  <div :class="fullwidth ? 'col-sm-12' : 'col-sm-6'">
    <textarea
      class="placeholder"
      v-if="disabled"
      v-html="modelValue"
      disabled
    ></textarea>

    <div
      v-else
      class="text-edit"
      :class="{ 'text-edit--fullscreen': isFullscreen }"
      @keydown.esc="isFullscreen = false"
      tabindex="-1"
    >
      <link rel="stylesheet" :href="contentCssHref" />

      <text-edit-toolbar
        :editor="editor"
        :show-code-view="showCodeView"
        :is-fullscreen="isFullscreen"
        @toggle-code-view="toggleCodeView"
        @toggle-fullscreen="isFullscreen = !isFullscreen"
      />

      <div v-if="showCodeView" class="text-edit__code-bar">
        <span class="text-edit__code-label">HTML-läge</span>
        <button
          type="button"
          class="btn btn-xs btn-primary"
          @click="applyCodeHtml"
        >
          Tillämpa
        </button>
        <button
          type="button"
          class="btn btn-xs btn-default"
          @click="cancelCodeView"
        >
          Avbryt
        </button>
      </div>

      <div
        v-if="!showCodeView"
        class="text-edit__content body-content"
      >
        <EditorContent :editor="editor" />
      </div>

      <textarea
        v-else
        class="text-edit__code"
        v-model="codeHtml"
      ></textarea>
    </div>
  </div>
</template>

<script setup lang="ts">
import Link from "@tiptap/extension-link";
import { Table } from "@tiptap/extension-table";
import TableCell from "@tiptap/extension-table-cell";
import TableHeader from "@tiptap/extension-table-header";
import TableRow from "@tiptap/extension-table-row";
import TextAlign from "@tiptap/extension-text-align";
import Underline from "@tiptap/extension-underline";
import StarterKit from "@tiptap/starter-kit";
import { EditorContent, useEditor } from "@tiptap/vue-3";
import { EventBus } from "@utils/eventbus";
import { computed, onMounted, ref, watch } from "vue";
import { ResizableImage } from "./resizableImageExtension";
import { CustomClass } from "./textEditExtensions";
import TextEditToolbar from "./TextEditToolbar.vue";

declare const cssMain: string;

const emit = defineEmits<{
  (e: "update:modelValue", value: string): void;
}>();

const props = defineProps<{
  modelValue: string | undefined;
  fullwidth?: boolean;
}>();

const disabled = ref(false);
const showCodeView = ref(false);
const isFullscreen = ref(false);
const codeHtml = ref("");
const previousHtml = ref("");

const contentCssHref = computed(() => "/dist/" + cssMain);

const editor = useEditor({
  content: props.modelValue || "",
  editable: true,
  extensions: [
    StarterKit.configure({
      heading: { levels: [1, 2] },
    }),
    Underline,
    TextAlign.configure({
      types: ["heading", "paragraph"],
    }),
    Link.configure({
      openOnClick: false,
      autolink: false,
    }),
    ResizableImage,
    Table.configure({
      resizable: false,
    }),
    TableRow,
    TableHeader,
    TableCell,
    CustomClass,
  ],
  onUpdate: ({ editor: currentEditor }) => {
    emit("update:modelValue", currentEditor.getHTML());
  },
});

onMounted(() => {
  EventBus.on("widgetDrag", (value: boolean) => {
    disabled.value = value;
  });
});

watch(
  () => props.modelValue,
  (value) => {
    if (!editor.value || showCodeView.value) {
      return;
    }
    const current = editor.value.getHTML();
    if (value !== current) {
      editor.value.commands.setContent(value || "", { emitUpdate: false });
    }
  },
);

watch(disabled, (value) => {
  editor.value?.setEditable(!value);
});

const toggleCodeView = () => {
  if (!editor.value) {
    return;
  }

  if (!showCodeView.value) {
    previousHtml.value = editor.value.getHTML();
    codeHtml.value = previousHtml.value;
    showCodeView.value = true;
    return;
  }

  applyCodeHtml();
};

const applyCodeHtml = () => {
  if (!editor.value) {
    return;
  }

  editor.value.commands.setContent(codeHtml.value, { emitUpdate: true });
  showCodeView.value = false;
};

const cancelCodeView = () => {
  codeHtml.value = "";
  showCodeView.value = false;
};
</script>

<style lang="scss" scoped>
.placeholder {
  width: 100%;
  height: 288px;
}

.text-edit {
  border: 1px solid #ccc;
  background: #fff;

  &--fullscreen {
    position: fixed;
    inset: 0;
    z-index: 1050;
    display: flex;
    flex-direction: column;
  }
}

.text-edit__code-bar {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 4px 8px;
  background: #fff8e1;
  border-bottom: 1px solid #ffe082;
  font-size: 12px;
}

.text-edit__code-label {
  flex: 1;
  color: #666;
  font-style: italic;
}

.text-edit__content {
  min-height: 200px;
  padding: 8px 12px;

  :deep(.ProseMirror) {
    min-height: 180px;
    outline: none;

    p {
      margin: 0 0 0.5em;
    }

    table {
      border-collapse: collapse;
      width: 100%;

      td,
      th {
        border: 1px solid #ccc;
        padding: 4px 8px;
      }
    }

    img {
      max-width: 100%;
      height: auto;
    }
  }
}

.text-edit--fullscreen .text-edit__content {
  flex: 1;
  overflow: auto;

  :deep(.ProseMirror) {
    min-height: 100%;
  }
}

.text-edit__code {
  width: 100%;
  min-height: 200px;
  padding: 8px 12px;
  border: 0;
  font-family: monospace;
  font-size: 13px;
  resize: vertical;
}

.text-edit--fullscreen .text-edit__code {
  flex: 1;
  min-height: 0;
}
</style>
