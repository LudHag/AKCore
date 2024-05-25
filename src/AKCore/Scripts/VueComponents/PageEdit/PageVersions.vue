<template>
  <div
    class="col-md-3 revisions hidden-xs"
    v-if="modelValue.revisions && modelValue.revisions.length"
  >
    <br class="visible-sm" />
    <h2>Versioner</h2>
    <a
      class="revision"
      href="#"
      @click.prevent="selectRevision(revision)"
      :class="{
        selected: selectedRevision && revision.id === selectedRevision.id
      }"
      v-for="revision in modelValue.revisions"
      :key="revision.id"
    >
      {{ revision.modified }} -
      {{ revision.modifiedBy }}
    </a>
    <a
      v-if="selectedRevision"
      class="revision"
      href="#"
      @click.prevent="selectRevision(null)"
    >
      Nuvarande
    </a>
  </div>
</template>
<script setup lang="ts">
import { PageEditModel, PageRevisionEditModel } from "./models";

const emit = defineEmits<{
  (e: "select", revision: PageRevisionEditModel | null): void;
}>();

defineProps<{
  modelValue: PageEditModel;
  selectedRevision: PageRevisionEditModel | null;
}>();

const selectRevision = (revision: PageRevisionEditModel | null) => {
  emit("select", revision);
};
</script>
<style lang="scss" scoped>
@import "../../../Styles/variables.scss";
.revisions {
  h2 {
    font-weight: 500;
    margin-top: 0;
    color: $akwhite;
    font-size: 15px;
    line-height: 15px;
    margin-bottom: 15px;
  }

  .revision {
    font-size: 12px;
    display: block;
    &.selected {
      text-decoration: underline;
    }
  }
}
</style>
