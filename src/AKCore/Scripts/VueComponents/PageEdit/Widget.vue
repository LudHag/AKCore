<template>
  <li class="row widget" :class="value.type">
    <div class="widget-header">
      <h4>{{ getHeader(value.type) }}</h4>
      <span
        class="btn pull-right remove-widget glyphicon glyphicon-remove"
      ></span
      ><span class="btn pull-right min-widget glyphicon glyphicon-minus"></span>
    </div>
    <div class="widget-body">
      <text-image
        v-if="value.type === 'TextImage'"
        v-model="value"
      ></text-image>
      <three-puffs v-if="value.type === 'ThreePuffs'" v-model="value">
      </three-puffs>
    </div>
  </li>
</template>
<script>
import { getHeader } from "./functions";
import TextImage from "./Widgets/TextImage.vue";
import ThreePuffs from "./Widgets/ThreePuffs.vue";

export default {
  components: { TextImage, ThreePuffs },
  props: ["value"],
  methods: {
    getHeader
  }
};
</script>
<style lang="scss"></style>
