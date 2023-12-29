<template>
  <a
    :href="track.filepath"
    class="playlist-element"
    :class="{ active: active, small: small }"
    @click.prevent="$emit('select', track)"
  >
    <span class="name" v-html="track.name"></span>
    <span
      class="glyphicon glyphicon-download"
      @click.prevent.stop="downloadUri"
    ></span>
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
<script setup lang="ts">
import { Track } from "./models";

const props = defineProps<{
  track: Track;
  active?: boolean | null;
  noAdd?: boolean;
  small?: boolean;
  remove?: boolean;
}>();

const downloadUri = () => {
  const link = document.createElement("a");
  const trackPathParts = props.track.filepath.split("/");
  link.download = trackPathParts[trackPathParts.length - 1];
  link.href = props.track.filepath;
  link.click();
};
</script>
<style lang="scss" scoped>
@import "bootstrap-sass/assets/stylesheets/bootstrap/_variables.scss";
@import "../../../Styles/variables.scss";
.playlist-element {
  display: flex;
  color: #a5a2a0;
  padding: 3px 0;
  font-size: 16px;
  text-align: left;
  .name {
    flex-grow: 1;
  }

  .glyphicon-download {
    float: right;
    margin-right: 15px;
  }

  &.small {
    font-size: 14px;
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
</style>
