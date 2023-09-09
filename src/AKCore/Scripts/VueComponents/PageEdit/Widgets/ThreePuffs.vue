<template>
  <div>
    <div>
      <text-edit v-model="modelValue.text"></text-edit>
      <image-edit v-model="modelValue.image"></image-edit>
    </div>
    <translation-edit
      :model-value="modelValue"
      @update:modelValue="$emit('update:modelValue', $event)"
      :translate="translate"
    ></translation-edit>
  </div>
</template>
<script setup lang="ts">
import { WidgetEditModel } from "../models";
import ImageEdit from "../WidgetParts/ImageEdit.vue";
import TextEdit from "../WidgetParts/TextEdit.vue";
import TranslationEdit from "../WidgetParts/TranslationEdit.vue";

defineProps<{
  modelValue: WidgetEditModel;
  translate: boolean;
}>();
</script>
<style lang="scss"></style>
