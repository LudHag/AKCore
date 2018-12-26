<template>
  <div class="media-library row" v-if="categories">
    <div class="tags col-sm-3">
      <div
        class="tag"
        v-for="(category, name) in categories"
        :class="{ selected : name === selectedCategory}"
        :key="name"
        @click="selectedCategory = name"
      >{{name}}</div>
    </div>
    <div class="col-sm-9" v-if="files">
      <div class="files">
        <div class="file" v-for="file in files" :key="file.id" draggable="true">
          <img :src="'/media/' + file.name" v-if="file.type==='Image'">
          <span class="glyphicon glyphicon-file" v-if="file.type==='Document'"></span>
          <p class="name">{{file.name}}</p>
        </div>
      </div>
    </div>
  </div>
</template>
<script>
export default {
  props: ["categories"],
  data() {
    return {
      selectedCategory: null
    };
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
  &.selected {
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
