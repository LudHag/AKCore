<template>
  <div>
    <text-edit v-model="modelValue.text"></text-edit>
    <image-edit v-model="modelValue.image"></image-edit>
  </div>
</template>
<script>
import ImageEdit from "../WidgetParts/ImageEdit.vue";
import TextEdit from "../WidgetParts/TextEdit.vue";
export default {
  components: { ImageEdit, TextEdit },
  props: ["modelValue"],
};
</script>
<style lang="scss"></style>
