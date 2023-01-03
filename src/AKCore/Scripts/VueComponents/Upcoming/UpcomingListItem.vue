<template>
  <div
    class="row event-row"
    @click.prevent="expanded = !expanded"
    v-bind:class="{ expandable, expanded }"
  >
    <div class="col-sm-4 col-xs-6" style="font-weight: 500">
      <p style="text-transform: capitalize">{{ event.day }}</p>
      <p v-if="isRep">{{ event.type }}</p>
      <p v-if="event.type === 'Rep'">{{ event.place }}</p>
      <template v-if="!isRep && loggedIn">
        <p>{{ event.name }}</p>
        <p>{{ event.place }}</p>
      </template>
    </div>
    <div class="col-sm-4 col-xs-6">
      <template v-if="loggedIn">
        <p v-if="event.halanTime">Samling i hålan: {{ event.halanTime }}</p>
        <p
          v-if="
            event.thereTime &&
            (event.type === 'Spelning' ||
              event.type === 'Kårhusrep' ||
              event.type === 'Athenrep')
          "
        >
          Samling på plats: {{ event.thereTime }}
        </p>
        <p v-if="event.startsTime && event.type === 'Spelning'">
          Spelning startar: {{ event.startsTime }}
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
          Spelning startar: {{ event.startsTime }}
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
          >Anmäld ({{ event.signupState }})</a
        >
        <a
          class="signup-link"
          v-if="member && !event.signupState"
          @click.prevent.stop="openSignup"
          :href="signupUrl"
          >Anmäl</a
        >
        <p class="hidden-xs">
          {{ event.coming }} Kommer - {{ event.notComing }} Kommer inte
        </p>
      </template>
      <p v-if="loggedIn && event.type === 'Spelning' && event.stand">
        Speltyp: {{ event.stand }}
      </p>
      <p
        v-if="
          loggedIn &&
          (event.type === 'Rep' ||
            event.type === 'Kårhusrep' ||
            event.type === 'Athenrep')
        "
      >
        Fika och städning: {{ event.fika }}
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
<script>
export default {
  props: ["event", "loggedIn", "member"],
  data() {
    return {
      expanded: false,
    };
  },
  methods: {
    openSignup() {
      this.$emit("signup", this.event.id);
    },
  },
  computed: {
    expandable() {
      return this.event.description || this.event.internalDescription;
    },
    isRep() {
      return (
        this.event.type === "Rep" ||
        this.event.type === "Kårhusrep" ||
        this.event.type === "Athenrep" ||
        this.event.type === "Fikarep"
      );
    },
    signupUrl() {
      return "/upcoming/Event/" + this.event.id;
    },
  },
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
