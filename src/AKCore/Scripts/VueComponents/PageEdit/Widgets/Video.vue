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
        :modelValue="indexedVideos"
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
              <input class="form-control video-link" v-model="element.link" />
            </div>
            <div class="col-sm-6">
              <input
                class="form-control video-title"
                v-model="element.title"
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
<script>
import draggable from "vuedraggable";
import TextEdit from "../WidgetParts/TextEdit.vue";
export default {
  components: { TextEdit, draggable },
  props: ["modelValue"],
  computed: {
    indexedVideos() {
      return this.modelValue.videos.map((video, index) => {
        return { ...video, index };
      });
    },
  },
  methods: {
    removeVideo(removeIndex) {
      this.modelValue.videos = this.modelValue.videos.filter(
        (video, index) => removeIndex !== index
      );
    },
    addVideo() {
      this.modelValue.videos.push({ title: "", link: "" });
    },
    sortValues(event) {
      this.modelValue.videos = event.map((x) => {
        return { title: x.title, link: x.link };
      });
    },
  },
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
