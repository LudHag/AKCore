<template>
  <div :class="fullwidth ? 'col-sm-12' : 'col-sm-6'">
    <div class="picture-select">
      <img class="selected-image" :src="value" />
      <a
        class="btn btn-default choose-picture-btn"
        @click.prevent="showModal = true"
        >Välj bild</a
      >
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
<script>
import ImagePickerModal from "../../ImagePickerModal.vue";
export default {
  components: { ImagePickerModal },
  props: ["value", "fullwidth"],
  data() {
    return {
      showModal: false
    };
  },
  methods: {
    selectImage(image) {
      this.$emit("input", "/media/" + image.name);
      this.showModal = false;
    }
  }
};
</script>
<style lang="scss"></style>
