<template>
  <div class="line-graph-container">
    <Doughnut v-if="data" :data="data" :options="options" />
    <spinner v-if="loading" class="spinner-container" :size="'large'" />
  </div>
</template>
<script setup lang="ts">
import { RequestsResponse } from "./models";
import Spinner from "../Spinner.vue";
import { Chart, ArcElement, Tooltip, Legend, ChartOptions } from "chart.js";
import { Doughnut } from "vue-chartjs";
import { computed } from "vue";
const props = defineProps<{
  dataPoints: RequestsResponse;
  loading: boolean;
}>();

Chart.register(ArcElement, Tooltip, Legend);

const options: ChartOptions<"doughnut"> = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      labels: {
        color: "#fff",
      },
    },
  },
};

const data = computed(() => ({
  labels: ["Mobile", "Desktop"],
  datasets: [
    {
      backgroundColor: ["#b10000", "#00b100"],
      data: [
        props.dataPoints.items
          .flatMap((item) => item.items)
          .reduce((acc, item) => acc + item.mobile, 0),
        props.dataPoints.items
          .flatMap((item) => item.items)
          .reduce((acc, item) => acc + item.desktop, 0),
      ],
    },
  ],
}));
</script>
<style lang="scss" scoped>
.spinner-container {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
}
</style>
