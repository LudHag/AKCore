<template>
  <div id="upcoming-app">
    <div v-if="!showEvent">
      <div class="calendar-actions" v-if="loggedIn">
        <a
          href="/upcoming/akevents.ics"
          @click.prevent="showIcal = !showIcal"
          class="fa fa-calendar"
        >
          Ical-länk</a
        >
        <div class="input-group ical-copy" v-if="showIcal">
          <input
            class="form-control"
            id="ical-link"
            type="text"
            readonly
            :value="icalLink"
          />
          <span class="input-group-btn">
            <button
              class="btn btn-default fa fa-files-o copy-btn"
              @click.prevent="copyIcal"
              type="button"
            ></button>
          </span>
        </div>
        <div class="calendar-control hidden-xs">
          <a
            href="#"
            class="event calendar-toggle"
            @click.prevent="calendarView = false"
            v-bind:class="{ active: !calendarView }"
            >Lista</a
          ><a
            href="#"
            class="month calendar-toggle"
            @click.prevent="calendarView = true"
            v-bind:class="{ active: calendarView }"
            >Månad</a
          >
        </div>
      </div>
      <spinner :size="'medium'" v-if="!years"></spinner>
      <upcoming-list
        v-if="!calendarView && years"
        :years="years"
        :logged-in="loggedIn"
        :member="member"
        @signup="signup"
      ></upcoming-list>
      <upcoming-calendar
        v-if="calendarView && years"
        :years="years"
        :logged-in="loggedIn"
        :member="member"
        @signup="signup"
      ></upcoming-calendar>
    </div>
    <keep-alive>
      <event-app
        v-if="showEvent"
        :event-id="selectedEventId"
        @close="closeEvent"
        @update="loadEvents"
      ></event-app>
    </keep-alive>
  </div>
</template>
<script setup lang="ts">
import Spinner from "../Spinner.vue";
import UpcomingList from "./UpcomingList.vue";
import UpcomingCalendar from "./UpcomingCalendar.vue";
import EventApp from "../Event/EventApp.vue";
import { UpcomingYears } from "./models";
import { ref, nextTick, onMounted } from "vue";

const props = defineProps<{
  eventId: number;
}>();

const years = ref<UpcomingYears | null>(null);
const loading = ref(false);
const loggedIn = ref(false);
const member = ref(false);
const calendarView = ref(false);
const icalLink = ref("");
const showIcal = ref(false);
const showEvent = ref(false);
const selectedEventId = ref(-1);
const latestTop = ref(0);

const copyIcal = () => {
  const copyText = document.querySelector("#ical-link") as HTMLInputElement;
  copyText.select();
  document.execCommand("copy");
};

const signup = (id: number) => {
  latestTop.value = window.pageYOffset;
  selectedEventId.value = id;
  showEvent.value = true;
  history.pushState(
    { showEvent: true, selectedEventId: id },
    "",
    "/upcoming/Event/" + id
  );
};

const closeEvent = () => {
  showEvent.value = false;
  history.pushState({ showEvent: false, selectedEventId: -1 }, "", "/upcoming");
  nextTick(() => {
    window.scrollTo(0, latestTop.value);
  });
};

const loadEvents = () => {
  loading.value = true;

  fetch("/Upcoming/UpcomingListData")
    .then((res) => res.json())
    .then((res) => {
      years.value = res.years;
      loggedIn.value = res.loggedIn;
      member.value = res.member;
      icalLink.value = res.icalLink;
      loading.value = false;
    })
    .catch(() => {
      console.log("fel");
      loading.value = false;
    });
};

onMounted(() => {
  if (props.eventId > -1) {
    selectedEventId.value = props.eventId;
    showEvent.value = true;
    history.replaceState(
      { showEvent: true, selectedEventId: props.eventId },
      "",
      "/upcoming/Event/" + props.eventId
    );
  } else {
    history.replaceState(
      { showEvent: false, selectedEventId: -1 },
      "",
      "/upcoming"
    );
  }
  loadEvents();
  window.onpopstate = function (event) {
    if (event.state) {
      showEvent.value = event.state.showEvent;
      selectedEventId.value = event.state.selectedEventId;
    }
  };
});
</script>
<style lang="scss"></style>
