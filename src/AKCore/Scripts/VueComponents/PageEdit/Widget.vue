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
          :model-value="modelValue"
          @update:modelValue="$emit('update:modelValue', $event)"
          :translate="translate"
        ></header-text>
        <hire
          v-if="modelValue.type === 'Hire'"
          :model-value="modelValue"
          @update:modelValue="$emit('update:modelValue', $event)"
          :translate="translate"
        ></hire>
        <image-widget
          v-if="modelValue.type === 'Image'"
          :model-value="modelValue"
          @update:modelValue="$emit('update:modelValue', $event)"
        ></image-widget>
        <join
          v-if="modelValue.type === 'Join'"
          :model-value="modelValue"
          @update:modelValue="$emit('update:modelValue', $event)"
          :translate="translate"
        ></join>
        <member-list v-if="modelValue.type === 'MemberList'"></member-list>
        <music
          v-if="modelValue.type === 'Music'"
          :model-value="modelValue"
          @update:modelValue="$emit('update:modelValue', $event)"
          :albums="albums"
        ></music>
        <post-list v-if="modelValue.type === 'PostList'"></post-list>
        <text-widget
          v-if="modelValue.type === 'Text'"
          :model-value="modelValue"
          @update:modelValue="$emit('update:modelValue', $event)"
          :translate="translate"
        ></text-widget>
        <text-image
          v-if="modelValue.type === 'TextImage'"
          :translate="translate"
          :model-value="modelValue"
          @update:modelValue="$emit('update:modelValue', $event)"
        ></text-image>
        <three-puffs
          v-if="modelValue.type === 'ThreePuffs'"
          :model-value="modelValue"
          @update:modelValue="$emit('update:modelValue', $event)"
          :translate="translate"
        >
        </three-puffs>

        <count-down
          v-if="modelValue.type === 'CountDown'"
          :model-value="modelValue"
          @update:modelValue="$emit('update:modelValue', $event)"
          :translate="translate"
        >
        </count-down>
        <video-widget
          v-if="modelValue.type === 'Video'"
          :model-value="modelValue"
          @update:modelValue="$emit('update:modelValue', $event)"
        ></video-widget>
      </div>
      <div class="col-xs-12">
        <a
          class="btn btn-default translate-btn"
          v-if="
            !translate &&
            modelValue.text &&
            !macrosWithNoTranslate.includes(modelValue.type)
          "
          @click.prevent="translate = true"
        >
          Översätt
        </a>

        <a
          class="btn btn-default translate-btn"
          v-if="translate && modelValue.text"
          @click.prevent="translate = false"
        >
          Minimera översättning
        </a>
      </div>
    </div>
  </li>
</template>
<script setup lang="ts">
import { onUpdated, ref } from "vue";
import { AlbumEditModel } from "../AlbumEdit/models";
import { getHeader } from "./functions";
import { WidgetEditModel } from "./models";
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
  CountDown,
} from "./Widgets/widgets";

const emit = defineEmits<{
  (e: "updated"): void;
  (e: "remove"): void;
  (e: "remove"): void;
  (e: "update:modelValue", value: WidgetEditModel): void;
}>();

defineProps<{
  modelValue: WidgetEditModel;
  albums: AlbumEditModel[];
}>();

const minimized = ref(false);
const translate = ref(false);

const macrosWithNoTranslate = ["Video"];

onUpdated(() => {
  emit("updated");
});

const minimize = () => {
  minimized.value = !minimized.value;
};

const remove = () => {
  if (window.confirm("Är du säker att du vill ta bort den här widgeten?")) {
    emit("remove");
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

.translate-btn {
  margin-top: 40px;
  display: block;
  width: max-content;
  margin-left: auto;
  margin-right: auto;
}
</style>
