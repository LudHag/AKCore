<template>
  <div>
    <h2>Funktionsanvändning</h2>
    <div class="controls">
      <div class="range-control">
        <a
          href="#"
          class="range-toggle"
          @click.prevent="usageRange = 'day'"
          :class="{ active: usageRange === 'day' }"
        >
          Idag
        </a>
        <a
          href="#"
          class="range-toggle"
          @click.prevent="usageRange = 'week'"
          :class="{ active: usageRange === 'week' }"
        >
          Senaste veckan
        </a>
        <a
          href="#"
          class="range-toggle"
          @click.prevent="usageRange = 'month'"
          :class="{ active: usageRange === 'month' }"
        >
          Senaste månaden
        </a>
      </div>
    </div>
    <div class="graphs-row" v-if="usageData">
      <UsageGraph :data-points="usageData" :loading="loadingUsage" />
    </div>
  </div>
</template>
<script setup lang="ts">
import { onMounted, ref, watch } from "vue";
import { getFromApi } from "@services/apiservice";
import UsageGraph from "./UsageGraph.vue";
import { RequestsRange, UsageResponse } from "./models";

const usageData = ref<UsageResponse | null>(null);
const usageRange = ref<RequestsRange>("week");
const loadingUsage = ref<boolean>(false);

watch([usageRange], () => {
  loadUsageData();
});

const loadUsageData = () => {
  loadingUsage.value = true;
  getFromApi<UsageResponse>(
    window.location.href + `/FeatureUsage?range=${usageRange.value}`,
  )
    .then((res) => {
      usageData.value = res;
    })
    .finally(() => {
      loadingUsage.value = false;
    });
};

onMounted(() => {
  loadUsageData();
});
</script>
<style lang="scss" scoped>
.controls {
  display: flex;
  align-items: center;
  justify-content: space-between;
}
.graphs-row {
  display: flex;
  gap: 10px;

  @media (max-width: 1225px) {
    flex-direction: column;
  }
}

.range-control {
  margin-top: 20px;
  margin-bottom: 20px;
  .range-toggle {
    padding: 4px 10px;
    color: #000;
    display: inline-block;
    background-color: #808080;
    border-right: 1px solid #575656;

    &:first-of-type {
      border-radius: 7px 0 0 7px;
    }

    &:last-of-type {
      border-radius: 0 7px 7px 0;
    }

    &.active {
      background-color: #5f5f5f;
    }
  }
}
</style>
