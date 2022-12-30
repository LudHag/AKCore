<template>
  <div>
    <text-edit v-model="modelValue.text" :fullwidth="true"></text-edit>
  </div>
</template>
<script>
import TextEdit from "../WidgetParts/TextEdit.vue";
export default {
  components: { TextEdit },
  props: ["modelValue"],
};
</script>
<style lang="scss"></style>
