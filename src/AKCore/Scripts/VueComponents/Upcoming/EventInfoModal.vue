<template>
  <modal :show-modal="!!event" :header="header" @close="close">
    <template v-slot:body>
      <div class="modal-body" v-if="event">
        <div class="row">
          <div class="col-sm-4" style="font-weight: 500">
            <p class="modal-day" style="text-transform: capitalize">
              {{ event.day }}
            </p>
            <p class="modal-place">{{ event.place }}</p>
          </div>
          <div class="col-sm-4">
            <p class="modal-halan" v-if="event.halanTime">
              Samling i hålan: {{ event.halanTime }}
            </p>
            <p class="modal-there" v-if="event.thereTime">
              Samling på plats: {{ event.thereTime }}
            </p>
            <p class="modal-start" v-if="event.startsTime">
              Spelning startar: {{ event.startsTime }}
            </p>
          </div>
          <div class="col-sm-4">
            <a
              class="green"
              v-if="signupable && event.signupState"
              @click.prevent.stop="openSignup"
              :href="signupUrl"
              >Anmäld ({{ event.signupState }})</a
            >
            <a
              v-if="signupable && !event.signupState"
              @click.prevent.stop="openSignup"
              :href="signupUrl"
              >Anmäl</a
            >
            <p class="modal-comming" v-if="signupable">
              {{ event.coming }} Kommer - {{ event.notComing }} Kommer inte
            </p>
            <p class="modal-stand" v-if="event.stand">
              Speltyp: {{ event.stand }}
            </p>
            <p class="modal-fika" v-if="event.fika">
              Fika och städning: {{ event.fika }}
            </p>
          </div>
          <div class="extra">
            <div class="col-sm-12">
              <p class="modal-description">{{ event.description }}</p>
            </div>
            <div class="col-sm-12">
              <p class="modal-intdescription">
                {{ event.internalDescription }}
              </p>
            </div>
          </div>
        </div>
      </div>
    </template>
  </modal>
</template>
<script setup lang="ts">
import { computed } from "vue";
import Modal from "../Modal.vue";
import { UpcomingEvent } from "./models";

const emit = defineEmits<{
  (e: "close"): void;
  (e: "signup", id: number): void;
}>();

const props = defineProps<{
  event: UpcomingEvent;
  member: boolean;
}>();

const close = () => emit("close");
const openSignup = () => emit("signup", props.event.id);

const signupUrl = "/upcoming/Event/" + props.event.id;

const signupable =
  props.member &&
  (props.event.type === "Spelning" ||
    props.event.type === "Kårhusrep" ||
    props.event.type === "Athenrep");

const header = computed(() => {
  if (!props.event) {
    return "";
  }
  return props.event.name;
});
</script>
<style lang="scss" scoped>
.green {
  color: #02c66f;
}
</style>
