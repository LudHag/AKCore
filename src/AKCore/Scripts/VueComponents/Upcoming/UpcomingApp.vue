<template>
  <div id="upcoming-app">
    <div v-if="!showEvent">
      <div class="calendar-actions" v-if="loggedIn">
        <div class="ical-container">
          <a
            @click.prevent="showIcal = !showIcal"
            @keydown.prevent.enter="showIcal = !showIcal"
            @keydown.prevent.space="showIcal = !showIcal"
            tabindex="0"
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
              :value="fullIcalLink"
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

        <select
          class="form-control rehersal-select visible-xs"
          v-model="rehearsalFilter"
          @change="
            handleFilterChange(
              ($event.target as HTMLSelectElement).value as RepFilterType,
            )
          "
        >
          <option value="all" key="all">{{ t("allFilter") }}</option>
          <option value="ballet" key="balett">{{ t("balletFilter") }}</option>
          <option value="orchestra" key="orchestra">
            {{ t("orchestraFilter") }}
          </option>
        </select>

        <div class="calendar-control hidden-xs">
          <a
            href="#"
            class="left toggle"
            :class="{ active: rehearsalFilter === 'all' }"
            @click.prevent="handleFilterChange('all')"
            @keydown.prevent.enter="handleFilterChange('all')"
            @keydown.prevent.space="handleFilterChange('all')"
          >
            {{ t("allFilter") }}
          </a>
          <a
            href="#"
            class="toggle"
            :class="{ active: rehearsalFilter === 'ballet' }"
            @click.prevent="handleFilterChange('ballet')"
            @keydown.prevent.enter="handleFilterChange('ballet')"
            @keydown.prevent.space="handleFilterChange('ballet')"
          >
            {{ t("balletFilter") }}
          </a>
          <a
            href="#"
            class="right toggle"
            :class="{ active: rehearsalFilter === 'orchestra' }"
            @click.prevent="handleFilterChange('orchestra')"
            @keydown.prevent.enter="handleFilterChange('orchestra')"
            @keydown.prevent.space="handleFilterChange('orchestra')"
          >
            {{ t("orchestraFilter") }}
          </a>
        </div>
        <div class="calendar-control hidden-xs">
          <a
            href="#"
            class="left toggle"
            @click.prevent="calendarView = false"
            @keydown.prevent.enter="calendarView = false"
            @keydown.prevent.space="calendarView = false"
            :class="{ active: !calendarView }"
          >
            {{ t("list") }}
          </a>
          <a
            href="#"
            class="right toggle"
            @click.prevent="calendarView = true"
            @keydown.prevent.enter="calendarView = true"
            @keydown.prevent.space="calendarView = true"
            :class="{ active: calendarView }"
          >
            {{ t("month") }}
          </a>
        </div>
      </div>

      <spinner :size="'medium'" v-if="!years"></spinner>
      <upcoming-list
        v-if="!calendarView && years"
        :years="years"
        :logged-in="loggedIn"
        :member="member"
        @signup="signup"
        class="upcomming-list"
        :class="{ loggedIn }"
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
const rehearsalFilter = ref<RepFilterType>("all");

const calendarImage = getImageLink("calendar.svg");
const copyImage = getImageLink("copy.svg");

const copyIcal = () => {
  const copyText = document.querySelector("#ical-link") as HTMLInputElement;
  copyText.select();
  document.execCommand("copy");
};

import { computed } from "vue";
import { RepFilterType } from "../models";
import { filterYears } from "./functions";

const allYears = ref<UpcomingYears | null>(null);

const years = computed(() => {
  if (!allYears.value) return null;

  return filterYears(allYears.value, rehearsalFilter.value);
});

const fullIcalLink = computed(() => {
  return icalLink.value + `?rehearsalFilter=${rehearsalFilter.value}`;
});

const loadEvents = () => {
  loading.value = true;
  fetch("/Upcoming/UpcomingListData")
    .then((res) => res.json())
    .then((res) => {
      allYears.value = res.years;
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

const handleFilterChange = (value: RepFilterType) => {
  rehearsalFilter.value = value;
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
@import "bootstrap-sass/assets/stylesheets/bootstrap/_variables.scss";
.calendar-actions {
  float: right;
  text-align: right;
  max-width: 500px;

  .calendar-control {
    margin-top: 20px;
    margin-bottom: 20px;
    display: flex;
    align-items: center;
    justify-content: flex-end;
    .toggle {
      padding: 4px 10px;
      color: #000;
      display: inline-block;
      background-color: #808080;
      border-right: 1px solid #575656;

      &.left {
        border-radius: 7px 0 0 7px;
      }

      &.right {
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
.upcomming-list.loggedIn {
  padding-top: 35px;
}

.rehersal-select {
  margin-top: 10px;
  color: #000;
}

@media screen and (max-width: $screen-xs-max) {
  .upcomming-list.loggedIn {
    padding-top: 0;
  }
}
</style>
