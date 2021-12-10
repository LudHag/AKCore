<template>
  <div>
    <text-edit v-model="value.text" :fullwidth="true"></text-edit>
  </div>
</template>
<script>
import TextEdit from "../WidgetParts/TextEdit.vue";
export default {
  components: { TextEdit },
  props: ["value"]
};
</script>
<style lang="scss"></style>
