<template>
  <div :class="fullwidth ? 'col-sm-12' : 'col-sm-6'">
    <div class="picture-select">
      <img class="selected-image" :src="modelValue" />
      <a
        class="btn btn-default choose-picture-btn"
        @click.prevent="showModal = true"
      >
        Välj bild
      </a>
    </div>
    <image-picker-modal
      v-if="showModal"
      :show-modal="showModal"
      :notransition="true"
      @close="showModal = false"
      @image="selectImage"
    ></image-picker-modal>
  </div>
</template>
<script setup lang="ts">
import { ref } from "vue";
import ImagePickerModal from "../../ImagePickerModal.vue";
import { Image } from "../../models";

const emit = defineEmits<{
  (e: "update:modelValue", value: string): void;
}>();

defineProps<{
  modelValue?: string;
  fullwidth?: boolean;
}>();

const showModal = ref(false);

const selectImage = (image: Image) => {
  emit("update:modelValue", "/media/" + image.name);
  showModal.value = false;
};
</script>
<style lang="scss"></style>
