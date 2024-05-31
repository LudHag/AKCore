<template>
  <section v-if="profileStatistics">
    <h3>{{ t("statisticsheader") }}</h3>
    <p>{{ t("statisticspreamble") }}</p>
    <ul>
      <li>
        {{ t("total-gigs") }}:
        <span class="bold">{{ profileStatistics.totalGigs }}</span>
      </li>
      <li>
        {{ t("halan") }}:
        <span class="bold">{{ profileStatistics.halan }}</span> -
        {{
          getPercentage(profileStatistics.halan, profileStatistics.totalGigs)
        }}%
      </li>
      <li>
        {{ t("direct") }}:
        <span class="bold">{{ profileStatistics.direct }}</span> -
        {{
          getPercentage(profileStatistics.direct, profileStatistics.totalGigs)
        }}%
      </li>
      <li>
        {{ t("cantCome") }}:
        <span class="bold">{{ profileStatistics.cantCome }}</span> -
        {{
          getPercentage(
            profileStatistics.cantCome,
            profileStatistics.totalGigs,
          )
        }}%
      </li>
      <li>
        {{ t("car") }}: <span class="bold">{{ profileStatistics.car }}</span> -
        {{ getPercentage(profileStatistics.car, profileStatistics.totalGigs) }}%
      </li>
      <li>
        {{ t("instrument-own") }}:
        <span class="bold">{{ profileStatistics.instrument }}</span> -
        {{
          getPercentage(
            profileStatistics.instrument,
            profileStatistics.totalGigs,
          )
        }}%
      </li>
      <li>
        {{ t("comment") }}:
        <span class="bold">{{ profileStatistics.comment }}</span> -
        {{
          getPercentage(profileStatistics.comment, profileStatistics.totalGigs)
        }}%
      </li>
    </ul>
  </section>
</template>
<script setup lang="ts">
import { ProfileStatisticsModel } from "./models";
import { TranslationDomain, translate } from "../../translations";

const props = defineProps<{ profileStatistics: ProfileStatisticsModel }>();
const { profileStatistics } = props;

const getPercentage = (value: number, total: number) => {
  return ((value / total) * 100).toFixed(2);
};

const t = (key: string, domain: TranslationDomain = "profile") => {
  return translate(domain, key);
};
</script>
<style scoped lang="scss">
section {
  padding-bottom: 1rem;
}
.bold {
  font-weight: bold;
  color: #d74b4b;
}
</style>
