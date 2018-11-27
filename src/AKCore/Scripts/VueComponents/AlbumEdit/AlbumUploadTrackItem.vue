<template>
    <div class="track">
        {{name}}
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
  }
};
</script>
<style lang="scss" scoped>
 
</style>