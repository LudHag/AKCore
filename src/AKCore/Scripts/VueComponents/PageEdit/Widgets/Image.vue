<template>
  <div>
    <image-edit v-model="modelValue.image"></image-edit>
  </div>
</template>
<script setup lang="ts">
import { WidgetEditModel } from "../models";
import ImageEdit from "../WidgetParts/ImageEdit.vue";
defineProps<{
  modelValue: WidgetEditModel;
}>();
</script>
<style lang="scss"></style>
