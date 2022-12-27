<template>
  <div>
    <recruits-header
      :searchText="searchText"
      :archived="archived"
      @searchchange="searchText = $event"
      @archivechange="archived = $event"
    ></recruits-header>
    <recruits-list
      :recruits="filteredRecruits"
      @update="updateRecruit"
      @remove="removeRecruit"
    ></recruits-list>
  </div>
</template>
<script>
import Modal from "../Modal.vue";
import RecruitsHeader from "./RecruitsHeader.vue";
import RecruitsList from "./RecruitsList.vue";
import Constants from "../../constants";
import ApiService from "../../services/apiservice";

export default {
  data: function () {
    return {
      searchText: "",
      archived: false,
      recruits: [],
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
        return recruit.archived === this.archived;
      });
    },
  },
  methods: {
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
