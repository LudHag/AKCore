<template>
  <div>
    <image-edit v-model="value.image"></image-edit>
  </div>
</template>
<script>
import ImageEdit from "../WidgetParts/ImageEdit.vue";
export default {
  components: { ImageEdit },
  props: ["value"]
};
</script>
<style lang="scss"></style>
