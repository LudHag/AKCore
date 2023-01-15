<template>
  <form method="POST" action="/MailBox" @submit.prevent="sendMailBox">
    <div class="alert alert-danger" style="display: none"></div>
    <div class="alert alert-success" style="display: none">
      Meddelande skickat
    </div>
    <div class="form-group">
      <label>Ämne</label>
      <input
        required
        v-model="mailboxSubject"
        type="text"
        class="form-control"
        placeholder="Ämne"
        name="Subject"
      />
    </div>
    <div class="form-group">
      <label>Meddelande</label>
      <textarea
        required
        class="form-control"
        v-model="mailBoxMessage"
        name="Message"
      ></textarea>
    </div>
    <div class="form-group">
      <button type="submit" class="btn btn-primary">Skicka</button>
    </div>
  </form>
</template>
<script setup lang="ts">
import { ref } from "vue";
import { defaultFormSend } from "../../services/apiservice";

const emit = defineEmits<{
  (e: "sent"): void;
}>();

const mailboxSubject = ref("");
const mailBoxMessage = ref("");

const sendMailBox = (event: Event) => {
  const error = $(".alert-danger");
  const success = $(".alert-success");
  defaultFormSend(event.target as HTMLFormElement, error, success, () => {
    mailboxSubject.value = "";
    mailBoxMessage.value = "";
    emit("sent");
  });
};
</script>
<style lang="scss" scoped></style>
