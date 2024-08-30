<template>
  <div>
    <a
      href="/Signup/Recruits"
      v-if="showRecruitsInfo"
      class="info-popup"
      :class="{ appear: appearRecruits }"
      ref="recruitsInfo"
      @click="saveHideRecruitsInfo"
    >
      Det finns {{ recruits }} olästa intresseanmälningar
    </a>
    <a
      href="/Mailbox"
      v-if="showMailboxInfo"
      class="info-popup"
      :class="{ appear: appearMailbox }"
      ref="mailboxInfo"
      @click="saveHideMailboxInfo"
    >
      Det finns {{ mails }} olästa brevlådeposter
    </a>
  </div>
</template>
<script setup lang="ts">
import { onMounted, ref } from "vue";
import { allowMovable } from "./utils";
import { setCookie } from "../../general";

const props = defineProps<{
  recruitsInfoDisabled: boolean;
  mailboxInfoDisabled?: boolean;
  recruits: number;
  mails: number;
}>();

const appearRecruits = ref(false);
const showRecruitsInfo = ref(!props.recruitsInfoDisabled && props.recruits > 0);
const recruitsInfo = ref<HTMLElement>();

const appearMailbox = ref(false);
const showMailboxInfo = ref(!props.mailboxInfoDisabled && props.mails > 0);
const mailboxInfo = ref<HTMLElement>();

const saveHideRecruitsInfo = () => {
  setCookie("recruitsPopup", "hide", 15);
};
const saveHideMailboxInfo = () => {
  setCookie("recruitsPopup", "hide", 15);
};

onMounted(() => {
  setTimeout(() => {
    appearRecruits.value = true;
  }, 100);
  if (recruitsInfo.value) {
    allowMovable(recruitsInfo.value, () => {
      showRecruitsInfo.value = false;
      saveHideRecruitsInfo();
    });
  }
});
</script>
<style lang="scss" scoped>
.info-popup {
  z-index: 1;
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
