<template>
  <a
    :href="track.filepath"
    class="playlist-element"
    :class="{'active': active, 'small': small}"
    @click.prevent="$emit('select', track)"
  >
    <span class="name" v-html="track.name"></span>
    <span class="glyphicon glyphicon-download" @click.prevent.stop="downloadUri"></span>
    <span
      class="glyphicon glyphicon-plus-sign"
      v-if="!noAdd && !remove"
      @click.prevent.stop="$emit('add', track)"
    ></span>
    <span
      class="glyphicon glyphicon-minus-sign"
      v-if="remove"
      @click.prevent.stop="$emit('remove', track)"
    ></span>
  </a>
</template>
<script>
export default {
  props: ["track", "active", "noAdd", "small", "remove"],
  methods: {
    downloadUri() {
      const link = document.createElement("a");
      const trackPathParts = this.track.filepath.split("/");
      link.download = trackPathParts[trackPathParts.length - 1];
      link.href = this.track.filepath;
      link.click();
    }
  }
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
  .glyphicon-download {
    float: right;
    margin-right: 30px;
  }

  &.small {
    font-size: 14px;
    .glyphicon-download {
      margin-right: 15px;
    }
  }

  &.queued {
    color: #2c7b12;
  }

  &.active {
    color: $akred;
  }
  .glyphicon-plus-sign,
  .glyphicon-minus-sign {
    float: right;
    margin-right: 15px;
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
