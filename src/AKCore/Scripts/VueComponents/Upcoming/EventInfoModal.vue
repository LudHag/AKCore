<template>
  <modal :show-modal="event" :header="header" @close="close">
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
<script>
import Modal from "../Modal.vue";
export default {
  props: ["event", "member"],
  components: {
    Modal,
  },
  methods: {
    close() {
      this.$emit("close");
    },
    openSignup() {
      this.$emit("signup", this.event.id);
    },
  },
  computed: {
    signupUrl() {
      return "/upcoming/Event/" + this.event.id;
    },
    signupable() {
      return (
        this.member &&
        (this.event.type === "Spelning" ||
          this.event.type === "Kårhusrep" ||
          this.event.type === "Athenrep")
      );
    },
    header() {
      if (!event) {
        return "";
      }
      return event.name;
    },
  },
};
</script>
<style lang="scss" scoped>
.green {
  color: #02c66f;
}
</style>
