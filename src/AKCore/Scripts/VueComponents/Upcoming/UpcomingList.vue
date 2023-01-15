<template>
  <div>
    <div v-for="year in years" :key="year.year">
      <h2>{{ year.year }}</h2>
      <div v-for="month in year.months" :key="getMonthName(month)">
        <h3 class="new-month">{{ getMonthName(month) }}</h3>
        <upcoming-list-item
          v-for="event in month"
          :key="event.id"
          :event="event"
          :logged-in="loggedIn"
          :member="member"
          @signup="signup"
        >
        </upcoming-list-item>
      </div>
    </div>
    <p v-if="noYears">
      Vi har tyvärr inga spelningar inplanerade närmaste tiden.
    </p>
  </div>
</template>
<script setup lang="ts">
import { computed } from "vue";
import Constants from "../../constants";
import { UpcomingYears, UpcomingEvent } from "./models";
import UpcomingListItem from "./UpcomingListItem.vue";

const emit = defineEmits<{
  (e: "signup", id: number): void;
}>();

const props = defineProps<{
  years: UpcomingYears;
  loggedIn: boolean;
  member: boolean;
}>();

const getMonthName = (monthEvents: UpcomingEvent[]) => {
  return Constants.MONTHS[monthEvents[0].month - 1];
};

const signup = (id: number) => {
  emit("signup", id);
};

const noYears = computed(() => {
  for (const key in props.years) {
    if (props.years.hasOwnProperty(key)) return false;
  }
  return true;
});
</script>
<style lang="scss"></style>
