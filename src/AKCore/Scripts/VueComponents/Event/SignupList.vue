<template>
  <div>
    <div id="signup-list">
      <p class="can-come">
        {{ t("coming", "common") }}: {{ coming.length }} -
        {{ t("not-coming", "common") }}: {{ notComing.length }}
      </p>
      <h2>{{ t("coming", "common") }}</h2>

      <div class="row signup-row" :key="signup.id" v-for="signup in coming">
        <div class="col-sm-2 signup-element">
          <p>{{ cleanName(signup.personName) }}</p>
        </div>
        <div class="col-sm-2 signup-element">
          <p>{{ signup.instrumentName }}{{ otherInstrumentsList(signup) }}</p>
        </div>
        <div class="col-sm-2 signup-element">
          <p>{{ getInfo(signup) }}</p>
        </div>
        <div class="col-sm-3 signup-element" v-if="nintendo">
          <p>{{ signup.comment }}</p>
        </div>
        <div class="col-sm-3 signup-element" v-if="nintendo">
          <p>{{ formatDate(signup.signupTime) }}</p>
        </div>
        <div class="col-sm-6 signup-element" v-if="!nintendo">
          <p>{{ signup.comment }}</p>
        </div>
      </div>
      <h2 class="no-print">{{ t("not-coming", "common") }}</h2>
      <div
        class="row signup-row no-print"
        :key="signup.id"
        v-for="signup in notComing"
      >
        <div class="col-sm-3 signup-element">
          <p>{{ cleanName(signup.personName) }}</p>
        </div>
        <div class="col-sm-3 signup-element">
          <p>{{ signup.instrumentName }}</p>
        </div>
        <div class="col-sm-3 signup-element" v-if="nintendo">
          <p>{{ signup.comment }}</p>
        </div>
        <div class="col-sm-3 signup-element" v-if="nintendo">
          <p>{{ formatDate(signup.signupTime) }}</p>
        </div>
        <div class="col-sm-6 signup-element" v-if="!nintendo">
          <p>{{ signup.comment }}</p>
        </div>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { toRefs, computed } from "vue";
import { formatDate } from "@utils/functions";
import { UpcomingSignup } from "@components/Upcoming/models";
import { TranslationDomain, translate } from "@scripts/translations";

const props = defineProps<{
  nintendo: boolean;
  signups: UpcomingSignup[];
}>();

const { signups } = toRefs(props);

const coming = computed(() => {
  if (!signups.value) {
    return [];
  }
  return signups.value.filter((signup) => {
    return signup.where !== "Kan inte komma";
  });
});

const notComing = computed(() => {
  if (!signups.value) {
    return [];
  }
  return signups.value.filter((signup) => {
    return signup.where === "Kan inte komma";
  });
});

const otherInstrumentsList = (signup: UpcomingSignup) => {
  if (signup.otherInstruments) {
    return ", " + signup.otherInstruments.replace(",", ", ");
  }
  return "";
};

const getInfo = (signup: UpcomingSignup) => {
  let info = signup.where;
  if (!signup.instrument) {
    info += ", " + t("need-instrument-transport").toLowerCase();
  }
  if (signup.car) {
    info += ", " + t("has-car").toLowerCase();
  }
  return info;
};

const cleanName = (name: string) => {
  return name.replace(/\\/g, "");
};
const t = (key: string, domain: TranslationDomain = "signup") => {
  return translate(domain, key);
};
</script>
<style lang="scss"></style>
