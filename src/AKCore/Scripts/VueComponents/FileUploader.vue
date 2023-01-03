<template>
  <form
    class="file-upload-area"
    :class="{ 'drag-hover': formDragHover }"
    v-on:submit.prevent
    v-on:drag.stop.prevent
    v-on:dragstart.stop.prevent
    v-on:dragover.stop.prevent="enterDrag"
    v-on:dragenter.stop.prevent="enterDrag"
    v-on:dragleave.stop.prevent="leaveDrag"
    v-on:dragend.stop.prevent="leaveDrag"
    v-on:drop.stop.prevent="onDrop"
  >
    <slot name="content"></slot>
    <label class="btn btn-default btn-file">
      <span>{{ buttonText }}</span>
      <input class="input-file" @change="onChange" multiple type="file" />
    </label>
  </form>
</template>
<script setup lang="ts">
import { ref } from "vue";

const emit = defineEmits<{
  (e: "upload", files: FileList): void;
}>();

defineProps<{
  buttonText: string;
}>();

const formDragHover = ref(false);

const onDrop = (e: DragEvent) => {
  formDragHover.value = false;
  processFiles(e.dataTransfer?.files);
};

const onChange = (e: Event) => {
  const target = e.target as HTMLInputElement;
  if (target.files && target.files.length > 0) {
    const files = target.files;
    processFiles(files);
    target.value = "";
  }
};

const enterDrag = () => {
  formDragHover.value = true;
};

const leaveDrag = (e: DragEvent) => {
  formDragHover.value = false;
};

const processFiles = (files?: FileList) => {
  if (files) {
    emit("upload", files);
  }
};
</script>

<style lang="scss" scoped>
.file-upload-area {
  padding: 20px;
  border: 2px dashed #a5a2a0;
  border-radius: 10px;
  text-align: center;
  &.drag-hover {
    background-color: #eee;
  }
}
.input-file {
  display: none;
}
.btn {
  margin-top: 20px;
}
</style>
