<template>
  <div>
    <div class="col-sm-6">
      <textarea class="mce-content" v-html="value" @input="update"></textarea>
    </div>
  </div>
</template>
<script>
export default {
  props: ["value"],
  methods: {
    update(event) {
      this.$emit("input", event.target.value);
    }
  }
};
</script>
<style lang="scss"></style>
