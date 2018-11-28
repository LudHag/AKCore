<template>
    <div class="track">
        <span class="name">{{name}}</span>
        <a class="rem-track" href="#" @click.prevent="remove">x</a>
    </div>
</template>
<script>

export default {
  props: ["track"],
  computed: {
    name() {
      if(this.track.name) {
        return this.track.name;
      }
      const nameParts = this.track.fileName.split(".");
      return nameParts[nameParts.length - 2].replace(/_/g, " ");
    }
  },
  methods: {
    remove() {
      this.$emit("remove", this.track.id);
    }
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