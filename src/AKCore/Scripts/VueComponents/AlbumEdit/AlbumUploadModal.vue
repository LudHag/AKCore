<template>
  <modal :show-modal="!!album" :header="header" @close="close">
    <template #body>
      <div class="modal-body" v-if="album">
        <file-uploader :button-text="'Ladda upp spår'" @upload="uploadFiles">
          <template #content>
            <div class="tracks">
              <div
                class="alert alert-danger"
                ref="errorElement"
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
<script setup lang="ts">
import Modal from "../Modal.vue";
import FileUploader from "../FileUploader.vue";
import AlbumUploadTrackItem from "./AlbumUploadTrackItem.vue";
import { postByObjectAsForm, postFormData } from "../../services/apiservice";
import { AlbumEditModel } from "./models";
import { computed, ref } from "vue";

const emit = defineEmits<{
  (e: "close"): void;
  (e: "update"): void;
}>();

const props = defineProps<{
  album: AlbumEditModel | null;
}>();

const errorElement = ref<HTMLElement | null>(null);

const header = computed(() => (props.album ? props.album.name : ""));

const orderedTracks = computed(() => {
  if (!props.album || !props.album.tracks) {
    return [];
  }
  return props.album.tracks.slice().sort((a, b) => {
    if (a.number < b.number) return -1;
    if (a.number > b.number) return 1;
    return 0;
  });
});

const close = () => {
  emit("close");
};

const uploadFiles = (files: FileList) => {
  const mediaData = new FormData();
  for (let i = 0; i < files.length; i++) {
    mediaData.append("TrackFiles", files[i]);
  }
  const albumId = props.album?.id!;
  mediaData.append("AlbumId", albumId.toString());
  const error = $(errorElement.value!);
  postFormData("/AlbumEdit/UploadTracks/", mediaData, error, null, () => {
    emit("update");
  });
};

const removeTrack = (id: number) => {
  const error = $(errorElement.value!);
  postByObjectAsForm(
    "/AlbumEdit/DeleteTrack",
    { id, album: props.album?.id },
    error,
    null,
    () => {
      emit("update");
    }
  );
};

const update = () => {
  emit("update");
};
</script>
<style lang="scss" scoped>
.tracks {
  width: 80%;
  text-align: left;
  margin: auto;
}
</style>
