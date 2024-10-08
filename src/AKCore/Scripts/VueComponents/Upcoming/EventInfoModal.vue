<template>
  <modal :show-modal="!!event" :header="header" @close="close">
    <template #body>
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
              {{ t("gather-in-hole", "common") }}: {{ event.halanTime }}
            </p>
            <p class="modal-there" v-if="event.thereTime">
              {{ t("gather-there", "common") }}: {{ event.thereTime }}
            </p>
            <p class="modal-start" v-if="event.startsTime">
              {{ t("concert-starts", "common") }}: {{ event.startsTime }}
            </p>
            <p v-if="event.playDuration && event.type === 'Spelning'">
              {{ t("play-duration", "common") }}: {{ event.playDuration }}
            </p>
          </div>
          <div class="col-sm-4">
            <a
              class="green"
              v-if="signupable && event.signupState && !event.disabled"
              @click.prevent.stop="openSignup"
              :href="signupUrl"
            >
              {{ t("signed-up") }} ({{ event.signupState }})
            </a>
            <a
              v-if="signupable && !event.signupState && !event.disabled"
              @click.prevent.stop="openSignup"
              :href="signupUrl"
            >
              {{ t("sign-up") }}
            </a>
            <a
              v-if="signupable && event.disabled"
              @click.prevent.stop="openSignup"
              :href="signupUrl"
            >
              {{ t("about-event") }}
            </a>
            <div v-if="event.disabled">
              <p>
                <span class="glyphicon glyphicon-warning-sign"></span>
                <span class="warning-text">{{
                  t("sign-up-not-allowed", "common")
                }}</span>
              </p>
            </div>
            <p class="modal-comming" v-if="signupable">
              {{ event.coming }} {{ t("coming", "common") }} -
              {{ event.notComing }}
              {{ t("not-coming", "common") }}
            </p>
            <p class="modal-stand" v-if="event.stand">
              {{ t("type-of-play") }}: {{ event.stand }}
            </p>
            <p class="modal-fika" v-if="event.fika">
              {{ t("fika-and-clean") }}: {{ event.fika }}
            </p>
          </div>
          <div class="extra">
            <div class="col-sm-12">
              <p
                class="modal-description"
                v-if="
                  event.description &&
                  (event.type === 'Spelning' || event.type === 'Evenemang')
                "
              >
                {{ event.description }}
              </p>
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
import { TranslationDomain, translate } from "../../translations";
import { eventIsRep } from "./functions";

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

const eventName = (e: UpcomingEvent) => {
  if (eventIsRep(e)) {
    return t(e.type);
  }
  return e.name;
};

const header = computed(() => {
  if (!props.event) {
    return "";
  }
  return eventName(props.event);
});

const t = (key: string, domain: TranslationDomain = "upcoming") => {
  return translate(domain, key);
};
</script>
<style lang="scss" scoped>
@import "../../../Styles/variables.scss";
.green {
  color: #02c66f;
}
.glyphicon-warning-sign {
  color: $akred;
}
.warning-text {
  margin-left: 8px;
}
</style>
