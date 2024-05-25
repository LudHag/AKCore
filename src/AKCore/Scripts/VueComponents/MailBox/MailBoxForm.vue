<template>
  <form method="POST" action="/MailBox" @submit.prevent="sendMailBox">
    <div class="alert alert-danger" ref="error" style="display: none"></div>
    <div class="alert alert-success" ref="success" style="display: none">
      {{ t("message-sent") }}
    </div>
    <div class="form-group">
      <label>{{ t("subject") }}</label>
      <input
        required
        v-model="mailboxSubject"
        type="text"
        class="form-control"
        :placeholder="t('subject')"
        name="Subject"
      />
    </div>
    <div class="form-group">
      <label>{{ t("message") }}</label>
      <textarea
        required
        class="form-control"
        v-model="mailBoxMessage"
        name="Message"
      ></textarea>
    </div>
    <div class="form-group">
      <button type="submit" class="btn btn-primary">{{ t("send") }}</button>
    </div>
  </form>
</template>
<script setup lang="ts">
import { ref } from "vue";
import { defaultFormSend } from "../../services/apiservice";
import { TranslationDomain, translate } from "../../translations";

const emit = defineEmits<{
  (e: "sent"): void;
}>();

const mailboxSubject = ref("");
const mailBoxMessage = ref("");
const error = ref<HTMLElement | null>(null);
const success = ref<HTMLElement | null>(null);

const sendMailBox = (event: Event) => {
  defaultFormSend(
    event.target as HTMLFormElement,
    error.value,
    success.value,
    () => {
      mailboxSubject.value = "";
      mailBoxMessage.value = "";
      emit("sent");
    },
  );
};
const t = (key: string, domain: TranslationDomain = "mailbox") => {
  return translate(domain, key);
};
</script>
<style lang="scss" scoped></style>
