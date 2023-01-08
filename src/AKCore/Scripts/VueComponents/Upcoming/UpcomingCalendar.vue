<template>
  <div>
    <div class="controls">
      <a
        href=""
        class="prev-month glyphicon glyphicon-chevron-left"
        @click.prevent="prevMonth"
        v-show="showPrevArrow"
      ></a>
      <span class="date">{{ thisMonthName }} {{ year }}</span>
      <a
        href=""
        class="next-month glyphicon glyphicon-chevron-right"
        @click.prevent="nextMonth"
      ></a>
    </div>
    <table class="month table table-bordered">
      <thead>
        <tr>
          <th v-for="day in days" :key="day">{{ day }}</th>
        </tr>
      </thead>
      <tbody>
        <tr class="week" v-for="day of firstWeekDays" :key="day.toString()">
          <calendar-day
            :year="year"
            :month="month"
            :monthevents="monthEvents"
            :day="day.addDays(i)"
            v-for="i in [0, 1, 2, 3, 4, 5, 6]"
            :key="month + '' + i"
            @open="openEvent"
          >
            >
          </calendar-day>
        </tr>
      </tbody>
    </table>
    <event-info-modal
      v-if="modalEvent"
      :event="modalEvent"
      :member="member"
      @signup="signup"
      @close="closeModal"
    ></event-info-modal>
  </div>
</template>
<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import Constants from "../../constants";
import CalendarDay from "./CalendarDay.vue";
import EventInfoModal from "./EventInfoModal.vue";
import { UpcomingYears, UpcomingEvent } from "./models";

const timeDay = 24 * 60 * 60 * 1000;
const today = new Date();

const emit = defineEmits<{
  (e: "signup", id: number): void;
}>();

const props = defineProps<{
  years: UpcomingYears;
  loggedIn: boolean;
  member: boolean;
}>();

const month = ref(0);
const year = ref(0);
const modalEvent = ref<UpcomingEvent | null>(null);

const signup = (id: number) => {
  closeModal();
  emit("signup", id);
};

const nextMonth = () => {
  month.value++;
  if (month.value > 11) {
    month.value = 0;
    year.value++;
  }
};

const prevMonth = () => {
  month.value--;
  if (month.value < 0) {
    month.value = 11;
    year.value--;
  }
};

const openEvent = (e: UpcomingEvent) => {
  modalEvent.value = e;
};

const closeModal = () => {
  modalEvent.value = null;
};

const days = Constants.DAYS;

const monthEvents = computed(() => {
  const yearEvents = props.years[year.value];
  if (yearEvents && yearEvents.months) {
    const events = yearEvents.months[month.value + 1];
    if (events) return events;
  }
  return [];
});

const thisMonthName = computed(() => {
  return Constants.MONTHS[month.value];
});

const firstWeekDays = computed(() => {
  const firstDayOfMonth = new Date(year.value, month.value, 1);
  const lastDayOfMonth = new Date(year.value, month.value + 1, 0);
  let firstDayWeekDay = firstDayOfMonth.getDay() - 1;
  if (firstDayWeekDay < 0) firstDayWeekDay = 7 + firstDayWeekDay;
  const firstDayOfCalendar = new Date(
    firstDayOfMonth.getTime() - firstDayWeekDay * timeDay
  );
  let monday = new Date(firstDayOfCalendar.getTime());
  const weeks = [];
  while (monday < lastDayOfMonth) {
    weeks.push(monday);
    monday = monday.addDays(7);
  }
  return weeks;
});

const showPrevArrow = computed(() => {
  return (
    month.value - 1 >= today.getMonth() || year.value > today.getFullYear()
  );
});

onMounted(() => {
  year.value = today.getFullYear();
  month.value = today.getMonth();
});
</script>
<style lang="scss" scoped>
@import "../../../Styles/variables.scss";
.controls {
  .date {
    min-width: 110px;
    display: inline-block;
    text-align: center;
  }
}

.table-bordered {
  border: 3px solid $akred;
}
</style>
