<template>
  <div class="text-edit__toolbar">
    <select
      class="text-edit__style-select"
      :disabled="showCodeView"
      :value="selectedStyleIndex"
      @change="applyStyle($event)"
    >
      <option value="" disabled>Stil</option>
      <option
        v-for="(option, index) in TEXT_STYLE_OPTIONS"
        :key="option.label"
        :value="index"
      >
        {{ option.label }}
      </option>
    </select>

    <span class="text-edit__separator"></span>

    <button
      type="button"
      class="text-edit__btn"
      :class="{ 'is-active': editor?.isActive('bold') }"
      :disabled="showCodeView"
      title="Fet"
      @click="editor?.chain().focus().toggleBold().run()"
    >
      <span class="glyphicon glyphicon-bold"></span>
    </button>
    <button
      type="button"
      class="text-edit__btn text-edit__btn--italic"
      :class="{ 'is-active': editor?.isActive('italic') }"
      :disabled="showCodeView"
      title="Kursiv"
      @click="editor?.chain().focus().toggleItalic().run()"
    >
      I
    </button>
    <button
      type="button"
      class="text-edit__btn text-edit__btn--underline"
      :class="{ 'is-active': editor?.isActive('underline') }"
      :disabled="showCodeView"
      title="Understruken"
      @click="editor?.chain().focus().toggleUnderline().run()"
    >
      U
    </button>
    <button
      type="button"
      class="text-edit__btn text-edit__btn--strike"
      :class="{ 'is-active': editor?.isActive('strike') }"
      :disabled="showCodeView"
      title="Genomstruken"
      @click="editor?.chain().focus().toggleStrike().run()"
    >
      S
    </button>

    <span class="text-edit__separator"></span>

    <button
      type="button"
      class="text-edit__btn"
      :class="{ 'is-active': editor?.isActive({ textAlign: 'left' }) }"
      :disabled="showCodeView"
      title="Vänster"
      @click="editor?.chain().focus().setTextAlign('left').run()"
    >
      <span class="glyphicon glyphicon-align-left"></span>
    </button>
    <button
      type="button"
      class="text-edit__btn"
      :class="{ 'is-active': editor?.isActive({ textAlign: 'center' }) }"
      :disabled="showCodeView"
      title="Centrerad"
      @click="editor?.chain().focus().setTextAlign('center').run()"
    >
      <span class="glyphicon glyphicon-align-center"></span>
    </button>
    <button
      type="button"
      class="text-edit__btn"
      :class="{ 'is-active': editor?.isActive({ textAlign: 'right' }) }"
      :disabled="showCodeView"
      title="Höger"
      @click="editor?.chain().focus().setTextAlign('right').run()"
    >
      <span class="glyphicon glyphicon-align-right"></span>
    </button>
    <button
      type="button"
      class="text-edit__btn"
      :class="{ 'is-active': editor?.isActive({ textAlign: 'justify' }) }"
      :disabled="showCodeView"
      title="Marginaljusterad"
      @click="editor?.chain().focus().setTextAlign('justify').run()"
    >
      <span class="glyphicon glyphicon-align-justify"></span>
    </button>

    <span class="text-edit__separator"></span>

    <button
      type="button"
      class="text-edit__btn"
      :class="{ 'is-active': editor?.isActive('bulletList') }"
      :disabled="showCodeView"
      title="Punktlista"
      @click="editor?.chain().focus().toggleBulletList().run()"
    >
      <span class="glyphicon glyphicon-list"></span>
    </button>
    <button
      type="button"
      class="text-edit__btn"
      :disabled="showCodeView || !editor?.can().undo()"
      title="Ångra"
      @click="editor?.chain().focus().undo().run()"
    >
      <span class="glyphicon glyphicon-share-alt text-edit__flip-h"></span>
    </button>
    <button
      type="button"
      class="text-edit__btn"
      :disabled="showCodeView || !editor?.can().redo()"
      title="Gör om"
      @click="editor?.chain().focus().redo().run()"
    >
      <span class="glyphicon glyphicon-share-alt"></span>
    </button>

    <span class="text-edit__separator"></span>

    <button
      type="button"
      class="text-edit__btn"
      :class="{
        'is-active':
          editor?.isActive('link') ||
          (editor?.isActive('imageResize') &&
            !!editor?.getAttributes('imageResize').href),
      }"
      :disabled="showCodeView"
      title="Länk"
      @click="setLink"
    >
      <span class="glyphicon glyphicon-link"></span>
    </button>
    <button
      type="button"
      class="text-edit__btn"
      :disabled="
        showCodeView ||
        (!editor?.isActive('link') &&
          !(
            editor?.isActive('imageResize') &&
            !!editor?.getAttributes('imageResize').href
          ))
      "
      title="Ta bort länk"
      @click="removeLink"
    >
      <span class="glyphicon glyphicon-remove-circle"></span>
    </button>
    <button
      type="button"
      class="text-edit__btn"
      :disabled="showCodeView"
      title="Bild"
      @click="pickImage"
    >
      <span class="glyphicon glyphicon-picture"></span>
    </button>
    <button
      type="button"
      class="text-edit__btn"
      :disabled="showCodeView"
      title="Dokument"
      @click="pickFile"
    >
      <span class="glyphicon glyphicon-file"></span>
    </button>

    <span class="text-edit__separator"></span>

    <button
      type="button"
      class="text-edit__btn"
      :disabled="showCodeView"
      title="Horisontell linje"
      @click="editor?.chain().focus().setHorizontalRule().run()"
    >
      <span class="glyphicon glyphicon-minus"></span>
    </button>
    <button
      type="button"
      class="text-edit__btn"
      :disabled="showCodeView"
      title="Rensa formatering"
      @click="clearFormatting"
    >
      <span class="glyphicon glyphicon-erase"></span>
    </button>
    <text-edit-table-toolbar :editor="editor" :show-code-view="showCodeView" />

    <span class="text-edit__separator"></span>

    <button
      type="button"
      class="text-edit__btn"
      :class="{ 'is-active': showCodeView }"
      title="HTML-kod (klicka igen för att tillämpa)"
      @click="emit('toggle-code-view')"
    >
      <span class="glyphicon glyphicon-console"></span>
    </button>
    <button
      type="button"
      class="text-edit__btn"
      :class="{ 'is-active': isFullscreen }"
      title="Helskärm (Esc för att avsluta)"
      @click="emit('toggle-fullscreen')"
    >
      <span class="glyphicon glyphicon-fullscreen"></span>
    </button>

    <link-modal
      v-if="showLinkModal"
      :show-modal="showLinkModal"
      :initial-url="linkUrl"
      :initial-text="linkInitialText"
      :show-text-field="linkShowTextField"
      @apply="applyLink"
      @close="showLinkModal = false"
    />
  </div>
</template>

<script setup lang="ts">
import { EventBus } from "@utils/eventbus";
import { getMarkRange } from "@tiptap/core";
import type { Editor } from "@tiptap/core";
import { computed, ref } from "vue";
import LinkModal from "./LinkModal.vue";
import { TEXT_STYLE_OPTIONS, type TextStyleOption } from "./textEditExtensions";
import TextEditTableToolbar from "./TextEditTableToolbar.vue";

const emit = defineEmits<{
  (e: "toggle-code-view"): void;
  (e: "toggle-fullscreen"): void;
}>();

const props = defineProps<{
  editor: Editor | undefined;
  showCodeView: boolean;
  isFullscreen: boolean;
}>();

const showLinkModal = ref(false);
const linkUrl = ref("");
const linkInitialText = ref("");
const linkTarget = ref<"text" | "image">("text");
const linkShowTextField = ref(false);
const linkIsEditing = ref(false);

const selectedStyleIndex = computed(() => {
  if (!props.editor) {
    return "";
  }

  for (let index = 0; index < TEXT_STYLE_OPTIONS.length; index++) {
    const option = TEXT_STYLE_OPTIONS[index];

    if (option.block === "heading" && option.level) {
      if (!props.editor.isActive("heading", { level: option.level })) {
        continue;
      }

      const editorClass = props.editor.getAttributes("heading").class ?? null;
      if (editorClass === (option.className ?? null)) {
        return index;
      }

      continue;
    }

    if (!props.editor.isActive("paragraph")) {
      continue;
    }

    const editorClass = props.editor.getAttributes("paragraph").class ?? null;
    if (editorClass === (option.className ?? null)) {
      return index;
    }
  }

  return "";
});

const applyStyle = (event: Event) => {
  const select = event.target as HTMLSelectElement;
  const index = Number(select.value);
  if (Number.isNaN(index)) {
    return;
  }

  applyTextStyle(TEXT_STYLE_OPTIONS[index]);
};

const applyTextStyle = (option: TextStyleOption) => {
  if (!props.editor) {
    return;
  }

  const chain = props.editor.chain().focus();

  if (option.block === "heading" && option.level) {
    chain.setHeading({ level: option.level });
    chain.updateAttributes("heading", { class: option.className ?? null });
  } else {
    chain.setParagraph();
    chain.updateAttributes("paragraph", { class: option.className ?? null });
  }

  chain.run();
};

const setLink = () => {
  if (!props.editor) {
    return;
  }

  if (props.editor.isActive("imageResize")) {
    const previousUrl = props.editor.getAttributes("imageResize").href as
      | string
      | undefined;
    linkTarget.value = "image";
    linkUrl.value = previousUrl || "https://";
    showLinkModal.value = true;
    return;
  }

  const previousUrl = props.editor.getAttributes("link").href as
    | string
    | undefined;
  const { $from, from, to } = props.editor.state.selection;
  const hasSelection = from !== to;
  const editingExisting = !hasSelection && props.editor.isActive("link");

  let existingText = "";
  if (editingExisting) {
    const range = getMarkRange($from, props.editor.schema.marks.link);
    if (range) {
      existingText = props.editor.state.doc.textBetween(range.from, range.to);
    }
  }

  linkTarget.value = "text";
  linkUrl.value = previousUrl || "https://";
  linkInitialText.value = existingText;
  linkShowTextField.value = !hasSelection;
  linkIsEditing.value = editingExisting;
  showLinkModal.value = true;
};

const applyLink = (url: string, text: string) => {
  showLinkModal.value = false;

  if (!props.editor) {
    return;
  }

  if (linkTarget.value === "image") {
    props.editor
      .chain()
      .focus()
      .updateAttributes("imageResize", { href: url || null })
      .run();
    return;
  }

  if (linkShowTextField.value) {
    const displayText = text || url;
    if (linkIsEditing.value) {
      const range = getMarkRange(
        props.editor.state.selection.$from,
        props.editor.schema.marks.link,
      );
      if (range) {
        props.editor
          .chain()
          .focus()
          .setTextSelection(range)
          .insertContent(`<a href="${url}">${displayText}</a>`)
          .run();
        return;
      }
    }
    props.editor
      .chain()
      .focus()
      .insertContent(`<a href="${url}">${displayText}</a>`)
      .run();
    return;
  }

  props.editor
    .chain()
    .focus()
    .extendMarkRange("link")
    .setLink({ href: url })
    .run();
};

const removeLink = () => {
  if (!props.editor) {
    return;
  }

  if (props.editor.isActive("imageResize")) {
    props.editor
      .chain()
      .focus()
      .updateAttributes("imageResize", { href: null, target: null, rel: null })
      .run();
    return;
  }

  props.editor.chain().focus().extendMarkRange("link").unsetLink().run();
};

const pickImage = () => {
  EventBus.trigger("loadimage", (url: string) => {
    props.editor?.chain().focus().setImage({ src: url }).run();
  });
};

const pickFile = () => {
  EventBus.trigger("loadfile", (url: string) => {
    if (!props.editor) {
      return;
    }

    const { from, to } = props.editor.state.selection;
    if (from !== to) {
      props.editor.chain().focus().setLink({ href: url }).run();
      return;
    }

    const fileName = url.split("/").pop() || url;
    props.editor
      .chain()
      .focus()
      .insertContent(`<a href="${url}">${fileName}</a>`)
      .run();
  });
};

const clearFormatting = () => {
  props.editor?.chain().focus().unsetAllMarks().clearNodes().run();
};
</script>

<style lang="scss" scoped>
.text-edit__toolbar {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 2px;
  padding: 4px;
  border-bottom: 1px solid #ccc;
}

.text-edit__style-select {
  min-width: 120px;
  height: 28px;
  font-size: 12px;
}

.text-edit__btn {
  min-width: 28px;
  height: 28px;
  padding: 0 6px;
  border: 1px solid transparent;
  border-radius: 2px;
  background: transparent;
  cursor: pointer;
  font-size: 12px;
  line-height: 1;

  &:hover:not(:disabled) {
    background: #e8e8e8;
  }

  &.is-active {
    background: #d0d0d0;
    border-color: #aaa;
  }

  &:disabled {
    opacity: 0.35;
    cursor: default;
  }

  &--italic {
    font-style: italic;
    font-weight: bold;
  }

  &--underline {
    text-decoration: underline;
    font-weight: bold;
  }

  &--strike {
    text-decoration: line-through;
    font-weight: bold;
  }
}

.text-edit__flip-h {
  display: inline-block;
  transform: scaleX(-1);
}

.text-edit__separator {
  width: 1px;
  height: 20px;
  margin: 0 4px;
  background: #ccc;
}
</style>
