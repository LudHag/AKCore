<template>
  <div>
    <p>Statistik</p>
    <Line class="line-graph" v-if="data" :data="data" :options="options" />
  </div>
</template>
<script setup lang="ts">
import { onMounted, ref, computed } from "vue";
import { getFromApi } from "../../services/apiservice";
import { RequestsResponse } from "./models";
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
  ChartOptions,
} from "chart.js";
import { Line } from "vue-chartjs";
import { getRandomColor } from "./utils";

ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
);

const dataPoints = ref<RequestsResponse | null>(null);

const data = computed(() =>
  dataPoints.value
    ? {
        labels: dataPoints.value.dates,
        datasets: dataPoints.value.items.map((item) => {
          return {
            label: item.path,
            backgroundColor: getRandomColor(item.path),
            borderColor: getRandomColor(item.path),
            data: item.items.map((i) => i.amount),
          };
        }),
      }
    : null,
);

const options: ChartOptions<"line"> = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      position: "bottom",
      labels: { color: "#fff" },
    },
  },
  scales: {
    x: {
      ticks: {
        color: "#fff",
      },
      grid: {
        color: "#6d6d6d",
      },
    },
    y: {
      ticks: {
        color: "#fff",
      },
      grid: {
        color: "#6d6d6d",
      },
    },
  },
};

onMounted(() => {
  getFromApi<RequestsResponse>(window.location.href + "/model").then((res) => {
    dataPoints.value = res;
  });
});
</script>
<style lang="scss" scoped>
.line-graph {
  max-height: 50vh;
}
</style>
