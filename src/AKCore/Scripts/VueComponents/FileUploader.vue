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
      <span>{{buttonText}}</span>
      <input class="input-file" @change="onChange" multiple type="file">
    </label>
  </form>
</template>
<script>
export default {
  props: ["buttonText"],
  data() {
    return {
      formDragHover: false
    };
  },
  methods: {
    onDrop(e) {
      this.formDragHover = false;
      this.processFiles(e.dataTransfer.files);
    },
    onChange(e) {
      const target = event.target;
      if (target.files.length > 0) {
        const files = target.files;
        this.processFiles(files);
        target.value = "";
      }
    },
    enterDrag() {
      this.formDragHover = true;
    },
    leaveDrag(e) {
      this.formDragHover = false;
    },
    processFiles(files) {
      this.$emit("upload", files);
    }
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