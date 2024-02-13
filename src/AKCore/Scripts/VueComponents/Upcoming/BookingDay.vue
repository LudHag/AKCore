<template>
  <td class="day" 
  :class="{ outside: outside }"
  @click="openEvent()">
    <span class="date">{{ day.getDate() }}</span>
    <a
      href="#"
      v-for="e in events"
      :key="e.id"
      @click.prevent="openEvent()"
      class="dayEvent"

      :class="{ green: e.message }"
    >
      {{ e.person }} {{ e.startsTime }} {{ e.message }}
    </a>
  </td>
</template>
<script setup lang="ts">
import { computed } from "vue";
import { BookingEvent, UpcomingEvent } from "./models";
import { eventIsRep } from "./functions";
import { TranslationDomain, translate } from "../../translations";

const today = new Date();
const emit = defineEmits<{
  (e: "open", day: Date, event?: BookingEvent): void;
  (e: 'callback', day: Date): void;
}>();

const props = defineProps<{
  bookingevents: BookingEvent[];
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
  if (!props.bookingevents || outside.value) {
    return [];
  }
  return props.bookingevents.filter((e) => {
    return e.dayInMonth === props.day.getDate();
  });
});

const openEvent = () => {
  console.log('open', props.day);
  
  emit("open", props.day);
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
