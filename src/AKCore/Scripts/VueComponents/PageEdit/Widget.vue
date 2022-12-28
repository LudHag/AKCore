<template>
  <li class="row widget" :class="modelValue.type">
    <div class="widget-header">
      <h4>{{ getHeader(modelValue.type) }}</h4>
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
        <header-text
          v-if="modelValue.type === 'HeaderText'"
          value="value"
        ></header-text>
        <hire
          v-if="modelValue.type === 'Hire'"
          :modelValue="modelValue"
          @update:modelValue="modelValue = $event"
        ></hire>
        <image-widget
          v-if="modelValue.type === 'Image'"
          :modelValue="modelValue"
          @update:modelValue="modelValue = $event"
        ></image-widget>
        <join
          v-if="modelValue.type === 'Join'"
          :modelValue="modelValue"
          @update:modelValue="modelValue = $event"
        ></join>
        <member-list v-if="modelValue.type === 'MemberList'"></member-list>
        <music
          v-if="modelValue.type === 'Music'"
          :modelValue="modelValue"
          @update:modelValue="modelValue = $event"
          :albums="albums"
        ></music>
        <post-list v-if="modelValue.type === 'PostList'"></post-list>
        <text-widget
          v-if="modelValue.type === 'Text'"
          :modelValue="modelValue"
          @update:modelValue="modelValue = $event"
        ></text-widget>
        <text-image
          v-if="modelValue.type === 'TextImage'"
          :modelValue="modelValue"
          @update:modelValue="modelValue = $event"
        ></text-image>
        <three-puffs
          v-if="modelValue.type === 'ThreePuffs'"
          :modelValue="modelValue"
          @update:modelValue="modelValue = $event"
        >
        </three-puffs>
        <video-widget
          v-if="modelValue.type === 'Video'"
          :modelValue="modelValue"
          @update:modelValue="modelValue = $event"
        ></video-widget>
      </div>
    </div>
  </li>
</template>
<script>
import { getHeader } from "./functions";
import {
  TextImage,
  ThreePuffs,
  HeaderText,
  TextWidget,
  Hire,
  Join,
  MemberList,
  ImageWidget,
  Music,
  VideoWidget,
  PostList,
} from "./Widgets/widgets";

export default {
  components: {
    TextImage,
    ThreePuffs,
    HeaderText,
    TextWidget,
    Hire,
    Join,
    MemberList,
    ImageWidget,
    Music,
    VideoWidget,
    PostList,
  },
  props: ["modelValue", "albums"],
  data() {
    return {
      minimized: false,
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
    },
  },
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
