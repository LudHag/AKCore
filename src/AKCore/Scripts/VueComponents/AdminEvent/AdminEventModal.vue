<template>
  <modal :show-modal="showModal" header="'Redigera händelse'" @close="close">
    <form slot="body" action="/AdminEvent/Edit" method="post" @submit.prevent="formSubmit">
      <div class="modal-body">
        <div class="alert alert-danger" style="display: none;"></div>
        <input type="hidden" name="Id" :value="eventId">
        <div class="form-group">
          <label>Typ</label>
          <select class="form-control" name="Type" v-model="eventType" required>
            <option value>Typ av händelse</option>
            <option v-for="e in eventTypes" :key="e">{{e}}</option>
          </select>
        </div>
        <div class="editeventbody" v-if="eventType">
          <div class="form-group" v-if="spelningFest">
            <div class="row">
              <div class="col-sm-6">
                <label>Namn</label>
                <input class="form-control" v-model="event.name" name="Name">
              </div>
              <div class="col-sm-6" v-if="eventType === 'Spelning'">
                <label></label>
                <div class="checkbox checkbox-center">
                  <label>
                    <input type="checkbox" v-model="event.secret" name="Secret"> Hemlig spelning
                  </label>
                </div>
              </div>
            </div>
          </div>
          <div class="form-group" v-if="spelningFest">
            <div class="row">
              <div class="col-sm-6">
                <label>Plats</label>
                <input class="form-control" v-model="event.place" name="Place">
              </div>
              <div class="col-sm-6" v-if="eventType === 'Spelning'">
                <label>Stå- eller gåspelning</label>
                <select class="form-control" v-model="event.stand" name="Stand" required>
                  <option value>Välj speltyp</option>
                  <option v-for="type in spelTyper" :key="type">{{type}}</option>
                </select>
              </div>
            </div>
          </div>
          <div class="form-group">
            <div class="row">
              <div class="col-sm-3">
                <label>Dag</label>
                <datepicker input-class="form-control" v-model="event.dayDate" name="Day" required></datepicker>
              </div>
              <div class="col-sm-3">
                <label>Vid hålan</label>
                <input
                  class="form-control"
                  type="time"
                  v-model="event.halanTime"
                  name="Halan"
                  value="00:00"
                >
              </div>
              <div class="col-sm-3" v-if="spelningKarhus">
                <label>På plats</label>
                <input
                  class="form-control"
                  type="time"
                  v-model="event.thereTime"
                  name="There"
                  value="00:00"
                >
              </div>
              <div class="col-sm-3" v-if="eventType === 'Spelning'">
                <label>Spelning</label>
                <input
                  class="form-control"
                  type="time"
                  name="Starts"
                  v-model="event.startsTime"
                  value="00:00"
                  required
                >
              </div>
              <div class="col-sm-6" v-if="repFika && eventType !== 'Fikarep'">
                <label>Fika</label>
                <select class="form-control" v-model="event.fika" name="Fika">
                  <option value>Välj en sektion</option>
                  <option v-for="s in sektioner" :key="s">{{s}}</option>
                </select>
              </div>
            </div>
          </div>

          <div class="form-group" v-if="eventType === 'Spelning'">
            <label>Beskrivning</label>
            <textarea class="form-control" v-model="event.description" name="Description"></textarea>
          </div>
          <div class="form-group">
            <label>Intern beskrivning</label>
            <textarea
              class="form-control"
              v-model="event.internalDescription"
              name="InternalDescription"
            ></textarea>
          </div>
        </div>
      </div>
      <div class="modal-footer">
        <button class="btn btn-default" @click.prevent="close">Stäng</button>
        <button v-if="!old || !eventId" type="submit" class="btn btn-primary">Spara</button>
      </div>
    </form>
  </modal>
</template>
<script>
import Constants from "../../constants";
import Datepicker from "vuejs-datepicker";
import ApiService from "../../services/apiservice";
import Modal from "../Modal";

export default {
  props: ["selectedEvent", "old", "showModal"],
  components: {
    Datepicker,
    Modal
  },
  data() {
    return {
      eventType: "",
      event: null
    };
  },
  methods: {
    close() {
      this.$emit("close");
    },
    formSubmit(event) {
      if (this.old && this.eventId) {
        return;
      }
      const error = $(event.target).find(".alert-danger");
      const success = $(".alert-success");
      const self = this;
      this.event.type = this.eventType;
      this.event.day = this.event.dayDate.toUTCString();
      ApiService.postByObject(
        "/AdminEvent/Edit",
        this.event,
        error,
        success,
        () => {
          self.$emit("update");
        }
      );
    },
    resetEvent() {
      const today = new Date();
      this.eventType = this.selectedEvent ? this.selectedEvent.type : "";
      this.event = this.selectedEvent
        ? Object.assign({}, this.selectedEvent)
        : {
            type: "",
            name: "",
            place: "",
            description: "",
            internalDescription: "",
            year: today.getFullYear(),
            month: today.getMonth() + 1,
            dayDate: today,
            fika: "",
            halanTime: "00:00",
            thereTime: "00:00",
            startsTime: "00:00",
            stand: "",
            secret: false
          };
      this.event.dayDate = new Date(this.event.dayDate);
    }
  },
  watch: {
    selectedEvent() {
      if (this.showModal) {
          this.resetEvent();
      }
    }
  },
  computed: {
    eventId() {
      if (!this.selectedEvent) {
        return 0;
      }
      return this.selectedEvent.id;
    },
    eventTypes() {
      return Constants.EVENTTYPES;
    },
    spelTyper() {
      return Constants.SPELTYPER;
    },
    sektioner() {
      return Constants.SEKTIONER;
    },
    spelningFest() {
      return this.eventType === "Spelning" || this.eventType === "Fest";
    },
    spelningKarhus() {
      return this.eventType === "Spelning" || this.eventType === "Kårhusrep";
    },
    repFika() {
      return (
        this.eventType === "Rep" ||
        this.eventType === "Kårhusrep" ||
        this.eventType === "Fikarep"
      );
    }
  },
  created() {
    this.resetEvent();
  }
};
</script>
<style lang="scss">
.vdp-datepicker .form-control[readonly] {
  background-color: #fff;
}
</style>
