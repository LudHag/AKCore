<template>
  <div class="media-library row" v-if="categories">
    <div class="col-sm-4">
      <div class="tags">
        <div
          class="tag"
          v-for="(category, name) in categories"
          :class="{ selected : name === selectedCategory}"
          :key="name"
          @drop.prevent="onFileDrop($event, name)"
          @dragover.prevent="onFileDragover"
          @dragleave="onFileDragleave"
          @click="selectedCategory = name"
        >{{name}} ({{category.length}})</div>
      </div>
      <p class="tag-info">Dra filer till en kategori för att ändra kategori.</p>
    </div>
    <div class="col-sm-8" v-if="files">
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
          <a
            href="#"
            class="remove-file glyphicon glyphicon-remove"
            @click.prevent.stop="remove(file)"
          ></a>
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
      selectedCategory: "Allmän"
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
      window.open("/media/" + file.name);
    },
    remove(file) {
      if (
        window.confirm("Är du säker på att du vill ta bort filen: " + file.name)
      ) {
        ApiService.postByUrl(
          "/Media/RemoveFile?filename=" + file.name,
          null,
          null,
          res => {
            this.$emit("update");
          }
        );
      }
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
@import "~bootstrap-sass/assets/stylesheets/bootstrap/_variables.scss";
@import "../../../Styles/variables.scss";
.tags {
  border: 3px solid $akred;
  padding: 8px;
}
.tag-info {
  margin-top: 20px;
}
.files {
  border: 3px solid $akred;
  padding: 15px;
}
.tag {
  padding: 14px 10px;
  font-size: 16px;
  cursor: pointer;
  &.selected,
  &.drag {
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
  @media screen and (max-width: $screen-xs-max) {
    grid-template-columns: 45% 45%;
  }
}
.file {
  cursor: pointer;
  width: 100%;
  height: auto;
  max-height: 200px;
  text-align: center;
  font-size: 12px;
  position: relative;

  .remove-file {
    position: absolute;
    top: 0;
    right: 0;
    background-color: $akred;
    color: #fff;
    padding: 5px;
    border-radius: 50%;
    transform: translate(50%, -50%);
  }

  img {
    width: 100%;
    max-height: 150px;
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