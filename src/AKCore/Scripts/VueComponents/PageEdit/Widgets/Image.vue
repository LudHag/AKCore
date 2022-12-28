<template>
  <div>
    <image-edit v-model="modelValue.image"></image-edit>
  </div>
</template>
<script>
import ImageEdit from "../WidgetParts/ImageEdit.vue";
export default {
  components: { ImageEdit },
  props: ["modelValue"],
};
</script>
<style lang="scss"></style>
