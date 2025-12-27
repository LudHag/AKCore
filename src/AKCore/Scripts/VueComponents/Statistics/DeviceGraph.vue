<template>
  <div class="line-graph-container">
    <Doughnut v-if="data" :data="data" :options="options" />
  </div>
</template>
<script setup lang="ts">
import { RequestsResponse } from "./models";
import { Chart, ArcElement, Tooltip, Legend, ChartOptions } from "chart.js";
import { Doughnut } from "vue-chartjs";
import { computed } from "vue";
const props = defineProps<{
  dataPoints: RequestsResponse;
}>();

Chart.register(ArcElement, Tooltip, Legend);

const data = computed(() => {
  const mobile = props.dataPoints.items
    .flatMap((item) => item.items)
    .reduce((acc, item) => acc + item.mobile, 0);
  const desktop = props.dataPoints.items
    .flatMap((item) => item.items)
    .reduce((acc, item) => acc + item.desktop, 0);

  const mobilePercentage = ((mobile / (mobile + desktop)) * 100).toFixed(0);
  const desktopPercentage = ((desktop / (mobile + desktop)) * 100).toFixed(0);

  return {
    labels: [
      `Mobile (${mobilePercentage}%)`,
      `Desktop (${desktopPercentage}%)`,
    ],
    datasets: [
      {
        backgroundColor: ["#b10000", "#00b100"],
        data: [mobile, desktop],
      },
    ],
  };
});

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
</script>
