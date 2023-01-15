<template>
  <div id="media-app">
    <file-uploader
      :button-text="'Ladda upp en fil'"
      class="media-upload-area"
      @upload="uploadFiles"
    >
      <template #content>
        <div>
          <div
            class="alert alert-danger"
            ref="error"
            style="display: none"
          ></div>
          <p>
            Välj kategori, dra hit fil(er) eller välj med knappen för att ladda
            up fil(er).
          </p>
          <select class="form-control" v-model="selectedTag" required>
            <option v-for="cat in IMAGETYPES" :key="cat">{{ cat }}</option>
          </select>
        </div>
      </template>
    </file-uploader>

    <media-list
      :categories="categories"
      v-if="categories"
      @update="loadMediaList"
    ></media-list>
  </div>
</template>
<script setup lang="ts">
import ApiService from "../../services/apiservice";
import FileUploader from "../FileUploader.vue";
import MediaList from "./MediaList.vue";
import { IMAGETYPES } from "../../constants";
import { ref, onMounted } from "vue";
import { MediaItem } from "./models";

const categories = ref<Record<string, MediaItem[]> | null>(null);
const selectedTag = ref("Allmän");
const error = ref<HTMLElement | null>(null);

const loadMediaList = () => {
  ApiService.get(
    "/Media/MediaData",
    null,
    (res: Record<string, MediaItem[]>) => {
      categories.value = res;
      IMAGETYPES.forEach((type) => {
        if (!(type in categories.value!)) {
          categories.value![type] = [];
        }
      });
    }
  );
};

const uploadFiles = (files: FileList) => {
  const mediaData = new FormData();
  for (let i = 0; i < files.length; i++) {
    mediaData.append("UploadFiles", files[i]);
  }
  mediaData.append("Tag", selectedTag.value);
  const errorField = $(error.value!);
  ApiService.postFormData(
    "/media/UploadFiles",
    mediaData,
    errorField,
    null,
    () => {
      loadMediaList();
    }
  );
};

onMounted(() => {
  loadMediaList();
});
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
