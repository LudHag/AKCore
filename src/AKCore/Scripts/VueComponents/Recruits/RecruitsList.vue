<template>
  <div>
    <div class="row recruit-head">
      <div class="col-sm-2">Skapad</div>
      <div class="col-sm-3">Namn</div>
      <div class="col-sm-2">Instrument</div>
      <div class="col-sm-4 contact">Kontaktinformation</div>
      <div class="col-sm-1 actions"></div>
    </div>
    <div
      class="row recruit hover-grey"
      v-for="recruit in recruits"
      :key="recruit.id"
    >
      <div class="col-sm-2">{{ getDate(recruit) }}</div>
      <div class="col-sm-3" v-html="recruit.fname + ' ' + recruit.lname"></div>
      <div class="col-sm-2" v-html="recruit.instrument"></div>
      <div class="col-sm-4 contact" v-html="geContactInfo(recruit)"></div>
      <div class="col-sm-1 actions">
        <a
          href="#"
          :title="recruit.archived ? 'Aktivera' : 'Arkivera'"
          class="archive glyphicon glyphicon-folder-open"
          :class="{ green: recruit.archived }"
          @click.prevent="archive(recruit)"
        ></a>
        <a
          href="#"
          title="Ta bort"
          class="remove glyphicon glyphicon-remove"
          @click.prevent="remove(recruit)"
        ></a>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { postToApi } from "../../services/apiservice";
import { GenericApiResponse } from "../../services/models";
import { Recruit, RecruitUpdate } from "./models";

const emit = defineEmits<{
  (e: "update", value: RecruitUpdate): void;
  (e: "remove", value: number): void;
}>();

defineProps<{
  recruits: Recruit[];
}>();

const getDate = (recruit: Recruit) => {
  return (
    recruit.created.getFullYear() +
    "-" +
    ("0" + (recruit.created.getMonth() + 1)).slice(-2) +
    "-" +
    ("0" + recruit.created.getDate()).slice(-2)
  );
};

const geContactInfo = (recruit: Recruit) => {
  return (
    recruit.email + (recruit.email.length > 1 ? "<br />" : "") + recruit.phone
  );
};

const archive = (recruit: Recruit) => {
  postToApi(
    "/Signup/Archive",
    { id: recruit.id, arch: !recruit.archived },
    null,
    null,
    (response: GenericApiResponse) => {
      if (response.success) {
        emit("update", { id: recruit.id, arch: !recruit.archived });
      }
    },
  );
};

const remove = (recruit: Recruit) => {
  if (
    !window.confirm("Är du säker att du vill ta bort den här intresseanmälan?")
  ) {
    return;
  }

  postToApi(
    "/Signup/Remove",
    { id: recruit.id },
    null,
    null,
    (response: GenericApiResponse) => {
      if (response.success) {
        emit("remove", recruit.id);
      }
    },
  );
};
</script>
<style lang="scss" scoped>
.recruit {
  margin-bottom: 0;
  padding: 15px 0;
}

.recruit-head {
  font-weight: 500;
}

.actions {
  .archive {
    &.green {
      color: #02c66f;
    }
  }

  .remove {
    margin-left: 15px;
  }
}
</style>
