<template>
  <div class="media-library row" v-if="categories">
    <div class="tags col-sm-3">
      <div
        class="tag"
        v-for="(category, name) in categories"
        :class="{ selected : name === selectedCategory}"
        :key="name"
        @drop.prevent="onFileDrop($event, name)"
        @dragover.prevent="onFileDragover"
        @dragleave="onFileDragleave"
        @click="selectedCategory = name"
      >{{name}}</div>
    </div>
    <div class="col-sm-9" v-if="files">
      <div class="files">
        <div
          class="file"
          v-for="file in files"
          :key="file.id"
          draggable="true"
          @dragstart="onFileDragStart($event, file)"
          @click="clickFile(file)"
        >
          <img :src="'/media/' + file.name" v-if="file.type==='Image'">
          <span class="glyphicon glyphicon-file" v-if="file.type==='Document'"></span>
          <p class="name">{{file.name}}</p>
        </div>
      </div>
    </div>
  </div>
</template>
<script>
import ApiService from "../../services/apiservice";

export default {
  props: ["categories"],
  data() {
    return {
      selectedCategory: null
    };
  },
  methods: {
    onFileDragStart(event, file) {
      event.dataTransfer.setData("text", file.id);
      event.dataTransfer.effectAllowed = "all";
    },
    onFileDrop(event, name) {
      if (event.target.classList.contains("selected")) {
        return;
      }
      event.target.classList.remove("drag");
      const id = event.dataTransfer.getData("text");
      if (name && id) {
        ApiService.postByObject(
          "/Media/EditFile",
          { Tag: name, Id: id },
          null,
          null,
          res => {
            this.$emit("update");
          }
        );
      }
    },
    onFileDragover(event) {
      if (event.target.classList.contains("selected")) {
        event.dataTransfer.dropEffect = "none";
        return;
      }
      event.dataTransfer.dropEffect = "move";
      event.target.classList.add("drag");
    },
    onFileDragleave(event) {
      event.target.classList.remove("drag");
    },
    clickFile(file) {
      window.open('/media/' + file.name);
    }
  },
  computed: {
    files() {
      if (!this.categories || !this.selectedCategory) {
        return null;
      }
      return this.categories[this.selectedCategory];
    }
  }
};
</script>
<style lang="scss" scoped>
@import "../../../Styles/variables.scss";
.tags,
.files {
  border: 3px solid $akred;
  padding: 15px;
}
.tag {
  padding: 14px 10px;
  font-size: 16px;
  cursor: pointer;
  &.selected, &.drag {
    background-color: #4d0101;
  }
}
.files {
  position: relative;
  display: grid;
  grid-template-columns: 22% 22% 22% 22%;
  grid-gap: 4%;
  grid-row-gap: 1em;
  justify-content: space-between;
}
.file {
  cursor: pointer;
  width: 100%;
  height: auto;
  max-height: 200px;
  overflow: hidden;
  text-align: center;
  font-size: 12px;
  position: relative;

  img {
    width: 100%;
  }
  .glyphicon-file {
    font-size: 100px;
    color: $akred;
    margin-right: -10px;
  }

  .name {
    word-break: break-all;
    overflow: hidden;
    text-overflow: ellipsis;
    width: 85%;
    right: 0;
    margin: auto;
    margin-top: 10px;
  }
}
</style>