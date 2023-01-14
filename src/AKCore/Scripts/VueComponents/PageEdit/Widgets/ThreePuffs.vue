<template>
  <div>
    <text-edit v-model="modelValue.text"></text-edit>
    <image-edit v-model="modelValue.image"></image-edit>
  </div>
</template>
<script setup lang="ts">
import { WidgetEditModel } from "../models";
import ImageEdit from "../WidgetParts/ImageEdit.vue";
import TextEdit from "../WidgetParts/TextEdit.vue";

defineProps<{
  modelValue: WidgetEditModel;
}>();
</script>
<style lang="scss"></style>
