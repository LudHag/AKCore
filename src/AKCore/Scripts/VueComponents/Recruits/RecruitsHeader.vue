<template>
  <div class="row filters">
    <div class="col-lg-5">
      <h1>Intresseanmälningar</h1>
      <a @click="$emit('export')" class="btn btn-primary"> Exportera </a>
    </div>
    <div class="col-lg-7">
      <div class="form-inline interest-filters">
        <div class="form-group">
          <input
            class="toggle-switch"
            id="archive-flip"
            type="checkbox"
            v-model="localArchived"
            @change="$emit('archivechange', localArchived)"
          />
          <label
            class="toggle-switch-btn"
            for="archive-flip"
            data-tg-off="Aktiva"
            data-tg-on="Arkiverade"
          ></label>
        </div>
        <div class="form-group">
          <input
            type="text"
            class="form-control"
            placeholder="Sök här"
            v-model="localSearchText"
            @keyup="$emit('searchchange', localSearchText)"
          />
        </div>
        <div class="form-group">
          <select
            class="form-control"
            @change="$emit('instrumentchange', $event.target.value)"
          >
            <option value="">Sök efter instrument</option>
            <option v-for="instr in instruments" :key="instr">
              {{ instr }}
            </option>
          </select>
        </div>
      </div>
    </div>
  </div>
</template>
<script>
import Constants from "../../constants";

export default {
  data: function () {
    return {
      localSearchText: "",
      localArchived: false,
    };
  },
  props: ["archived"],
  computed: {
    instruments() {
      return Constants.INSTRUMENTS;
    },
  },
  created() {
    this.localArchived = this.archived;
  },
};
</script>
<style lang="scss" scoped></style>
