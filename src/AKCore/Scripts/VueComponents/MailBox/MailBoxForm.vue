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
<script>
import ApiService from "../../services/apiservice";

export default {
  data() {
    return {
      mailboxSubject: "",
      mailBoxMessage: "",
    };
  },
  methods: {
    sendMailBox(event) {
      const error = $($(".alert-danger")[0]);
      const success = $($(".alert-success")[0]);
      const form = $(event.target);
      ApiService.defaultFormSend(form, error, success, () => {
        this.mailboxSubject = "";
        this.mailBoxMessage = "";
        this.$emit("sent");
      });
    },
  },
};
</script>
<style lang="scss" scoped></style>
