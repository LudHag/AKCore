<template>
  <div id="media-app">
    <file-uploader
      :button-text="'Ladda upp en fil'"
      class="media-upload-area"
      @upload="uploadFiles"
    >
      <div slot="content">
        <div class="alert alert-danger" ref="error" style="display: none;"></div>
        <p>Välj kategori, dra hit fil(er) eller välj med knappen för att ladda up fil(er).</p>
        <select class="form-control" v-model="selectedTag" required>
          <option v-for="cat in imageTypes" :key="cat">{{cat}}</option>
        </select>
      </div>
    </file-uploader>

    <media-list :categories="categories" v-if="categories" @update="loadMediaList"></media-list>
  </div>
</template>
<script>
import ApiService from "../../services/apiservice";
import FileUploader from "../FileUploader";
import MediaList from "./MediaList";
import Constants from "../../constants";

export default {
  components: {
    MediaList,
    FileUploader
  },
  data() {
    return {
      categories: null,
      selectedTag: "Allmän"
    };
  },
  computed: {
    categoryNames() {
      if (!this.categories) {
        return [];
      }
      return Object.keys(this.categories);
    },
    imageTypes() {
      return Constants.IMAGETYPES;
    }
  },
  methods: {
    loadMediaList() {
      ApiService.get("/Media/MediaData", null, res => {
        this.categories = res;
        this.imageTypes.forEach((type) => {
          if(!(type in this.categories)) {
            this.categories[type] = [];
          }
        })  
      });
    },
    uploadFiles(files) {
      const mediaData = new FormData();
      for (var i = 0; i < files.length; i++) {
        mediaData.append("UploadFiles", files[i]);
      }
      mediaData.append("Tag", this.selectedTag);
      const error = $(this.$refs.error);
      ApiService.postFormData(
        "/media/UploadFiles",
        mediaData,
        error,
        null,
        () => {
          this.loadMediaList();
        }
      );
    }
  },
  created() {
    this.loadMediaList();
  }
};
</script>
<style lang="scss" scoped>
.media-upload-area {
  margin-bottom: 30px;
}
select {
  display: inline-block;
  width: auto;
}
</style>
