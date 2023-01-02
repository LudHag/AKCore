<template>
  <div>
    <recruits-header
      :archived="archived"
      v-model="instrument"
      @searchchange="searchText = $event"
      @archivechange="archived = $event"
      @instrumentchange="instrument = $event"
      @export="showModal = true"
    ></recruits-header>
    <recruits-list
      :recruits="filteredRecruits"
      @update="updateRecruit"
      @remove="removeRecruit"
    ></recruits-list>
    <modal :show-modal="showModal" header="Export" @close="close">
      <template v-slot:body>
        <div>
          <div class="modal-body">
            <textarea
              class="form-control"
              rows="5"
              v-html="exportedText"
            ></textarea>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-default" @click="close">
              Stäng
            </button>
          </div>
        </div>
      </template>
    </modal>
  </div>
</template>
<script>
import Modal from "../Modal.vue";
import RecruitsHeader from "./RecruitsHeader.vue";
import RecruitsList from "./RecruitsList.vue";
import Constants from "../../constants";

export default {
  data: function () {
    return {
      searchText: "",
      instrument: "",
      archived: false,
      recruits: [],
      showModal: false,
    };
  },
  components: {
    Modal,
    RecruitsHeader,
    RecruitsList,
  },
  computed: {
    instruments() {
      return Constants.INSTRUMENTS;
    },
    filteredRecruits() {
      return this.recruits.filter((recruit) => {
        return (
          recruit.archived === this.archived &&
          this.hasSearchterm(recruit) &&
          this.hasInstrument(recruit)
        );
      });
    },
    exportedText() {
      return this.filteredRecruits
        .map((recruit) => {
          return `${recruit.fname}\t${recruit.lname}\t${recruit.instrument}\t${recruit.email}\t${recruit.phone}`;
        })
        .join("\n");
    },
  },
  methods: {
    hasInstrument(recruit) {
      return this.instrument === "" || recruit.instrument === this.instrument;
    },
    hasSearchterm(recruit) {
      return (
        this.searchText === "" ||
        recruit.fname.toLowerCase().includes(this.searchText.toLowerCase()) ||
        recruit.lname.toLowerCase().includes(this.searchText.toLowerCase())
      );
    },
    close() {
      this.showModal = false;
    },
    updateRecruit({ id, arch }) {
      this.recruits = this.recruits.map((recruit) => {
        if (recruit.id === id) {
          recruit.archived = arch;
        }
        return recruit;
      });
    },
    removeRecruit(id) {
      this.recruits = this.recruits.filter((recruit) => {
        return recruit.id !== id;
      });
    },
  },
  created() {
    this.recruits = recruitList;
  },
};
</script>
<style lang="scss" scoped></style>
