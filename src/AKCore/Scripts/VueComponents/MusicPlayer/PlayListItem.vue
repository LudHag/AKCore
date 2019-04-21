<template>
  <a
    :href="track.filepath"
    class="playlist-element"
    :class="{'active': active, 'small': small}"
    @click.prevent="$emit('select', track)"
  >
    <span class="name" v-html="track.name"></span>
    <span class="glyphicon glyphicon-download"></span>
    <span
      class="glyphicon glyphicon-plus-sign"
      v-if="!noAdd"
      @click.prevent.stop="$emit('add', track)"
    ></span>
  </a>
</template>
<script>
export default {
  props: ["track", "active", "noAdd", "small"]
};
</script>
<style lang="scss" scoped>
@import "~bootstrap-sass/assets/stylesheets/bootstrap/_variables.scss";
@import "../../../Styles/variables.scss";
.playlist-element {
  display: block;
  color: #a5a2a0;
  padding: 3px 0;
  font-size: 16px;
  text-align: left;

  &.small {
    font-size: 14px;
  }

  &.queued {
    color: #2c7b12;
  }

  &.active {
    color: $akred;
  }
  .glyphicon-plus-sign {
    float: right;
    margin-right: 15px;
  }
  .glyphicon-download {
    float: right;
    margin-right: 30px;
  }
}
@media screen and (max-width: $screen-xs-max) {
  .playlist-element {
    position: relative;

    .name {
      width: 82%;
      display: inline-block;
    }

    .glyphicon-download {
      position: absolute;
      top: 50%;
      transform: translateY(-50%);
    }
  }
}
</style>
