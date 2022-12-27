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
      <div class="col-sm-3">{{ recruit.fname + " " + recruit.lname }}</div>
      <div class="col-sm-2">{{ recruit.instrument }}</div>
      <div class="col-sm-4 contact" v-html="geContactInfo(recruit)"></div>
      <div class="col-sm-1 actions">
        <a
          href="#"
          :title="recruit.archived ? 'Aktivera' : 'Arkivera'"
          class="archive glyphicon glyphicon-folder-open"
          :class="{ green: recruit.archived }"
          @click="archive(recruit)"
        ></a>
        <a
          href="#"
          title="Ta bort"
          class="remove glyphicon glyphicon-remove"
          @click="remove(recruit)"
        ></a>
      </div>
    </div>
  </div>
</template>
<script>
import ApiService from "../../services/apiservice";

export default {
  props: ["recruits"],
  methods: {
    getDate(recruit) {
      return (
        recruit.created.getFullYear() +
        "-" +
        ("0" + (recruit.created.getMonth() + 1)).slice(-2) +
        "-" +
        ("0" + recruit.created.getDate()).slice(-2)
      );
    },
    geContactInfo(recruit) {
      return (
        recruit.email +
        (recruit.email.length > 1 ? "<br />" : "") +
        recruit.phone
      );
    },
    archive(recruit) {
      console.log("archive");
    },
    remove(recruit) {
      console.log("remove");
    },
  },
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
