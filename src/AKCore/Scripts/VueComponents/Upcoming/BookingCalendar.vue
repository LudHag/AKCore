<template>
  <div>
    <div class="controls">
      <a href="" class="prev-month glyphicon glyphicon-chevron-left" @click.prevent="prevMonth"
        v-show="showPrevArrow"></a>
      <span class="date"> {{ thisMonthName }}</span>
      <a href="" class="next-month glyphicon glyphicon-chevron-right" @click.prevent="nextMonth"></a>
    </div>
    <table class="month table table-bordered">
      <thead>
        <tr>
          <th v-for="day in days" :key="day">{{ day }}</th>
        </tr>
      </thead>
      <tbody>
        <tr class="week" v-for="day of firstWeekDays" :key="day.toString()">
          <booking-day :year="year" :month="month" :bookingevents="monthEvents" :day="addDays(day, i)"
             @open="openEvent" v-for="i in [0, 1, 2, 3, 4, 5, 6]" :key="month + '' + i">
          </booking-day>
        </tr>
      </tbody>
    </table>
    <booking-info-modal
      v-if="modalIsOpen"
      :event="modalEvent"
      :member="member"
      :is-open="modalIsOpen"
      @onBook="bookEvent"
      @close="closeModal"
    ></booking-info-modal>
  </div>
</template>
<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import Constants from "../../constants";
import BookingInfoModal from "./BookingInfoModal.vue";
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
const modalIsOpen = ref(false);
const modalEvent = ref<Date | null>(null);
const totalEvents = ref<BookingEvent[]>([]);
const monthEvents = ref<BookingEvent[]>([]);

const signup = (id: number) => {
  closeModal();
  emit("signup", id);
};
const setShownEvents = () => {
  let newEvents = [] as BookingEvent[];
  totalEvents.value.forEach(e => {
    if (e.date.getMonth() === month.value) {
      newEvents = newEvents.concat(e)
    }
  })

  monthEvents.value = newEvents;
  console.log(monthEvents.value);

}

const openEvent = (date: Date) => {
  console.log('openmodal');
  modalIsOpen.value = true;
  modalEvent.value = date;
};

const nextMonth = () => {
  month.value++;
  if (month.value > 11) {
    month.value = 0;
    year.value++;
  }
  setShownEvents();
};

const prevMonth = () => {
  month.value--;
  if (month.value < 0) {
    month.value = 11;
    year.value--;
  }
  setShownEvents();
};

interface BookingItem {
  id: number;
  person: string;
  message: string;
  bookedDate: Date;
  approved: boolean;
}

const loadEvents = () => {
  totalEvents.value = [];
  fetch("/Booking/GetItems")
    .then((res) => res.json())
    .then((res: BookingItem[]) => {
      let newEvents = totalEvents.value;
      res.forEach(r => {
        const dateTime = new Date(r.bookedDate);

        const newEvent = {
          dayInMonth: dateTime.getDate(),
          date: dateTime,
          person: r.person,
          startsTime: dateTime.toLocaleTimeString(),
          message: r.message,

        } as BookingEvent

        newEvents = newEvents.concat(newEvent)
      });
      totalEvents.value = newEvents;
      setShownEvents();
    })
    .catch((e) => {
      console.log("fel", e);
    });


};



const bookEvent = async (bookingEvent: BookingEvent) => {

  const {date, title, message, startsTime} = bookingEvent;
  //monthEvents.value = monthEvents.value.concat({ dayInMonth: date.getDate(), title, id: 2, message, , startsTime, date: date })
  const dateUTC = date;

  const time = startsTime.replaceAll('0', '').split(':');
  console.log('startsTime', time);
  const newDate = new Date(date.getTime() + (60*60*1000* +time[0]) + (60*1000* +time[1]))


  
  let formData = new FormData();
  formData.append('message', message);
  formData.append('bookedDate', newDate.toJSON());
  const response = await fetch("/Booking/SaveBooking/", {
    method: "POST",
    body: formData,
  });

  loadEvents();
};

const closeModal = () => {
  modalEvent.value = null;
  modalIsOpen.value = false;
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
  loadEvents();
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
