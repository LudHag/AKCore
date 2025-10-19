<template>
  <div>
    <div>
      <text-edit v-model="modelValue.text" :fullwidth="true"></text-edit>
    </div>
    <translation-edit
      :model-value="modelValue"
      @update:modelValue="emit('update:modelValue', $event)"
      :translate="translate"
    ></translation-edit>
  </div>
</template>
<script setup lang="ts">
import { WidgetEditModel } from "../models";
import TextEdit from "../WidgetParts/TextEdit.vue";
import TranslationEdit from "../WidgetParts/TranslationEdit.vue";

defineProps<{
  modelValue: WidgetEditModel;
  translate: boolean;
}>();
const emit = defineEmits<{
  (e: "update:modelValue", event: any): void;
}>();
</script>
<style lang="scss"></style>
