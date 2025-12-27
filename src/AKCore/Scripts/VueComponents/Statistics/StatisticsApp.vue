<template>
  <div>
    <h2>Sidladdningar</h2>
    <div class="controls">
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
      <div class="range-control hidden-xs">
        <a
          href="#"
          class="range-toggle"
          @click.prevent="range = 'day'"
          :class="{ active: range === 'day' }"
        >
          Idag
        </a>
        <a
          href="#"
          class="range-toggle"
          @click.prevent="range = 'week'"
          :class="{ active: range === 'week' }"
        >
          Senaste veckan
        </a>
        <a
          href="#"
          class="range-toggle"
          @click.prevent="range = 'month'"
          :class="{ active: range === 'month' }"
        >
          Senaste månaden
        </a>
      </div>
    </div>
    <div class="graphs-row" v-if="dataPoints">
      <PageViewGraph :data-points="dataPoints" :loading="loading" />
      <DeviceGraph :data-points="dataPoints" />
    </div>
  </div>
</template>
<script setup lang="ts">
import { onMounted, ref, watch } from "vue";
import { getFromApi } from "../../services/apiservice";
import PageViewGraph from "./PageViewGraph.vue";
import DeviceGraph from "./DeviceGraph.vue";
import { RequestsRange, RequestsResponse } from "./models";

const dataPoints = ref<RequestsResponse | null>(null);
const loggedIn = ref<boolean>(true);
const loggedOut = ref<boolean>(true);
const range = ref<RequestsRange>("day");
const loading = ref<boolean>(false);

watch(loggedIn, () => {
  reloadData();
});

watch(loggedOut, () => {
  reloadData();
});

watch(range, () => {
  reloadData();
});

const reloadData = () => {
  loading.value = true;
  getFromApi<RequestsResponse>(
    window.location.href +
      `/model?loggedIn=${loggedIn.value}&loggedOut=${loggedOut.value}&range=${range.value}`,
  )
    .then((res) => {
      dataPoints.value = res;
    })
    .finally(() => {
      loading.value = false;
    });
};

onMounted(() => {
  reloadData();
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

.checkbox {
  display: flex;
  gap: 10px;
}

.range-control {
  margin-top: 20px;
  margin-bottom: 20px;
  .range-toggle {
    padding: 4px 10px;
    color: #000;
    display: inline-block;
    background-color: #808080;

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
