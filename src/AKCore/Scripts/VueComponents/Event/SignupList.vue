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
          <p>{{ formatDateMethod(signup.signupTime) }}</p>
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
          <p>{{ formatDateMethod(signup.signupTime) }}</p>
        </div>
        <div class="col-sm-6 signup-element" v-if="!nintendo">
          <p>{{ signup.comment }}</p>
        </div>
      </div>
    </div>
  </div>
</template>
<script>
import { formatDate } from "../../utils/functions";
export default {
  props: ["signups", "nintendo"],
  computed: {
    coming() {
      if (!this.signups) {
        return [];
      }
      return this.signups.filter((signup) => {
        return signup.where !== "Kan inte komma";
      });
    },
    notComing() {
      if (!this.signups) {
        return [];
      }
      return this.signups.filter((signup) => {
        return signup.where === "Kan inte komma";
      });
    },
  },
  methods: {
    formatDateMethod(date) {
      return formatDate(date);
    },
    otherInstrumentsList(signup) {
      if (signup.otherInstruments) {
        return ", " + signup.otherInstruments.replace(",", ", ");
      }
      return "";
    },
    getInfo(signup) {
      let info = signup.where;
      if (signup.instrument) {
        info += ", har instrument";
      }
      if (signup.car) {
        info += ", har bil";
      }
      return info;
    },
    cleanName(name) {
      return name.replace(/\\/g, "");
    },
  },
};
</script>
<style lang="scss"></style>
