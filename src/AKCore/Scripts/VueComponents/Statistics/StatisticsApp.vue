<template>
  <div>
    <h2>Spelningar</h2>
    <div class="controls">
      <div class="checkbox">
        <div class="range-control">
          <a
            href="#"
            class="range-toggle"
            @click.prevent="gigsRange = 'Month'"
            :class="{ active: gigsRange === 'Month' }"
          >
            Senaste Månaden
          </a>
          <a
            href="#"
            class="range-toggle"
            @click.prevent="gigsRange = 'Year'"
            :class="{ active: gigsRange === 'Year' }"
          >
            Senaste Året
          </a>
        </div>
      </div>
    </div>
    <div class="graphs-row" v-if="gigs">
      <GigsGraph :data-points="gigs" />
    </div>
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
          @click.prevent="requestsRange = 'day'"
          :class="{ active: requestsRange === 'day' }"
        >
          Idag
        </a>
        <a
          href="#"
          class="range-toggle"
          @click.prevent="requestsRange = 'week'"
          :class="{ active: requestsRange === 'week' }"
        >
          Senaste veckan
        </a>
        <a
          href="#"
          class="range-toggle"
          @click.prevent="requestsRange = 'month'"
          :class="{ active: requestsRange === 'month' }"
        >
          Senaste månaden
        </a>
      </div>
    </div>
    <div class="graphs-row" v-if="dataPoints">
      <PageViewGraph :data-points="dataPoints" :loading="loadingRequests" />
      <DeviceGraph :data-points="dataPoints" />
    </div>
  </div>
</template>
<script setup lang="ts">
import { onMounted, ref, watch } from "vue";
import { getFromApi } from "../../services/apiservice";
import PageViewGraph from "./PageViewGraph.vue";
import DeviceGraph from "./DeviceGraph.vue";
import GigsGraph from "./GigsGraph.vue";

import {
  RequestsRange,
  RequestsResponse,
  GigsResponse,
  GigItem,
  GigsRange,
} from "./models";
const dataPoints = ref<RequestsResponse | null>(null);
const gigs = ref<GigItem[] | null>(null);
const loggedIn = ref<boolean>(true);
const loggedOut = ref<boolean>(true);
const requestsRange = ref<RequestsRange>("day");
const gigsRange = ref<GigsRange>("Year");
const loadingRequests = ref<boolean>(false);
const loadingGigs = ref<boolean>(false);

watch([loggedIn, loggedOut, requestsRange], () => {
  loadRequestData();
});

watch([gigsRange], () => {
  loadGigsData();
});

const loadRequestData = () => {
  loadingRequests.value = true;
  getFromApi<RequestsResponse>(
    window.location.href +
      `/SiteRequests?loggedIn=${loggedIn.value}&loggedOut=${loggedOut.value}&range=${requestsRange.value}`,
  )
    .then((res) => {
      dataPoints.value = res;
    })
    .finally(() => {
      loadingRequests.value = false;
    });
};
const loadGigsData = () => {
  loadingGigs.value = true;
  getFromApi<GigsResponse>(
    window.location.href + `/Gigs?range=${gigsRange.value}`,
  )
    .then((res) => {
      gigs.value = res.items;
    })
    .finally(() => {
      loadingGigs.value = false;
    });
};

onMounted(() => {
  loadRequestData();
  loadGigsData();
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
