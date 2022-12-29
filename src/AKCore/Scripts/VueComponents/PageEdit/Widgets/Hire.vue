<template>
  <div>
    <text-edit
      :modelValue="modelValue.text"
      @update:modelValue="update($event)"
      :fullwidth="true"
    ></text-edit>
  </div>
</template>
<script>
import TextEdit from "../WidgetParts/TextEdit.vue";
export default {
  components: { TextEdit },
  props: ["modelValue"],
  methods: {
    update(event) {
      this.$emit("update:modelValue", { ...this.modelValue, text: event });
    },
  },
};
</script>
<style lang="scss"></style>
