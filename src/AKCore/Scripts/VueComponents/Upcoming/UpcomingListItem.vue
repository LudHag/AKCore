<template>
  <div
    class="row event-row"
    @click.prevent="expanded = !expanded"
    :class="{ expandable, expanded }"
  >
    <div class="col-sm-4 col-xs-6" style="font-weight: 500">
      <p style="text-transform: capitalize">{{ event.day }}</p>
      <p v-if="isRep">{{ t(event.type) }}</p>
      <p v-if="event.type === 'Rep'">{{ event.place }}</p>
      <template v-if="!isRep && loggedIn">
        <p>{{ event.name }}</p>
        <p>{{ event.place }}</p>
      </template>
    </div>
    <div class="col-sm-4 col-xs-6">
      <template v-if="loggedIn">
        <p v-if="event.halanTime">
          {{ t("gather-in-hole", "common") }}: {{ event.halanTime }}
        </p>
        <p
          v-if="
            event.thereTime &&
            (event.type === 'Spelning' ||
              event.type === 'Kårhusrep' ||
              event.type === 'Athenrep')
          "
        >
          {{ t("gather-there", "common") }}: {{ event.thereTime }}
        </p>
        <p v-if="event.startsTime && event.type === 'Spelning'">
          {{ t("concert-starts", "common") }}: {{ event.startsTime }}
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
            event.type === 'Athenrep')
        "
      >
        <a
          class="green signup-link"
          v-if="member && event.signupState"
          @click.prevent.stop="openSignup"
          :href="signupUrl"
        >
          {{ t("signed-up") }} ({{ event.signupState }})
        </a>
        <a
          class="signup-link"
          v-if="member && !event.signupState"
          @click.prevent.stop="openSignup"
          :href="signupUrl"
        >
          {{ t("sign-up") }}
        </a>
        <p class="hidden-xs">
          {{ event.coming }} {{ t("coming", "common") }} -
          {{ event.notComing }}
          {{ t("not-coming", "common") }}
        </p>
      </template>
      <p v-if="loggedIn && event.type === 'Spelning' && event.stand">
        {{ t("type-of-play") }}: {{ event.stand }}
      </p>
      <p
        v-if="
          loggedIn &&
          (event.type === 'Rep' ||
            event.type === 'Kårhusrep' ||
            event.type === 'Athenrep')
        "
      >
        {{ t("fika-and-clean") }}: {{ event.fika }}
      </p>
    </div>
    <div class="extra">
      <div class="col-sm-12 description" v-if="event.description">
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
import { TranslationDomain, translate } from "../../translations";

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
  return (
    props.event.type === "Rep" ||
    props.event.type === "Kårhusrep" ||
    props.event.type === "Athenrep" ||
    props.event.type === "Fikarep"
  );
});

const signupUrl = computed(() => {
  return "/upcoming/Event/" + props.event.id;
});
const t = (key: string, domain: TranslationDomain = "upcoming") => {
  return translate(domain, key);
};
</script>
<style lang="scss">
@import "../../../Styles/variables.scss";

.event-row.expandable a {
  margin: 0 0 10px;
  display: block;
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
