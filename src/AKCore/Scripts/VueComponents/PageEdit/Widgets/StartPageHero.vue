<template>
  <div>
    <div>
      <div class="row">
        <div class="col-sm-12">
          <div class="form-group">
            <label for="hero-header">Rubrik:</label>
            <input
              id="hero-header"
              type="text"
              class="form-control"
              :value="modelValue.targetDate || ''"
              @input="
                emit('update:modelValue', {
                  ...modelValue,
                  targetDate: ($event.target as HTMLInputElement).value,
                })
              "
              placeholder="T.ex. Välkommen till Alte Kamereren"
            />
          </div>
        </div>
      </div>
      <image-edit
        v-model="modelValue.image"
        :alt-text="'Hero background image'"
        @update:altText="() => {}"
      ></image-edit>
      <div class="row">
        <div class="col-sm-6">
          <div class="form-group">
            <label for="button1-text">Knapp 1 - Text:</label>
            <input
              id="button1-text"
              type="text"
              class="form-control"
              :value="modelValue.text || ''"
              @input="
                emit('update:modelValue', {
                  ...modelValue,
                  text: ($event.target as HTMLInputElement).value,
                })
              "
              placeholder="T.ex. Kommande spel"
            />
          </div>
        </div>
        <div class="col-sm-6">
          <div class="form-group">
            <label for="button1-link">Knapp 1 - Länk:</label>
            <input
              id="button1-link"
              type="text"
              class="form-control"
              :value="modelValue.imageAlt || ''"
              @input="
                emit('update:modelValue', {
                  ...modelValue,
                  imageAlt: ($event.target as HTMLInputElement).value,
                })
              "
              placeholder="/upcoming"
            />
          </div>
        </div>
      </div>
      <div class="row">
        <div class="col-sm-6">
          <div class="form-group">
            <label for="button2-text">Knapp 2 - Text:</label>
            <input
              id="button2-text"
              type="text"
              class="form-control"
              :value="modelValue.textEng || ''"
              @input="
                emit('update:modelValue', {
                  ...modelValue,
                  textEng: ($event.target as HTMLInputElement).value,
                })
              "
              placeholder="T.ex. Gå med"
            />
          </div>
        </div>
        <div class="col-sm-6">
          <div class="form-group">
            <label for="button2-link">Knapp 2 - Länk:</label>
            <input
              id="button2-link"
              type="text"
              class="form-control"
              :value="modelValue.alignment || ''"
              @input="
                emit('update:modelValue', {
                  ...modelValue,
                  alignment: ($event.target as HTMLInputElement).value,
                })
              "
              placeholder="/join"
            />
          </div>
        </div>
      </div>
    </div>
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
<style lang="scss" scoped>
.form-group {
  margin-bottom: 15px;

  label {
    display: block;
    margin-bottom: 5px;
    font-weight: 500;
  }

  input.form-control {
    width: 100%;
  }
}
</style>
