<template>
  <div>
    <p>Statistik</p>
    <div class="checkbox">
      <label>
        <input
          name="loggedIn"
          class="logged"
          v-model="loggedIn"
          type="checkbox"
        />
        Inloggade
      </label>

      <label>
        <input
          name="loggedOut"
          class="logged"
          v-model="loggedOut"
          type="checkbox"
        />
        Utloggade
      </label>
    </div>
    <div class="checkbox"></div>
    <Line class="line-graph" v-if="data" :data="data" :options="options" />
  </div>
</template>
<script setup lang="ts">
import { onMounted, ref, computed, watch } from "vue";
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
const loggedIn = ref<boolean>(true);
const loggedOut = ref<boolean>(true);

watch(loggedIn, () => {
  reloadData();
});

watch(loggedOut, () => {
  reloadData();
});

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

const reloadData = () => {
  getFromApi<RequestsResponse>(
    window.location.href +
      `/model?loggedIn=${loggedIn.value}&loggedOut=${loggedOut.value}`,
  ).then((res) => {
    dataPoints.value = res;
  });
};

onMounted(() => {
  reloadData();
});
</script>
<style lang="scss" scoped>
.line-graph {
  max-height: 50vh;
}

.checkbox {
  display: flex;
  gap: 10px;
}
</style>
