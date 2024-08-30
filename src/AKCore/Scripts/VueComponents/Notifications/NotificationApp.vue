<template>
  <div>
    <a
      href="/Signup/Recruits"
      v-if="showNotificationInfo"
      class="info-popup"
      :class="{ appear }"
      ref="info"
    >
      Det finns 2 olästa intresseanmälningar
    </a>
  </div>
</template>
<script setup lang="ts">
import { onMounted, ref } from "vue";
import { allowMovable } from "./utils";
import { setCookie } from "../../general";

const props = defineProps<{
  notificationInfoDisabled: boolean;
}>();

const appear = ref(false);
const showNotificationInfo = ref(!props.notificationInfoDisabled);
const info = ref<HTMLElement>();

const saveHideNotificationInfo = () => {
  setCookie("notificationPopup", "hide", 15);
};

onMounted(() => {
  setTimeout(() => {
    appear.value = true;
  }, 100);
  if (info.value) {
    allowMovable(info.value, () => {
      showNotificationInfo.value = false;
      saveHideNotificationInfo();
    });
  }
});
</script>
<style lang="scss" scoped>
.info-popup {
  position: fixed;
  bottom: 10px;
  width: 200px;
  height: 50px;
  left: -100%;
  transition: left 0.5s ease-in-out;
  background-color: #6e1601;
  color: #dddddd;
  display: flex;
  justify-content: center;
  align-items: center;
  padding: 20px;
  &.appear {
    left: calc(50% - 100px);
  }
  &.dragged {
    transition: opacity 0.5s ease-in-out;
  }
  &.removed {
    opacity: 0;
  }
}
</style>
