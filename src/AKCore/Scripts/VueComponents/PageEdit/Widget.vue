<template>
  <li class="row widget" :class="value.type">
    <div class="widget-header">
      <h4>{{ getHeader(value.type) }}</h4>
      <span
        class="btn min-widget glyphicon glyphicon-minus"
        @click="minimize"
      ></span>
      <span
        class="btn remove-widget glyphicon glyphicon-remove"
        @click="remove"
      ></span>
    </div>
    <div class="widget-body row" v-if="!minimized">
      <div class="col-xs-12">
        <text-image
          v-if="value.type === 'TextImage'"
          v-model="value"
        ></text-image>
        <three-puffs v-if="value.type === 'ThreePuffs'" v-model="value">
        </three-puffs>
      </div>
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
  data() {
    return {
      minimized: false
    };
  },
  updated() {
    this.$emit("updated");
  },
  methods: {
    getHeader,
    minimize() {
      this.minimized = !this.minimized;
    },
    remove() {
      if (window.confirm("Är du säker att du vill ta bort den här widgeten?")) {
        this.$emit("remove");
      }
    }
  }
};
</script>
<style lang="scss" scoped>
.widget {
  background-color: white;
  margin-right: 0;
  margin-left: 0;
  margin-top: 20px;
  color: black;
  border-radius: 4px;
}
.widget-header {
  position: relative;
  display: flex;
  align-items: center;
  padding: 12px 15px 12px 40px;
  background-color: #ece8e5;
  border-radius: 4px;
}
.widget-body {
  padding-top: 20px;
  padding-bottom: 20px;
}

.min-widget {
  margin-left: auto;
}

.min-widget,
.remove-widget {
  font-size: 22px;
}
</style>
