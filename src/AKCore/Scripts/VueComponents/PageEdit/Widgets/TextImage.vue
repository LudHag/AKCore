<template>
  <div>
    <text-edit v-model="value.text"></text-edit>
    <image-edit v-model="value.image"></image-edit>
  </div>
</template>
<script>
import ImageEdit from "../WidgetParts/ImageEdit.vue";
import TextEdit from "../WidgetParts/TextEdit.vue";
export default {
  components: { ImageEdit, TextEdit },
  props: ["value"]
};
</script>
<style lang="scss"></style>
