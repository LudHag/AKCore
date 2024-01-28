<template>
  <div id="upcoming-app">
    <div v-if="!showEvent">
      <div class="calendar-actions" v-if="loggedIn">
        <div class="ical-container">
          <a
            href="/upcoming/akevents.ics"
            @click.prevent="showIcal = !showIcal"
            class="fa fa-calendar"
          >
            {{ " " + t("ical-link") }}
          </a>
          <div class="input-group ical-copy" v-if="showIcal">
            <input
              class="form-control"
              id="ical-link"
              type="text"
              readonly
              :value="icalLink"
            />
            <span class="input-group-btnp">
              <button
                class="btn btn-default fa fa-files-o copy-btn"
                @click.prevent="copyIcal"
                type="button"
              ></button>
            </span>
          </div>
        </div>
        <div class="calendar-control hidden-xs">
          <a
            href="#"
            class="event calendar-toggle"
            @click.prevent="view = 'list'"
            :class="{ active: view === 'list' }"
          >
            {{ t("list") }}
          </a>
          <a
            href="#"
            class="month calendar-toggle"
            @click.prevent="view = 'calendar'"
            :class="{ active: view === 'calendar' }"
          >
            {{ t("month") }}
          </a>
          <a
            href="#"
            class="month calendar-toggle"
            @click.prevent="view = 'booking'"
            :class="{ active: view === 'booking' }"
          >
            {{ t("book") }}
          </a>
        </div>
      </div>
      <spinner :size="'medium'" v-if="!years"></spinner>
      <upcoming-list
        v-if="view === 'list' && years"
        :years="years"
        :logged-in="loggedIn"
        :member="member"
        @signup="signup"
      ></upcoming-list>
      <upcoming-calendar
        v-if="view === 'calendar' && years"
        :years="years"
        :logged-in="loggedIn"
        :member="member"
        @signup="signup"
      ></upcoming-calendar>
      <booking-calendar
        v-if="view === 'booking' && years"
        :years="years"
        :logged-in="loggedIn"
        :member="member"
        @signup="signup"
      ></booking-calendar>
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
import { TranslationDomain, translate } from "../../translations";
import BookingCalendar from "./BookingCalendar.vue";

const props = defineProps<{
  eventId: number;
}>();

const years = ref<UpcomingYears | null>(null);
const loading = ref(false);
const loggedIn = ref(false);
const member = ref(false);
const view = ref<'list' | 'calendar' | 'booking'>('list');
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
const t = (key: string, domain: TranslationDomain = "upcoming") => {
  return translate(domain, key);
};
</script>
<style lang="scss" scoped>
.calendar-actions {
  float: right;
  text-align: right;
  max-width: 360px;

  .calendar-control {
    margin-top: 20px;
    margin-bottom: 20px;
    .calendar-toggle {
      padding: 4px 10px;
      color: #000;
      display: inline-block;
      background-color: #808080;

      &.event {
        border-radius: 7px 0 0 7px;
      }

      &.month {
        border-radius: 0 7px 7px 0;
      }

      &.active {
        background-color: #5f5f5f;
      }
    }
  }
}

.ical-container {
  display: flex;
  align-items: center;
  justify-content: end;
  gap: 10px;

  .input-group {
    display: flex;
    gap: 10px;
  }
}
</style>
