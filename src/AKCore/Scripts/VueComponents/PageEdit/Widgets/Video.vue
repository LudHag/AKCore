<template>
  <div>
    <div class="col-sm-12" v-if="modelValue.videos">
      <div class="row">
        <div class="col-sm-6">
          <label>Video</label>
        </div>
        <div class="col-sm-6">
          <label>Namn</label>
        </div>
      </div>
      <draggable
        v-model="videoList"
        @update:modelValue="sortValues($event)"
        item-key="index"
        handle=".video-drag-area"
      >
        <template #item="{ element }">
          <div class="form-group video-area row">
            <div
              class="col-sm-1 glyphicon glyphicon-align-justify video-drag-area"
            ></div>
            <div class="col-sm-5">
              <input
                class="form-control video-link"
                :value="element.link"
                @keyup="updateLink($event, element)"
              />
            </div>
            <div class="col-sm-6">
              <input
                class="form-control video-title"
                :value="element.title"
                @keyup="updateTitle($event, element)"
              /><a
                href="#"
                class="btn glyphicon glyphicon-remove remove-video"
                @click.prevent="removeVideo(element.index)"
              ></a>
            </div>
          </div>
        </template>
      </draggable>
    </div>
    <div class="col-sm-6">
      <a href="#" class="btn btn-default" @click.prevent="addVideo"
        >Lägg till video</a
      >
    </div>
  </div>
</template>
<script setup lang="ts">
import { onMounted, ref } from "vue";
import draggable from "vuedraggable";
import { WidgetEditModel, EditVideoModel } from "../models";

const emit = defineEmits<{
  (e: "update:modelValue", value: WidgetEditModel): void;
}>();

const prop = defineProps<{
  modelValue: WidgetEditModel;
}>();

const videoList = ref<EditVideoModel[]>([]);

onMounted(() => {
  videoList.value =
    prop.modelValue.videos?.map((video, index) => {
      return { ...video, index };
    }) ?? [];
});

const updateLink = (event: Event, element: EditVideoModel) => {
  const value = (event.target as HTMLInputElement).value;
  videoList.value = videoList.value.map((video) => {
    if (video.index === element.index) {
      return { ...video, link: value };
    }
    return video;
  });
  const updatedValue: WidgetEditModel = {
    ...prop.modelValue,
    videos: videoList.value,
  };

  emit("update:modelValue", updatedValue);
};

const updateTitle = (event: Event, element: EditVideoModel) => {
  const value = (event.target as HTMLInputElement).value;
  videoList.value = videoList.value.map((video) => {
    if (video.index === element.index) {
      return { ...video, title: value };
    }
    return video;
  });

  updateVideos();
};

const removeVideo = (removeIndex: number) => {
  videoList.value = videoList.value.filter(
    (video) => video.index !== removeIndex
  );

  updateVideos();
};

const addVideo = () => {
  if (!videoList.value) {
    videoList.value = [];
  }

  const newIndex =
    videoList.value.reduce(
      (prev, current) => (prev > current.index! ? prev : current.index!),
      0
    ) + 1;

  videoList.value.push({ title: "", link: "", index: newIndex });
  updateVideos();
};

const sortValues = (event: EditVideoModel[]) => {
  videoList.value = event;
  updateVideos();
};

const updateVideos = () => {
  const updatedValue: WidgetEditModel = {
    ...prop.modelValue,
    videos: videoList.value,
  };
  emit("update:modelValue", updatedValue);
};
</script>
<style lang="scss" scoped>
.video-area {
  position: relative;
}
.btn.remove-video {
  color: black;
  position: absolute;
  right: 10px;
}
.video-drag-area {
  height: 34px;
  display: flex;
  align-items: center;
  cursor: grab;
}
</style>
