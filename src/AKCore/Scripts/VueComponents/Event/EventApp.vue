﻿<template>
  <div id="event-app">
    <spinner v-if="loading && !eventInfo" :size="'medium'"></spinner>
    <div v-if="eventInfo">
      <a
        href="#"
        @click.prevent="close"
        class="close-event pull-right glyphicon glyphicon-remove"
      ></a>
      <h1>{{ eventInfo.event.name }}</h1>
      <div class="row hidden-print">
        <div class="col-sm-6">
          <div v-if="eventInfo.event.disabled" class="col-sm-12">
            <p>
              <span class="glyphicon glyphicon-warning-sign"></span>
              <span class="warning-text">{{
                t("sign-up-not-allowed", "common")
              }}</span>
            </p>
          </div>
          <div v-else>
            <event-form
              :event-info="eventInfo"
              @update="loadEvents"
            ></event-form>
          </div>
        </div>
        <div class="col-sm-6">
          <div class="col-sm-12" style="font-weight: 500">
            <p style="text-transform: capitalize">{{ eventInfo.event.day }}</p>
            <p>{{ eventInfo.event.place }}</p>
            <br />
          </div>
          <div class="col-sm-12">
            <p v-if="eventInfo.event.halanTime">
              {{ t("gather-in-hole", "common") }}:
              {{ eventInfo.event.halanTime }}
            </p>
            <p v-if="eventInfo.event.thereTime">
              {{ t("gather-there", "common") }}: {{ eventInfo.event.thereTime }}
            </p>
            <p v-if="eventInfo.event.startsTime">
              {{ t("concert-starts", "common") }}:
              {{ eventInfo.event.startsTime }}
            </p>
            <p v-if="eventInfo.event.playDuration">
              {{ t("play-duration", "common") }}:
              {{ eventInfo.event.playDuration }}
            </p>
          </div>
          <div class="col-sm-12">
            <a
              href="#"
              class="btn btn-default"
              v-if="eventInfo.isNintendo"
              @click.prevent="showAdminEdit"
            >
              Lägg till anmälningar
            </a>
            <a href="#" class="btn btn-default" @click.prevent="toggleInfo">
              {{ t("show-info") }}
            </a>
          </div>
        </div>
        <div class="col-xs-12" v-if="showInfo">
          <p v-if="eventInfo.event.description">
            {{ eventInfo.event.description }}
          </p>
          <p v-if="eventInfo.event.internalDescription">
            {{ eventInfo.event.internalDescription }}
          </p>
        </div>
      </div>
      <div>
        <edit-signup-modal
          v-if="eventInfo"
          :show-modal="showEditForm"
          :event-id="eventId"
          :members="eventInfo.members"
          @update="loadEvent"
          @close="closeModal"
        ></edit-signup-modal>
      </div>
      <signup-list
        :signups="eventInfo.signups"
        :nintendo="eventInfo.isNintendo"
      ></signup-list>
    </div>
  </div>
</template>
<script setup lang="ts">
import Spinner from "../Spinner.vue";
import EventForm from "./EventForm.vue";
import SignupList from "./SignupList.vue";
import EditSignupModal from "./EditSignupModal.vue";
import { ref, computed, toRefs, watch, onMounted } from "vue";
import { UpcomingEventInfo } from "../Upcoming/models";
import { getFromApi } from "../../services/apiservice";
import { TranslationDomain, translate } from "../../translations";

const emit = defineEmits<{
  (e: "close"): void;
  (e: "update"): void;
}>();

const props = defineProps<{
  eventId: number;
}>();

const { eventId } = toRefs(props);

const events = ref<{ [eventId: number]: UpcomingEventInfo }>({});
const loading = ref(false);
const showEditForm = ref(false);
const showInfo = ref(false);

const eventInfo = computed(() => {
  if (eventId.value < 0 || !events.value) {
    return null;
  }
  const eInfo = events.value[eventId.value];
  return eInfo;
});

watch(eventId, () => {
  if (!events.value[eventId.value]) {
    loadEvent();
  }
});

const loadEvents = () => {
  loadEvent();
  emit("update");
};

const loadEvent = async () => {
  loading.value = true;
  const result = await getFromApi("/upcoming/Event/EventData/" + eventId.value);
  events.value = Object.assign({}, events.value, {
    [eventId.value]: result,
  });
  loading.value = false;
};

const close = () => {
  emit("close");
};

const showAdminEdit = () => {
  showEditForm.value = true;
};

const closeModal = () => {
  showEditForm.value = false;
};

const toggleInfo = () => {
  showInfo.value = !showInfo.value;
};

onMounted(() => {
  if (eventId.value > -1) {
    loadEvent();
  }
});
const t = (key: string, domain: TranslationDomain = "signup") => {
  return translate(domain, key);
};
</script>
<style lang="scss" scoped>
@import "../../../Styles/variables.scss";
.close-event {
  font-size: 26px;
}
.warning-text {
  margin-left: 8px;
}
.glyphicon-warning-sign {
  color: $akred;
}
</style>
