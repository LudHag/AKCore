<template>
  <td class="day" :class="{ outside: outside }">
    <span class="date">{{ day.getDate() }}</span>
    <a
      href="#"
      v-for="e in events"
      :key="e.id"
      @click.prevent="openEvent(e)"
      class="dayEvent"
      :class="{ green: e.signupState }"
    >
      {{ e.halanTime }} {{ e.signupState }} {{ eventName(e) }}
    </a>
  </td>
</template>
<script setup lang="ts">
import { computed } from "vue";
import { UpcomingEvent } from "./models";
import { eventIsRep } from "./functions";
import { TranslationDomain, translate } from "@scripts/translations";

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

const t = (key: string, domain: TranslationDomain = "upcoming") => {
  return translate(domain, key);
};

const eventName = (e: UpcomingEvent) => {
  if (eventIsRep(e)) {
    return t(e.type);
  }
  return e.name;
};
</script>
<style lang="scss" scoped>
.day {
  height: 90px;

  &.outside {
    color: #5f5f5f;
  }

  .dayEvent {
    display: block;
    font-size: 12px;
    overflow: hidden;
    text-overflow: ellipsis;
  }
  .green {
    color: #02c66f;
  }
}
</style>
