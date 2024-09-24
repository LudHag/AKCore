<template>
  <div>
    <text-edit fullwidth v-model="modelValue.text"></text-edit>
    <div class="col-sm-12">
      <label>Inställt datum </label>
      <input class="form-control" type="date" v-model="modelValue.targetDate" name="Date" />
      <br>
      <label>Inställd tid </label>
      <input class="form-control" type="time" v-model="modelValue.targetTime" name="Time" />
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
