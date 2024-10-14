<template>
  <div class="line-graph-container" :class="{ overlay: loading }">
    <Line class="line-graph" v-if="data" :data="data" :options="options" />
    <spinner v-if="loading" class="spinner-container" :size="'large'" />
  </div>
</template>
<script setup lang="ts">
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
import { computed } from "vue";
import Spinner from "../Spinner.vue";

const { dataPoints } = defineProps<{
  dataPoints: RequestsResponse;
  loading: boolean;
}>();

ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
);

const data = computed(() =>
  dataPoints
    ? {
        labels: dataPoints.dates,
        datasets: dataPoints.items.map((item) => {
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
  elements: {
    line: {
      cubicInterpolationMode: "monotone", // Enables cubic curves
    },
  },
  plugins: {
    legend: {
      position: "bottom",
      labels: { color: "#fff" },
      onClick: function (e, legendItem, legend) {
        const index = legendItem.datasetIndex;
        const ci = legend.chart;

        if (index === undefined) {
          return;
        }

        const someHidden = ci.data.datasets.some((dataset) => dataset.hidden);

        if (someHidden) {
          ci.data.datasets.forEach((dataset) => {
            dataset.hidden = false;
          });
        } else {
          ci.data.datasets.forEach((dataset, i) => {
            if (i === index) {
              dataset.hidden = false;
            } else {
              dataset.hidden = true;
            }
          });
        }
        ci.update();
      },
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
</script>
<style lang="scss" scoped>
.line-graph-container {
  position: relative;
  height: 50vh;
  &.overlay::after {
    content: "";
    display: block;
    width: 100%;
    height: 100%;
    background-color: #000;
    opacity: 0.5;
    position: absolute;
    top: 0;
  }
}
.spinner-container {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
}
</style>
