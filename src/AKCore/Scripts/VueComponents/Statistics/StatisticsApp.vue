<template>
  <div>
    <h2>Sidladdningar</h2>

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
    <PageViewGraph v-if="dataPoints" :data-points="dataPoints" />
  </div>
</template>
<script setup lang="ts">
import { onMounted, ref, watch } from "vue";
import { getFromApi } from "../../services/apiservice";
import PageViewGraph from "./PageViewGraph.vue";
import { RequestsResponse } from "./models";

const dataPoints = ref<RequestsResponse | null>(null);
const loggedIn = ref<boolean>(true);
const loggedOut = ref<boolean>(true);

watch(loggedIn, () => {
  reloadData();
});

watch(loggedOut, () => {
  reloadData();
});

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
.checkbox {
  display: flex;
  gap: 10px;
}
</style>
