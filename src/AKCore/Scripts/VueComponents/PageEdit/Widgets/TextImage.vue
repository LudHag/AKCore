<template>
  <div>
    <div>
      <text-edit v-model="modelValue.text"></text-edit>
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
      <div class="col-sm-6">
        <div class="form-group">
          <label for="image-alignment">Bild placering:</label>
          <select
            id="image-alignment"
            class="form-control"
            :value="modelValue.alignment || 'right'"
            @change="
              emit('update:modelValue', {
                ...modelValue,
                alignment: ($event.target as HTMLSelectElement).value,
              })
            "
          >
            <option value="right">Höger (standard)</option>
            <option value="left">Vänster</option>
          </select>
        </div>
      </div>
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
import ImageEdit from "../WidgetParts/ImageEdit.vue";
import TextEdit from "../WidgetParts/TextEdit.vue";
import TranslationEdit from "../WidgetParts/TranslationEdit.vue";

defineProps<{
  modelValue: WidgetEditModel;
  translate: boolean;
}>();

const emit = defineEmits<{
  (e: "update:modelValue", event: WidgetEditModel): void;
}>();
</script>
<style lang="scss" scoped>
.form-group {
  margin-bottom: 15px;

  label {
    display: block;
    margin-bottom: 5px;
    font-weight: 500;
  }

  select.form-control {
    width: 100%;
  }
}
</style>
