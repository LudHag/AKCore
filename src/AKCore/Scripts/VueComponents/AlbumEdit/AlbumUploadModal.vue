<template>
    <modal :show-modal="album" :header="header" @close="close">
        <div slot="body" class="modal-body" v-if="album">
          <file-uploader :button-text="'Ladda upp spår'">
            <div slot="content" class="tracks">
              <album-upload-track-item :track="track" v-for="track in orderedTracks" :key="track.id"></album-upload-track-item>
            </div>
          </file-uploader>
        </div>
    </modal>
</template>
<script>
import Modal from "../Modal";
import FileUploader from "../FileUploader";
import AlbumUploadTrackItem from "./AlbumUploadTrackItem";

export default {
  components: {
    Modal,
    FileUploader,
    AlbumUploadTrackItem
  },
  props: ["album"],
  computed: {
    header() {
      return this.album ? this.album.name : "";
    },
    orderedTracks() {
      if(!this.album || !this.album.tracks) {
        return [];
      }
      return this.album.tracks.slice().sort((a, b) => {
         if (a.number < b.number)
          return -1;
        if (a.number > b.number)
          return 1;
        return 0;
      });
    }
  },
  methods: {
    close() {
      this.$emit("close");
    }
  }
};
</script>
<style lang="scss" scoped>
  .tracks{
    width: 100%;
    text-align: left;
  }
</style>