<template>
  <div>
    <div id="signup-list">
      <p class="can-come">
        Kommer: {{ coming.length }} - Kommer inte: {{ notComing.length }}
      </p>
      <h2>Kommer</h2>

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
      <h2>Kommer inte</h2>
      <div class="row signup-row" :key="signup.id" v-for="signup in notComing">
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
import { formatDate } from "../../utils/functions";
import { UpcomingSignup } from "../Upcoming/models";

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
  if (signup.instrument) {
    info += ", har instrument";
  }
  if (signup.car) {
    info += ", har bil";
  }
  return info;
};

const cleanName = (name: string) => {
  return name.replace(/\\/g, "");
};
</script>
<style lang="scss"></style>
