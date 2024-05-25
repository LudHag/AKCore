<template>
  <modal :show-modal="showModal" header="Redigera händelse" @close="close">
    <template #body>
      <form
        action="/AdminEvent/Edit"
        method="post"
        @submit.prevent="formSubmit"
        v-if="upcomingEvent"
      >
        <div class="modal-body">
          <div
            class="alert alert-danger"
            ref="error"
            style="display: none"
          ></div>
          <input type="hidden" name="Id" :value="eventId" />
          <div class="form-group">
            <label>Typ</label>
            <select
              class="form-control"
              name="Type"
              v-model="eventType"
              required
            >
              <option value>Typ av händelse</option>
              <option v-for="e in EVENTTYPES" :key="e">{{ e }}</option>
            </select>
          </div>
          <div class="editeventbody" v-if="eventType">
            <div
              class="form-group"
              v-if="spelningFest || eventType === 'Evenemang'"
            >
              <div class="row">
                <div class="col-sm-6">
                  <label>Namn</label>
                  <input
                    class="form-control"
                    v-model="upcomingEvent.name"
                    name="Name"
                  />
                </div>
                <div class="col-sm-6" v-if="eventType === 'Spelning'">
                  <label></label>
                  <div class="checkbox checkbox-center">
                    <label>
                      <input
                        type="checkbox"
                        v-model="upcomingEvent.secret"
                        name="Secret"
                      />
                      Hemlig spelning
                    </label>
                  </div>
                </div>
              </div>
            </div>
            <div
              class="form-group"
              v-if="spelningFest || eventType == 'Evenemang'"
            >
              <div class="row">
                <div class="col-sm-6">
                  <label>Plats</label>
                  <input
                    class="form-control"
                    v-model="upcomingEvent.place"
                    name="Place"
                  />
                </div>
                <div class="col-sm-6" v-if="eventType === 'Spelning'">
                  <label>Stå- eller gåspelning</label>
                  <select
                    class="form-control"
                    v-model="upcomingEvent.stand"
                    name="Stand"
                    required
                  >
                    <option value>Välj speltyp</option>
                    <option v-for="playType in SPELTYPER" :key="playType">
                      {{ playType }}
                    </option>
                  </select>
                </div>
              </div>
            </div>
            <div class="form-group">
              <div class="row">
                <div class="col-sm-3">
                  <label>Dag</label>
                  <datepicker
                    class="form-control"
                    v-model="upcomingEvent.dayDate as Date"
                    name="Day"
                    required
                  ></datepicker>
                </div>
                <div class="col-sm-3" v-if="eventType !== 'Evenemang'">
                  <label>Vid hålan</label>
                  <input
                    class="form-control"
                    type="time"
                    v-model="upcomingEvent.halanTime"
                    name="Halan"
                  />
                </div>
                <div class="col-sm-3" v-if="spelningKarhus">
                  <label>På plats</label>
                  <input
                    class="form-control"
                    type="time"
                    v-model="upcomingEvent.thereTime"
                    name="There"
                  />
                </div>
                <div class="col-sm-3" v-if="eventType === 'Spelning'">
                  <label>Spelning</label>
                  <input
                    class="form-control"
                    type="time"
                    name="Starts"
                    v-model="upcomingEvent.startsTime"
                    required
                  />
                </div>

                <div class="col-sm-6" v-if="repFika && eventType !== 'Fikarep'">
                  <label>Fika</label>
                  <select
                    class="form-control"
                    v-model="upcomingEvent.fika"
                    name="Fika"
                  >
                    <option value>Välj en sektion</option>
                    <option v-for="s in SEKTIONER" :key="s">{{ s }}</option>
                  </select>
                </div>
              </div>
              <div class="row" v-if="eventType === 'Spelning'">
                <div class="col-sm-6">
                  <label>Total speltid</label>
                  <input
                    class="form-control"
                    v-model="upcomingEvent.playDuration"
                    name="Duration"
                  />
                </div>
              </div>
            </div>
            <template
              v-if="eventType === 'Spelning' || eventType === 'Evenemang'"
            >
              <div class="form-group">
                <label>Beskrivning</label>
                <textarea
                  class="form-control"
                  v-model="upcomingEvent.description"
                  name="Description"
                ></textarea>
              </div>
              <div class="form-group">
                <label>Engelsk beskrivning</label>
                <textarea
                  class="form-control"
                  v-model="upcomingEvent.descriptionEng"
                  name="Description"
                ></textarea>
              </div>
            </template>
            <div class="form-group">
              <label>Intern beskrivning</label>
              <textarea
                class="form-control"
                v-model="upcomingEvent.internalDescription"
                name="InternalDescription"
              ></textarea>
            </div>
            <div class="form-group">
              <label>Intern beskrivning på engelska</label>
              <textarea
                class="form-control"
                v-model="upcomingEvent.internalDescriptionEng"
                name="InternalDescription"
              ></textarea>
            </div>
          </div>
        </div>
        <div class="modal-footer">
          <button
            v-if="
              (upcomingEvent.description ||
                upcomingEvent.internalDescription) &&
              !loadingDescTrans &&
              !loadingIntDescTrans
            "
            class="btn btn-default translate-event-btn"
            @click.prevent="translateDescs"
          >
            Översätt beskrivningar med ChatGpt
          </button>
          <spinner
            class="translate-event-spinner"
            size="medium"
            v-if="loadingDescTrans || loadingIntDescTrans"
          ></spinner>
          <button class="btn btn-default" @click.prevent="close">Stäng</button>
          <button type="submit" class="btn btn-primary">Spara</button>
        </div>
      </form>
    </template>
  </modal>
</template>
<script setup lang="ts">
import { EVENTTYPES, SPELTYPER, SEKTIONER } from "../../constants";
import Datepicker from "vue3-datepicker";
import { postToApi } from "../../services/apiservice";
import Modal from "../Modal.vue";
import Spinner from "../Spinner.vue";
import { UpcomingEvent } from "../Upcoming/models";
import { computed, onMounted, ref, watch } from "vue";

const emit = defineEmits<{
  (e: "close"): void;
  (e: "update"): void;
}>();

const props = defineProps<{
  selectedEvent: UpcomingEvent | null;
  old: boolean;
  showModal: boolean;
}>();

const eventType = ref("");
const upcomingEvent = ref<UpcomingEvent | null>(null);
const error = ref<HTMLElement | null>(null);
const loadingDescTrans = ref(false);
const loadingIntDescTrans = ref(false);

const close = () => {
  emit("close");
};

const translateDescs = () => {
  if (upcomingEvent.value?.description) {
    loadingDescTrans.value = true;
    postToApi(
      "/ExtraInfo/TranslateText",
      {
        text: upcomingEvent.value.description,
      },
      null,
      null,
      (response) => {
        if (upcomingEvent.value) {
          upcomingEvent.value.descriptionEng = response.data;
        }
        loadingDescTrans.value = false;
      },
    );
  }
  if (upcomingEvent.value?.internalDescription) {
    loadingIntDescTrans.value = true;
    postToApi(
      "/ExtraInfo/TranslateText",
      {
        text: upcomingEvent.value.internalDescription,
      },
      null,
      null,
      (response) => {
        if (upcomingEvent.value) {
          upcomingEvent.value.internalDescriptionEng = response.data;
        }
        loadingIntDescTrans.value = false;
      },
    );
  }
};

const formSubmit = () => {
  const success = document.getElementsByClassName(
    "alert-success",
  )[0] as HTMLElement;
  const eventValue = upcomingEvent.value!;

  eventValue.type = eventType.value;
  eventValue.day = new Date(eventValue.dayDate).toUTCString();
  postToApi("/AdminEvent/Edit", eventValue, error.value, success, () => {
    emit("update");
  });
};

const resetEvent = () => {
  const today = new Date();
  eventType.value = props.selectedEvent ? props.selectedEvent.type : "";
  upcomingEvent.value = props.selectedEvent
    ? Object.assign({}, props.selectedEvent)
    : {
        id: -1,
        day: "",
        dayInMonth: 1,
        signupState: "",
        coming: 0,
        notComing: 0,
        type: "",
        name: "",
        place: "",
        description: "",
        descriptionEng: "",
        internalDescription: "",
        internalDescriptionEng: "",
        year: today.getFullYear(),
        month: today.getMonth() + 1,
        dayDate: today,
        fika: "",
        halanTime: "00:00",
        thereTime: "00:00",
        startsTime: "00:00",
        playDuration: "",
        stand: "",
        secret: false,
      };
  upcomingEvent.value!.dayDate = new Date(upcomingEvent.value!.dayDate);
};

watch(
  () => props.selectedEvent,
  () => {
    if (props.showModal) {
      resetEvent();
    }
  },
);

const eventId = computed(() => {
  if (!props.selectedEvent) {
    return 0;
  }
  return props.selectedEvent.id;
});

const spelningFest = computed(() => {
  return eventType.value === "Spelning" || eventType.value === "Fest";
});

const spelningKarhus = computed(() => {
  return (
    eventType.value === "Spelning" ||
    eventType.value === "Kårhusrep" ||
    eventType.value === "Athenrep"
  );
});

const repFika = computed(() => {
  return (
    eventType.value === "Rep" ||
    eventType.value === "Kårhusrep" ||
    eventType.value === "Athenrep" ||
    eventType.value === "Fikarep"
  );
});

onMounted(() => {
  resetEvent();
});
</script>
<style lang="scss">
.v3dp__datepicker .form-control[readonly] {
  background-color: #fff;
}
.translate-event-btn {
  float: left;
}
.translate-event-spinner {
  float: left;
}
</style>
