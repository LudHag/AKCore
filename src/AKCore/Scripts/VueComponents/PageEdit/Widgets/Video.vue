<template>
  <div>
    <div class="col-sm-12" v-if="value.videos">
      <div class="row">
        <div class="col-sm-6">
          <label>Video</label>
        </div>
        <div class="col-sm-6">
          <label>Namn</label>
        </div>
      </div>
      <div
        class="form-group video-area row"
        v-for="video in value.videos"
        :key="video.link"
      >
        <div class="col-sm-6">
          <input class="form-control video-link" v-model="video.link" />
        </div>
        <div class="col-sm-6">
          <input class="form-control video-title" v-model="video.title" /><a
            href="#"
            class="btn glyphicon glyphicon-remove"
            @click.prevent="removeVideo(video)"
          ></a>
        </div>
      </div>
    </div>
    <div class="col-sm-6">
      <a href="#" class="btn btn-default" @click.prevent="addVideo"
        >Lägg till video</a
      >
    </div>
  </div>
</template>
<script>
import TextEdit from "../WidgetParts/TextEdit.vue";
export default {
  components: { TextEdit },
  props: ["value"],
  methods: {
    removeVideo(video) {
      this.value.videos = this.value.videos.filter(
        x => x.link !== video.link && x.title !== video.title
      );
    },
    addVideo() {
      this.value.videos.push({ title: "", link: "" });
    }
  }
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
</style>
