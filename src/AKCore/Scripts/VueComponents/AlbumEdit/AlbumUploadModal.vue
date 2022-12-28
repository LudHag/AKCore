<template>
  <modal :show-modal="album" :header="header" @close="close">
    <template v-slot:body>
      <div class="modal-body" v-if="album">
        <file-uploader :button-text="'Ladda upp spår'" @upload="uploadFiles">
          <template v-slot:content>
            <div class="tracks">
              <div
                class="alert alert-danger"
                ref="error"
                style="display: none"
              ></div>
              <album-upload-track-item
                :track="track"
                v-for="track in orderedTracks"
                :key="track.id"
                @update="update"
                @remove="removeTrack"
              ></album-upload-track-item>
            </div>
          </template>
        </file-uploader>
      </div>
    </template>
  </modal>
</template>
<script>
import Modal from "../Modal.vue";
import FileUploader from "../FileUploader.vue";
import AlbumUploadTrackItem from "./AlbumUploadTrackItem.vue";
import ApiService from "../../services/apiservice";

export default {
  components: {
    Modal,
    FileUploader,
    AlbumUploadTrackItem,
  },
  props: ["album"],
  computed: {
    header() {
      return this.album ? this.album.name : "";
    },
    orderedTracks() {
      if (!this.album || !this.album.tracks) {
        return [];
      }
      return this.album.tracks.slice().sort((a, b) => {
        if (a.number < b.number) return -1;
        if (a.number > b.number) return 1;
        return 0;
      });
    },
  },
  methods: {
    close() {
      this.$emit("close");
    },
    uploadFiles(files) {
      const mediaData = new FormData();
      for (var i = 0; i < files.length; i++) {
        mediaData.append("TrackFiles", files[i]);
      }
      mediaData.append("AlbumId", this.album.id);
      const error = $(this.$refs.error);
      ApiService.postFormData(
        "/AlbumEdit/UploadTracks/",
        mediaData,
        error,
        null,
        () => {
          this.$emit("update");
        }
      );
    },
    removeTrack(id) {
      const error = $(this.$refs.error);
      ApiService.postByObjectAsForm(
        "/AlbumEdit/DeleteTrack",
        { id, album: this.album.id },
        error,
        null,
        () => {
          this.$emit("update");
        }
      );
    },
    update() {
      this.$emit("update");
    },
  },
};
</script>
<style lang="scss" scoped>
.tracks {
  width: 80%;
  text-align: left;
  margin: auto;
}
</style>
