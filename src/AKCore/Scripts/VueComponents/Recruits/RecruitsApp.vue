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
<script setup lang="ts">
import Modal from "../Modal.vue";
import RecruitsHeader from "./RecruitsHeader.vue";
import RecruitsList from "./RecruitsList.vue";
import Constants from "../../constants";
import { computed, onMounted, ref } from "vue";
import { Recruit } from "./models";

const searchText = ref("");
const instrument = ref("");
const archived = ref(false);
const recruits = ref<Recruit[]>([]);
const showModal = ref(false);

const instruments = computed(() => Constants.INSTRUMENTS);

const filteredRecruits = computed(() => {
  return recruits.value.filter((recruit) => {
    return (
      recruit.archived === archived.value &&
      hasSearchterm(recruit) &&
      hasInstrument(recruit)
    );
  });
});

const exportedText = computed(() => {
  return filteredRecruits.value
    .map((recruit) => {
      return `${recruit.fname}\t${recruit.lname}\t${recruit.instrument}\t${recruit.email}\t${recruit.phone}`;
    })
    .join("\n");
});

const hasInstrument = (recruit: Recruit) => {
  return instrument.value === "" || recruit.instrument === instrument.value;
};

const hasSearchterm = (recruit: Recruit) => {
  return (
    searchText.value === "" ||
    recruit.fname.toLowerCase().includes(searchText.value.toLowerCase()) ||
    recruit.lname.toLowerCase().includes(searchText.value.toLowerCase())
  );
};

const close = () => {
  showModal.value = false;
};

const updateRecruit = ({ id, arch }: { id: number; arch: boolean }) => {
  recruits.value = recruits.value.map((recruit) => {
    if (recruit.id === id) {
      recruit.archived = arch;
    }
    return recruit;
  });
};

const removeRecruit = (id: number) => {
  recruits.value = recruits.value.filter((recruit) => {
    return recruit.id !== id;
  });
};

onMounted(() => {
  //@ts-ignore
  // eslint-disable-next-line no-undef
  recruits.value = recruitList;
});
</script>
<style lang="scss" scoped></style>
