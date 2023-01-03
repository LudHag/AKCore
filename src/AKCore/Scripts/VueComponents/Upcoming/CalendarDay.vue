<template>
  <td class="day" v-bind:class="{ outside: outside }">
    <span class="date">{{ day.getDate() }}</span>
    <a
      href="#"
      v-for="e in events"
      @click.prevent="openEvent(e)"
      class="dayEvent"
      :class="{ green: e.signupState }"
    >
      {{ e.halanTime }} {{ e.signupState }} {{ e.name }}</a
    >
  </td>
</template>
<script setup lang="ts">
import { computed } from "vue";
import { UpcomingEvent } from "./models";

const today = new Date();
const emit = defineEmits<{
  (e: "open", event: UpcomingEvent): void;
}>();

const props = defineProps<{
  monthevents: UpcomingEvent[];
  day: Date;
  month: number;
  year: number;
}>();

const outside = computed(() => {
  if (
    props.month == props.day.getMonth() &&
    props.year == props.day.getFullYear()
  ) {
    return (
      props.year == today.getFullYear() &&
      props.month == today.getMonth() &&
      props.day.getDate() < today.getDate()
    );
  } else {
    return true;
  }
});

const events = computed(() => {
  if (!props.monthevents || outside.value) {
    return [];
  }
  return props.monthevents.filter((e) => {
    return e.dayInMonth === props.day.getDate();
  });
});

const openEvent = (e: UpcomingEvent) => {
  emit("open", e);
};
</script>
<style lang="scss"></style>
