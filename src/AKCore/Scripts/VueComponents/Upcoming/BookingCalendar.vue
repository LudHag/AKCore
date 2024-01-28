<template>
  <div>
    <div class="controls">
      <a
        href=""
        class="prev-month glyphicon glyphicon-chevron-left"
        @click.prevent="prevMonth"
        v-show="showPrevArrow"
      ></a>
      <span class="date"> Boka hålan</span>
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
          <booking-day
            :year="year"
            :month="month"
            :bookingevents="monthEvents"
            :day="addDays(day, i)"
            @callback="bookEvent"
            v-for="i in [0, 1, 2, 3, 4, 5, 6]"
            :key="month + '' + i"
          >
          </booking-day>
        </tr>
      </tbody>
    </table>
    <!-- <event-info-modal
      v-if="modalEvent"
      :event="modalEvent"
      :member="member"
      @signup="signup"
      @close="closeModal"
    ></event-info-modal> -->
  </div>
</template>
<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import Constants from "../../constants";
import CalendarDay from "./CalendarDay.vue";
import EventInfoModal from "./EventInfoModal.vue";
import { UpcomingYears, UpcomingEvent, BookingEvent } from "./models";
import { isEnglish } from "../../translations";
import BookingDay from "./BookingDay.vue";

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
const modalEvent = ref<BookingEvent | null>(null);
const monthEvents = ref<BookingEvent[]>([{id: 1,info: 'Banjofest', person: 'Emil', startsTime: '19:00', dayInMonth: 28}]);

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

const bookEvent = (date: Date) => {
  console.log(date);
  monthEvents.value = monthEvents.value.concat({dayInMonth: date.getDate(),id: 2,info: '2asda', person: 'Emil Jönsson', startsTime: '2'})

};

const closeModal = () => {
  modalEvent.value = null;
};

const addDays = (date: Date, days: number): Date => {
  const dat = new Date(date.valueOf());
  dat.setDate(dat.getDate() + days);
  return dat;
};

const days = computed(() => {
  return isEnglish ? Constants.DAYS_ENG : Constants.DAYS;
});



const thisMonthName = computed(() => {
  return isEnglish
    ? Constants.MONTHS_ENG[month.value]
    : Constants.MONTHS[month.value];
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
    monday = addDays(monday, 7);
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
