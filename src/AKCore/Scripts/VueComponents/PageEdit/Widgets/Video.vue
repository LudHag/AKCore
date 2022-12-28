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
      <draggable v-model="modelValue.videos" handle=".video-drag-area">
        <div
          class="form-group video-area row"
          v-for="(video, index) in modelValue.videos"
          :key="index"
        >
          <div
            class="col-sm-1 glyphicon glyphicon-align-justify video-drag-area"
          ></div>
          <div class="col-sm-5">
            <input class="form-control video-link" v-model="video.link" />
          </div>
          <div class="col-sm-6">
            <input class="form-control video-title" v-model="video.title" /><a
              href="#"
              class="btn glyphicon glyphicon-remove remove-video"
              @click.prevent="removeVideo(index)"
            ></a>
          </div>
        </div>
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
  methods: {
    removeVideo(removeIndex) {
      this.modelValue.videos = this.modelValue.videos.filter(
        (video, index) => removeIndex !== index
      );
    },
    addVideo() {
      this.modelValue.videos.push({ title: "", link: "" });
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
