<template>
  <div>
    <image-edit
      v-model="modelValue.image"
      :alt-text="modelValue.imageAlt"
      @update:altText="
        emit('update:modelValue', {
          ...modelValue,
          imageAlt: $event,
        })
      "
    ></image-edit>
  </div>
</template>
<script setup lang="ts">
import { WidgetEditModel } from "../models";
import ImageEdit from "../WidgetParts/ImageEdit.vue";
defineProps<{
  modelValue: WidgetEditModel;
}>();

const emit = defineEmits<{
  (e: "update:modelValue", event: WidgetEditModel): void;
}>();
</script>
<style lang="scss"></style>
