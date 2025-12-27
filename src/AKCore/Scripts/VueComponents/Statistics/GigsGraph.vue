<template>
  <div class="bar-graph-container">
    <Bar class="bar-graph" :options="options" :data="chartData" />
  </div>
</template>
<script setup lang="ts">
import { GigItem } from "./models";
import { Bar } from "vue-chartjs";
import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  BarElement,
  CategoryScale,
  LinearScale,
  ChartOptions,
} from "chart.js";
import { computed } from "vue";

const props = defineProps<{
  dataPoints: GigItem[];
}>();

ChartJS.register(
  Title,
  Tooltip,
  Legend,
  BarElement,
  CategoryScale,
  LinearScale,
);

const chartData = computed(() => {
  return {
    labels: props.dataPoints.map((item) => item.name),
    datasets: [
      {
        label: "Can Come",
        data: props.dataPoints.map((item) => item.canCome),
        backgroundColor: "#00b100",
      },
      {
        label: "Cant Come",
        data: props.dataPoints.map((item) => item.cantCome),
        backgroundColor: "#b10000",
      },
    ],
  };
});

const options: ChartOptions<"bar"> = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      labels: {
        color: "#fff",
      },
    },
  },
  scales: {
    x: {
      ticks: {
        color: "#fff",
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
</script>
<style lang="scss" scoped>
.bar-graph-container {
  height: 300px;
  flex-grow: 1;
  position: relative;
}
.bar-graph {
  height: 100%;
}
</style>
