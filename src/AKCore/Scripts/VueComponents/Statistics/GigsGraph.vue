<template>
  <div class="bar-graph-container">
    <Bar
      ref="chartRef"
      class="bar-graph"
      :options="options"
      :data="chartData"
      @click="handleChartClick"
    />
  </div>
</template>
<script setup lang="ts">
import { GigItem } from "./models";
import { Bar } from "vue-chartjs";
import type { ChartComponentRef } from "vue-chartjs";
import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  BarElement,
  CategoryScale,
  LinearScale,
  ChartOptions,
  InteractionItem,
} from "chart.js";
import { computed, useTemplateRef } from "vue";

const props = defineProps<{
  dataPoints: GigItem[];
}>();

const chartRef = useTemplateRef<ChartComponentRef<"bar">>("chartRef");

ChartJS.register(
  Title,
  Tooltip,
  Legend,
  BarElement,
  CategoryScale,
  LinearScale,
);

const filteredData = computed(() =>
  props.dataPoints.filter((item) => item.canCome > 0 || item.cantCome > 0),
);

const handleChartClick = (event: any) => {
  if (!chartRef.value || !chartRef.value.chart) return;
  const chart = chartRef.value?.chart;
  const interactionItems: InteractionItem[] = chart.getElementsAtEventForMode(
    event,
    "nearest",
    { intersect: true },
    true,
  );

  if (interactionItems.length > 0) {
    const dataIndex = interactionItems[0].index;
    const gig = filteredData.value[dataIndex];
    const gigId = gig.id;

    window.location.href = `/upcoming/Event/${gigId}`;
  }
};

const chartData = computed(() => {
  return {
    labels: filteredData.value.map((item) => item.name),
    datasets: [
      {
        label: "Kan inte komma",
        data: filteredData.value.map((item) => item.cantCome),
        backgroundColor: "#b10000",
      },
      {
        label: "Kan komma",
        data: filteredData.value.map((item) => item.canCome),
        backgroundColor: "#00b100",
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
      onClick: (_e, legendItem, legend) => {
        // Toggle datasets when clicking legend
        const index = legendItem.datasetIndex ?? 0;
        const ci = legend.chart;
        if (ci.isDatasetVisible(index)) {
          ci.hide(index);
        } else {
          ci.show(index);
        }
      },
    },
    tooltip: {
      callbacks: {
        title: (tooltipItems) => {
          const dataIndex = tooltipItems[0].dataIndex;
          const gigItem = filteredData.value[dataIndex];
          return `${gigItem.name} - ${gigItem.day.split("T")[0]}`;
        },
      },
    },
  },
  scales: {
    x: {
      stacked: true,
      ticks: {
        color: "#fff",
        minRotation: 70,
        callback: function (_value, index) {
          const gigItem = filteredData.value[index];
          return gigItem ? gigItem.name : "";
        },
      },
    },
    y: {
      stacked: true,
      ticks: {
        color: "#fff",
      },
      grid: {
        color: "#6d6d6d",
      },
    },
  },
  onHover: (event, activeElements) => {
    if (event.native?.target) {
      if (activeElements.length > 0) {
        (event.native.target as HTMLElement).style.cursor = "pointer";
      } else {
        (event.native.target as HTMLElement).style.cursor = "default";
      }
    }
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
