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
    <div class="form-group" v-if="modelValue">
      <label for="imageAlt">Alt text för bilden:</label>
      <input
        type="text"
        class="form-control"
        :value="altText"
        @input="
          emit(
            'update:altText',
            ($event.target as HTMLInputElement).value ?? '',
          )
        "
        placeholder="Beskrivande text för bilden"
      />
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
  (e: "update:altText", value: string): void;
}>();

defineProps<{
  modelValue?: string;
  altText?: string;
  fullwidth?: boolean;
}>();

const showModal = ref(false);

const selectImage = (image: Image) => {
  emit("update:modelValue", "/media/" + image.name);
  showModal.value = false;
};
</script>
<style lang="scss" scoped>
.choose-picture-btn {
  margin-top: 20px;
  display: block;
  width: 100px;
  margin-left: auto;
  margin-right: auto;
}

.picture-select {
  text-align: center;
  img {
    width: 188px;
    height: 188px;
  }
}
</style>
