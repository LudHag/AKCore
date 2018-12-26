<template>
  <div id="media-app">
    <div class="media-library row" v-if="mediaList">
      <div class="tags col-sm-3">
        <div
          class="tag"
          v-for="(category, name) in mediaList"
          :class="{ selected : name === selectedCategory}"
          :key="name"
          @click="selectedCategory = name"
        >{{name}}</div>
      </div>
      <div class="files col-sm-9" v-if="files">
        <div class="file col-sm-3" v-for="file in files" :key="file.id">
          <img :src="'/media/' + file.name">
          <p class="name">{{file.name}}</p>
        </div>
      </div>
    </div>
  </div>
</template>
<script>
import ApiService from "../../services/apiservice";

export default {
  data() {
    return {
      mediaList: null,
      selectedCategory: null
    };
  },
  computed: {
    files() {
      if (!this.mediaList || !this.selectedCategory) {
        return null;
      }
      return this.mediaList[this.selectedCategory];
    }
  },
  methods: {
    loadMediaList() {
      ApiService.get("/Media/MediaData", null, res => {
        this.mediaList = res;
      });
    }
  },
  created() {
    this.loadMediaList();
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
  padding: 14px 0;
  font-size: 16px;
  cursor: pointer;
  &.selected {
    background-color: #4d0101;
  }
}
.file {
  cursor: pointer;
  height: 210px;
  overflow: hidden;
  text-align: center;
  font-size: 12px;

  img {
    width: 100%;
  }
  .glyphicon-file {
    font-size: 140px;
    color: $akred;
    margin-right: -10px;
  }

  .name {
    word-break: break-all;
    width: 85%;
    bottom: 20px;
    position: absolute;
    left: 0;
    right: 0;
    margin-left: auto;
    margin-right: auto;
  }
}
</style>
