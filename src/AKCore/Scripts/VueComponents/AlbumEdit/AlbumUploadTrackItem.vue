<template>
  <div class="track">
    <span class="name" v-if="!showEditName" @click="nameClick">{{ name }}</span>
    <input
      class="name-input"
      ref="inputelement"
      v-if="showEditName"
      v-model="editName"
      @keyup.enter="onInputBlur"
      @blur="onInputBlur"
    />
    <a class="rem-track" href="#" @click.prevent="remove">x</a>
  </div>
</template>
<script>
import ApiService from "../../services/apiservice";

export default {
  props: ["track"],
  data() {
    return {
      showEditName: false,
      editName: ""
    };
  },
  computed: {
    name() {
      if (this.track.name) {
        return this.track.name;
      }
      const nameParts = this.track.fileName.split(".");
      return nameParts[nameParts.length - 2].replace(/_/g, " ");
    }
  },
  methods: {
    remove() {
      this.$emit("remove", this.track.id);
    },
    nameClick() {
      this.showEditName = true;
      this.$nextTick(() => this.$refs.inputelement.focus());
    },
    onInputBlur() {
      this.showEditName = false;
      ApiService.postByObjectAsForm(
        "/AlbumEdit/ChangeTrackName",
        { id: this.track.id, name: this.editName },
        null,
        null,
        () => {
          this.$emit("update");
        }
      );
    }
  },
  created() {
    this.editName = this.name;
  }
};
</script>
<style lang="scss" scoped>
.track {
  .rem-track {
    float: right;
  }
}
</style>
