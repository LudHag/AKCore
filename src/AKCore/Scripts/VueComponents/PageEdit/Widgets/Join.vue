<template>
  <div>
    <text-edit v-model="modelValue.text" :fullwidth="true"></text-edit>
  </div>
</template>
<script setup lang="ts">
import { WidgetEditModel } from "../models";
import TextEdit from "../WidgetParts/TextEdit.vue";
defineProps<{
  modelValue: WidgetEditModel;
}>();
</script>
<style lang="scss"></style>
