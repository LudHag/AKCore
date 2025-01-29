<template>
  <div id="upcoming-app">
    <div v-if="!showEvent">
      <div class="calendar-actions" v-if="loggedIn">
        <div class="ical-container">
          <a
            href="/upcoming/akevents.ics"
            @click.prevent="showIcal = !showIcal"
          >
            <img
              style="max-width: 20px; vertical-align: baseline"
              :src="calendarImage"
            />
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
                class="btn btn-default copy-btn"
                @click.prevent="copyIcal"
                type="button"
              >
                <img style="max-width: 20px; display: block" :src="copyImage" />
              </button>
            </span>
          </div>
        </div>
        <div class="calendar-control">
          <div class="hidden-xs">
            <a
              href="#"
              class="event calendar-toggle"
              @click.prevent="calendarView = false"
              :class="{ active: !calendarView }"
            >
              {{ t("list") }}
            </a>
            <a
              href="#"
              class="month calendar-toggle"
              @click.prevent="calendarView = true"
              :class="{ active: calendarView }"
            >
              {{ t("month") }}
            </a>
        </div>
          <div class="rehearsal-filter">
            <select
              class="form-control"
              name="rehearsal-filter"
              v-model="rehearsalFilter"
              @change="handleFilterChange"
            >
              <option value="all" key="all"> {{ t("allFilter") }}</option>
              <option value="ballet" key="balett"> {{ t("balletFilter") }}</option>
              <option value="orchestra" key="orchestra"> {{ t("orchestraFilter") }}</option>
            </select>
          </div>
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
import { UpcomingMonths, UpcomingYears } from "./models";
import { ref, nextTick, onMounted } from "vue";
import { TranslationDomain, translate } from "../../translations";
import { getCookie, getImageLink, setCookie } from "../../general";

const props = defineProps<{
  eventId: number;
}>();

const loading = ref(false);
const loggedIn = ref(false);
const member = ref(false);
const calendarView = ref(false);
const icalLink = ref("");
const showIcal = ref(false);
const showEvent = ref(false);
const selectedEventId = ref(-1);
const latestTop = ref(0);
const rehearsalFilter = ref<RepFilterType> ('all')

const calendarImage = getImageLink("calendar.svg");
const copyImage = getImageLink("copy.svg");

const copyIcal = () => {
  const copyText = document.querySelector("#ical-link") as HTMLInputElement;
  copyText.select();
  document.execCommand("copy");
};


import { computed } from "vue";
import { RepFilterType } from "../models";

const allYears = ref<UpcomingYears | null>(null); // Store all unfiltered events

const years = computed(() => {
  if (!allYears.value) return null;

  const filteredYears: UpcomingYears = {};

  Object.entries(allYears.value).forEach(([year, yearData]) => {
    const filteredMonths: UpcomingMonths = {};

    Object.entries(yearData.months).forEach(([month, events]) => {
      // Apply filter to events
      const filteredEvents = events.filter((event) => {
        if (rehearsalFilter.value === "all") return true;
        if (rehearsalFilter.value === 'ballet' && event.type === 'Rep') return false
        if (rehearsalFilter.value === 'orchestra' && event.type === 'Balettrep') return false
        return true;
      });

      if (filteredEvents.length > 0) {
        filteredMonths[month] = filteredEvents;
      }
    });

    if (Object.keys(filteredMonths).length > 0) {
      filteredYears[year] = { ...yearData, months: filteredMonths };
    }
  });

  return filteredYears;
});

const loadEvents = () => {
  loading.value = true;
  fetch("/Upcoming/UpcomingListData")
    .then((res) => res.json())
    .then((res) => {
      allYears.value = res.years; // Store unfiltered data
      loggedIn.value = res.loggedIn;
      member.value = res.member;
      icalLink.value = res.icalLink;
      loading.value = false;
    })
    .catch(() => {
      console.log("Error loading events");
      loading.value = false;
    });
};

const signup = (id: number) => {
  latestTop.value = window.pageYOffset;
  selectedEventId.value = id;
  showEvent.value = true;
  history.pushState(
    { showEvent: true, selectedEventId: id },
    "",
    "/upcoming/Event/" + id,
  );
};

const closeEvent = () => {
  showEvent.value = false;
  history.pushState({ showEvent: false, selectedEventId: -1 }, "", "/upcoming");
  nextTick(() => {
    window.scrollTo(0, latestTop.value);
  });
};

const handleFilterChange = () => {
  loadEvents();
  setCookie("rehersalFilter", rehearsalFilter.value, 365);
};

const initializeFilter = () => {
  const savedFilter = getCookie("rehersalFilter");
  if (savedFilter) {
    rehearsalFilter.value = savedFilter as RepFilterType;
  }
};


onMounted(() => {
  initializeFilter();
  if (props.eventId > -1) {
    selectedEventId.value = props.eventId;
    showEvent.value = true;
    history.replaceState(
      { showEvent: true, selectedEventId: props.eventId },
      "",
      "/upcoming/Event/" + props.eventId,
    );
  } else {
    history.replaceState(
      { showEvent: false, selectedEventId: -1 },
      "",
      "/upcoming",
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
    display: flex;
    align-items: center;
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
.rehearsal-filter {
  color: #000;
  max-width: fit-content;
  margin-left: 10px;
}
</style>
