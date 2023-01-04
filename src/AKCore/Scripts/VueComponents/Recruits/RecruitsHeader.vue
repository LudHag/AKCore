<template>
  <div class="row filters">
    <div class="col-lg-5">
      <h1>Intresseanmälningar</h1>
      <a @click="$emit('export')" class="btn btn-primary"> Exportera </a>
    </div>
    <div class="col-lg-7">
      <div class="form-inline interest-filters">
        <div class="form-group">
          <input
            class="toggle-switch"
            id="archive-flip"
            type="checkbox"
            v-model="localArchived"
            @change="$emit('archivechange', localArchived)"
          />
          <label
            class="toggle-switch-btn"
            for="archive-flip"
            data-tg-off="Aktiva"
            data-tg-on="Arkiverade"
          ></label>
        </div>
        <div class="form-group">
          <input
            type="text"
            class="form-control"
            placeholder="Sök här"
            v-model="localSearchText"
            @keyup="$emit('searchchange', localSearchText)"
          />
        </div>
        <div class="form-group">
          <select class="form-control" @change="instrumentChange($event)">
            <option value="">Sök efter instrument</option>
            <option v-for="instr in instruments" :key="instr">
              {{ instr }}
            </option>
          </select>
        </div>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { onMounted, ref } from "vue";
import Constants from "../../constants";

const emit = defineEmits<{
  (e: "export"): void;
  (e: "archivechange", value: boolean): void;
  (e: "searchchange", value: string): void;
  (e: "instrumentchange", value: string): void;
}>();

const { archived } = defineProps<{
  archived: boolean;
}>();

const localSearchText = ref("");
const localArchived = ref(false);

const instruments = Constants.INSTRUMENTS;

const instrumentChange = (event: Event) => {
  const target = event.target as HTMLSelectElement;
  emit("instrumentchange", target.value);
};

onMounted(() => {
  localArchived.value = archived;
});
</script>
<style lang="scss" scoped></style>
