<template>
  <div>
    <div class="col-sm-6">
      <textarea class="mce-content" v-html="value.text"></textarea>
    </div>
    <div class="col-sm-6">
      <div class="picture-select">
        <img class="selected-image" :src="value.image" />
        <a class="btn btn-default choose-picture-btn">Välj bild</a>
      </div>
    </div>
  </div>
</template>
<script>
export default {
  props: ["value"]
};
</script>
<style lang="scss"></style>
