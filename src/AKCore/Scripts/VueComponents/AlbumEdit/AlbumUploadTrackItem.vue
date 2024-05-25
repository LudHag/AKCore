<template>
  <div class="track">
    <span class="name" v-if="!showEditName" @click="nameClick">{{ name }}</span>
    <input
      class="name-input"
      ref="inputelement"
      v-if="showEditName"
      v-model="editName"
      @keyup.enter="onInputBlur"
      @blur="onInputBlur"
    />
    <a class="rem-track" href="#" @click.prevent="remove">x</a>
  </div>
</template>
<script setup lang="ts">
import { computed, nextTick, onMounted, ref } from "vue";
import { postToApi } from "../../services/apiservice";
import { TrackEditModel } from "./models";

const emit = defineEmits<{
  (e: "remove", id: number): void;
  (e: "update"): void;
}>();

const props = defineProps<{
  track: TrackEditModel;
}>();

const showEditName = ref(false);
const editName = ref("");
const inputelement = ref<HTMLInputElement | null>(null);

const name = computed(() => {
  if (props.track.name) {
    return props.track.name;
  }
  const nameParts = props.track.fileName.split(".");
  return nameParts[nameParts.length - 2].replace(/_/g, " ");
});

const remove = () => {
  emit("remove", props.track.id);
};

const nameClick = () => {
  showEditName.value = true;
  nextTick(() => inputelement.value!.focus());
};

const onInputBlur = () => {
  showEditName.value = false;
  postToApi(
    "/AlbumEdit/ChangeTrackName",
    { id: props.track.id, name: editName.value },
    null,
    null,
    () => {
      emit("update");
    }
  );
};

onMounted(() => {
  editName.value = name.value;
});
</script>
<style lang="scss" scoped>
.track {
  .rem-track {
    float: right;
  }
}
</style>
