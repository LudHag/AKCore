<template>
  <div
    class="row event-row"
    @click.prevent="expanded = !expanded"
    @keydown.prevent.enter="expanded = !expanded"
    @keydown.prevent.space="expanded = !expanded"
    :class="{ expandable, expanded }"
    :tabindex="expandable ? 0 : -1"
  >
    <div class="col-sm-4 col-xs-6" style="font-weight: 500">
      <p style="text-transform: capitalize">{{ event.day }}</p>
      <p v-if="isRep">{{ t(event.type) }}</p>
      <p v-if="event.type === 'Rep' || event.type === 'Samlingsrep'">
        {{ event.place }}
      </p>
      <template v-if="!isRep && loggedIn">
        <p>{{ event.name }}</p>
        <p>{{ event.place }}</p>
      </template>
    </div>
    <div class="col-sm-4 col-xs-6">
      <template v-if="loggedIn">
        <p v-if="event.halanTime">
          <span v-if="event.type === 'Balettrep'">
            {{ t("at-rehersal-place", "common") }}
          </span>
          <span v-else> {{ t("gather-in-hole", "common") }} </span>:
          {{ event.halanTime }}
        </p>
        <p
          v-if="
            event.thereTime &&
            (event.type === 'Spelning' ||
              event.type === 'Kårhusrep' ||
              event.type === 'Athenrep' ||
              event.type === 'Samlingsrep')
          "
        >
          {{ t("gather-there", "common") }}: {{ event.thereTime }}
        </p>
        <p v-if="event.startsTime && event.type === 'Spelning'">
          {{ t("concert-starts", "common") }}: {{ event.startsTime }}
        </p>
        <p v-if="event.playDuration && event.type === 'Spelning'">
          {{ t("play-duration", "common") }}: {{ event.playDuration }}
        </p>
      </template>
      <template v-if="!loggedIn">
        <p>{{ event.name }}</p>
        <p>{{ event.place }}</p>
      </template>
    </div>
    <div class="col-sm-4 col-xs-12">
      <template v-if="!loggedIn">
        <p v-if="event.startsTime && !loggedIn">
          {{ t("concert-starts", "common") }}: {{ event.startsTime }}
        </p>
      </template>
      <template
        v-if="
          loggedIn &&
          (event.type === 'Spelning' ||
            event.type === 'Kårhusrep' ||
            event.type === 'Athenrep' ||
            event.type === 'Samlingsrep')
        "
      >
        <a
          class="green signup-link"
          v-if="member && event.signupState"
          @click.prevent.stop="openSignup"
          @keydown.prevent.enter="openSignup"
          @keydown.prevent.space="openSignup"
          :href="signupUrl"
        >
          {{ t("signed-up") }} ({{ event.signupState }})
        </a>
        <a
          class="signup-link"
          v-if="member && !event.signupState"
          @click.prevent.stop="openSignup"
          @keydown.prevent.enter="openSignup"
          @keydown.prevent.space="openSignup"
          :href="signupUrl"
        >
          {{ event.disabled ? t("about-event") : t("sign-up") }}
        </a>
        <p class="hidden-xs">
          {{ event.coming }} {{ t("coming", "common") }} -
          {{ event.notComing }}
          {{ t("not-coming", "common") }}
        </p>
      </template>
      <div v-if="event.disabled">
        <p>
          <span
            class="glyphicon glyphicon-warning-sign event-disabled-warning"
          ></span>
          <span class="warning-text">{{
            t("sign-up-not-allowed", "common")
          }}</span>
        </p>
      </div>
      <p v-if="loggedIn && event.type === 'Spelning' && event.stand">
        {{ t("type-of-play") }}: {{ event.stand }}
      </p>
      <div
        v-if="
          loggedIn &&
          (event.type === 'Rep' ||
            event.type === 'Kårhusrep' ||
            event.type === 'Athenrep' ||
            event.type === 'Samlingsrep')
        "
      >
        <div>
          {{ t("fika-and-clean") }}:
          <span v-for="(item, index) in event.fikaCollection" :key="index">
            {{ item
            }}<span
              v-if="
                event.fikaCollection.length > 1 &&
                index !== event.fikaCollection.length - 1
              "
              >,
            </span>
          </span>
        </div>
      </div>
    </div>
    <div class="extra">
      <div
        class="col-sm-12 description"
        v-if="
          event.description &&
          (event.type === 'Spelning' || event.type === 'Evenemang')
        "
      >
        <p>{{ event.description }}</p>
      </div>
      <div class="col-xs-12" v-if="event.internalDescription">
        <p>{{ event.internalDescription }}</p>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { computed, ref } from "vue";
import { UpcomingEvent } from "./models";
import { TranslationDomain, translate } from "@scripts/translations";
import { eventIsRep } from "./functions";

const emit = defineEmits<{
  (e: "signup", id: number): void;
}>();

const props = defineProps<{
  event: UpcomingEvent;
  loggedIn: boolean;
  member: boolean;
}>();

const expanded = ref(false);

const openSignup = () => {
  emit("signup", props.event.id);
};

const expandable = computed(() => {
  return props.event.description || props.event.internalDescription;
});

const isRep = computed(() => {
  return eventIsRep(props.event);
});

const signupUrl = computed(() => {
  return "/upcoming/Event/" + props.event.id;
});
const t = (key: string, domain: TranslationDomain = "upcoming") => {
  return translate(domain, key);
};
</script>
<style lang="scss" scoped>
@import "../../../Styles/variables.scss";

.event-row.expandable a {
  margin: 0 0 10px;
  display: block;
}

.event-row:focus {
  outline: 1px solid #fff;
  outline-offset: 5px;
}

.event-row {
  padding: 15px;
  border: 3px solid $akred;
  border-radius: 7px;
  position: relative;
}
.event-row:hover {
  background-color: #1a0000;
}

.event-row .green {
  color: #02c66f;
}

.event-disabled-warning {
  color: $akred;
}

.event-row .warning-text {
  margin-left: 8px;
}

@media screen and (max-width: 768px) {
  .event-row a.signup-link {
    display: inline-block;
  }

  .event-row.expandable:before {
    content: "+";
    color: $akred;
    position: absolute;
    right: 6px;
    top: 3px;
    font-size: 25px;
    line-height: 25px;
  }

  .event-row.expandable .extra {
    display: none;
  }

  .event-row.expanded .extra {
    display: block;
    float: left;

    .description {
      margin-top: 0;
    }
  }

  .event-row.expandable.expanded:before {
    content: "-";
    right: 6px;
    top: 0px;
    font-size: 50px;
  }
}
</style>
