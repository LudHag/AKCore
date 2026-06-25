<template>
  <div ref="tablePickerRef" class="text-edit__table-picker">
    <button
      type="button"
      class="text-edit__btn"
      :class="{ 'is-active': showTablePicker || isInTable }"
      :disabled="showCodeView"
      title="Infoga tabell"
      @click.stop="toggleTablePicker"
    >
      <span class="glyphicon glyphicon-th"></span>
    </button>
    <div v-if="showTablePicker" class="text-edit__table-grid-popup">
      <div class="text-edit__table-grid-label">
        {{
          gridHoverRows && gridHoverCols
            ? `${gridHoverRows} × ${gridHoverCols}`
            : "Välj storlek"
        }}
      </div>
      <div class="text-edit__table-grid" @mouseleave="resetGridHover">
        <button
          v-for="cell in tableGridCells"
          :key="`${cell.row}-${cell.col}`"
          type="button"
          class="text-edit__table-grid-cell"
          :class="{
            'is-highlighted':
              cell.row <= gridHoverRows && cell.col <= gridHoverCols,
          }"
          :title="`${cell.row} × ${cell.col}`"
          @mouseenter="setGridHover(cell.row, cell.col)"
          @click="insertTableAt(cell.row, cell.col)"
        ></button>
      </div>
    </div>
  </div>

  <template v-if="isInTable">
    <button
      type="button"
      class="text-edit__btn text-edit__btn--table-op"
      :disabled="showCodeView"
      title="Lägg till rad ovanför"
      @click="editor?.chain().focus().addRowBefore().run()"
    >
      R↑
    </button>
    <button
      type="button"
      class="text-edit__btn text-edit__btn--table-op"
      :disabled="showCodeView"
      title="Lägg till rad under"
      @click="editor?.chain().focus().addRowAfter().run()"
    >
      R↓
    </button>
    <button
      type="button"
      class="text-edit__btn text-edit__btn--table-op"
      :disabled="showCodeView || !editor?.can().deleteRow()"
      title="Ta bort rad"
      @click="editor?.chain().focus().deleteRow().run()"
    >
      R−
    </button>
    <button
      type="button"
      class="text-edit__btn text-edit__btn--table-op"
      :disabled="showCodeView"
      title="Lägg till kolumn till vänster"
      @click="editor?.chain().focus().addColumnBefore().run()"
    >
      K←
    </button>
    <button
      type="button"
      class="text-edit__btn text-edit__btn--table-op"
      :disabled="showCodeView"
      title="Lägg till kolumn till höger"
      @click="editor?.chain().focus().addColumnAfter().run()"
    >
      K→
    </button>
    <button
      type="button"
      class="text-edit__btn text-edit__btn--table-op"
      :disabled="showCodeView || !editor?.can().deleteColumn()"
      title="Ta bort kolumn"
      @click="editor?.chain().focus().deleteColumn().run()"
    >
      K−
    </button>
    <button
      type="button"
      class="text-edit__btn"
      :disabled="showCodeView"
      title="Ta bort tabell"
      @click="editor?.chain().focus().deleteTable().run()"
    >
      <span class="glyphicon glyphicon-trash"></span>
    </button>
  </template>
</template>

<script setup lang="ts">
import type { Editor } from "@tiptap/core";
import {
  computed,
  onMounted,
  onUnmounted,
  ref,
  watch,
} from "vue";

const TABLE_GRID_SIZE = 8;

const tableGridCells = Array.from(
  { length: TABLE_GRID_SIZE * TABLE_GRID_SIZE },
  (_, index) => ({
    row: Math.floor(index / TABLE_GRID_SIZE) + 1,
    col: (index % TABLE_GRID_SIZE) + 1,
  }),
);

const props = defineProps<{
  editor: Editor | undefined;
  showCodeView: boolean;
}>();

const tablePickerRef = ref<HTMLElement | null>(null);
const showTablePicker = ref(false);
const gridHoverRows = ref(0);
const gridHoverCols = ref(0);
const editorRevision = ref(0);

const isInTable = computed(() => {
  void editorRevision.value;
  return props.editor?.isActive("table") ?? false;
});

watch(
  () => props.editor,
  (editor, _, onCleanup) => {
    if (!editor) {
      return;
    }

    const bumpRevision = () => {
      editorRevision.value++;
    };

    editor.on("transaction", bumpRevision);
    editor.on("selectionUpdate", bumpRevision);
    onCleanup(() => {
      editor.off("transaction", bumpRevision);
      editor.off("selectionUpdate", bumpRevision);
    });
  },
  { immediate: true },
);

watch(
  () => props.showCodeView,
  (showCodeView) => {
    if (showCodeView) {
      showTablePicker.value = false;
    }
  },
);

const toggleTablePicker = () => {
  showTablePicker.value = !showTablePicker.value;
  if (showTablePicker.value) {
    resetGridHover();
  }
};

const setGridHover = (rows: number, cols: number) => {
  gridHoverRows.value = rows;
  gridHoverCols.value = cols;
};

const resetGridHover = () => {
  gridHoverRows.value = 0;
  gridHoverCols.value = 0;
};

const insertTableAt = (rows: number, cols: number) => {
  props.editor
    ?.chain()
    .focus()
    .insertTable({ rows, cols, withHeaderRow: true })
    .run();
  showTablePicker.value = false;
  resetGridHover();
};

const onDocumentClick = (event: MouseEvent) => {
  if (!showTablePicker.value) {
    return;
  }

  if (tablePickerRef.value?.contains(event.target as Node)) {
    return;
  }

  showTablePicker.value = false;
  resetGridHover();
};

onMounted(() => {
  document.addEventListener("click", onDocumentClick);
});

onUnmounted(() => {
  document.removeEventListener("click", onDocumentClick);
});
</script>

<style lang="scss" scoped>
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

  &--table-op {
    min-width: 24px;
    padding: 0 4px;
    font-size: 10px;
    font-weight: bold;
  }
}

.text-edit__table-picker {
  position: relative;
}

.text-edit__table-grid-popup {
  position: absolute;
  top: calc(100% + 2px);
  left: 0;
  z-index: 20;
  padding: 8px;
  background: #fff;
  border: 1px solid #ccc;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
}

.text-edit__table-grid-label {
  min-height: 14px;
  margin-bottom: 4px;
  font-size: 11px;
  color: #666;
}

.text-edit__table-grid {
  display: grid;
  grid-template-columns: repeat(8, 16px);
  gap: 2px;
}

.text-edit__table-grid-cell {
  width: 16px;
  height: 16px;
  padding: 0;
  border: 1px solid #ddd;
  background: #fff;
  cursor: pointer;

  &.is-highlighted {
    background: #b3d4fc;
    border-color: #6aa9f7;
  }

  &:hover {
    border-color: #999;
  }
}
</style>
